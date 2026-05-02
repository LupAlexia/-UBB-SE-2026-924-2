using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using AirportApp.Src.Domain;

namespace AirportApp.Src.Repository
{
    public class FlightTicketRepository : IFlightTicketRepository
    {
        private const string CancelledStatus = "Cancelled";

        private readonly IDatabaseConnectionFactory databaseConnectionFactory;

        public FlightTicketRepository(IDatabaseConnectionFactory databaseConnectionFactory)
        {
            this.databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
        }

        public IEnumerable<FlightTicket> GetTicketsByUserId(int userId)
        {
            var tickets = new List<FlightTicket>();
            var ticketById = new Dictionary<int, FlightTicket>();
            using (var connection = this.databaseConnectionFactory.GetConnection())
            {
                connection.Open();
                string query = @"
                    SELECT t.ticket_id, t.user_id, t.flight_id, t.seat, t.price, t.status,
                           t.passenger_first_name, t.passenger_last_name, t.passenger_email, t.passenger_phone,
                           f.flight_number, f.date as flight_date,
                           r.route_type, r.departure_time, r.arrival_time,
                           a.city, a.code as airport_code,
                           g.name as gate_name
                    FROM Tickets t
                    INNER JOIN Flights f ON t.flight_id = f.id
                    INNER JOIN Routes r ON f.route_id = r.id
                    INNER JOIN Airports a ON r.airport_id = a.id
                    LEFT JOIN Gates g ON f.gate_id = g.id
                    WHERE t.user_id = @UserId";

                using (var getUserTicketsCommand = new SqlCommand(query, connection))
                {
                    getUserTicketsCommand.Parameters.AddWithValue("@UserId", userId);

                    using (var reader = getUserTicketsCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var user = new Customer { UserId = reader.GetInt32(reader.GetOrdinal("user_id")) };

                            var airport = new Airport
                            {
                                City = reader.IsDBNull(reader.GetOrdinal("city")) ? null : reader.GetString(reader.GetOrdinal("city")),
                                AirportCode = reader.IsDBNull(reader.GetOrdinal("airport_code")) ? null : reader.GetString(reader.GetOrdinal("airport_code"))
                            };

                            var route = new Route
                            {
                                RouteType = reader.IsDBNull(reader.GetOrdinal("route_type")) ? null : reader.GetString(reader.GetOrdinal("route_type")),
                                DepartureTime = reader.GetDateTime(reader.GetOrdinal("departure_time")),
                                ArrivalTime = reader.GetDateTime(reader.GetOrdinal("arrival_time")),
                                Airport = airport
                            };

                            Gate? gate = null;
                            if (!reader.IsDBNull(reader.GetOrdinal("gate_name")))
                            {
                                gate = new Gate
                                {
                                    GateName = reader.GetString(reader.GetOrdinal("gate_name"))
                                };
                            }

                            var flight = new Flight
                            {
                                FlightId = reader.GetInt32(reader.GetOrdinal("flight_id")),
                                FlightNumber = reader.IsDBNull(reader.GetOrdinal("flight_number")) ? null : reader.GetString(reader.GetOrdinal("flight_number")),
                                Date = reader.GetDateTime(reader.GetOrdinal("flight_date")),
                                Route = route,
                                Gate = gate
                            };

                            var ticket = new FlightTicket
                            {
                                TicketId = reader.GetInt32(reader.GetOrdinal("ticket_id")),
                                User = user,
                                Flight = flight,
                                Seat = reader.IsDBNull(reader.GetOrdinal("seat")) ? null : reader.GetString(reader.GetOrdinal("seat")),
                                Price = (float)reader.GetDecimal(reader.GetOrdinal("price")),
                                Status = reader.IsDBNull(reader.GetOrdinal("status")) ? null : reader.GetString(reader.GetOrdinal("status")),
                                PassengerFirstName = reader.IsDBNull(reader.GetOrdinal("passenger_first_name")) ? null : reader.GetString(reader.GetOrdinal("passenger_first_name")),
                                PassengerLastName = reader.IsDBNull(reader.GetOrdinal("passenger_last_name")) ? null : reader.GetString(reader.GetOrdinal("passenger_last_name")),
                                PassengerEmail = reader.IsDBNull(reader.GetOrdinal("passenger_email")) ? null : reader.GetString(reader.GetOrdinal("passenger_email")),
                                PassengerPhone = reader.IsDBNull(reader.GetOrdinal("passenger_phone")) ? null : reader.GetString(reader.GetOrdinal("passenger_phone"))
                            };

                            tickets.Add(ticket);
                            ticketById[ticket.TicketId] = ticket;
                        }
                    }
                }

                if (ticketById.Count > 0)
                {
                    var parameters = ticketById.Keys
                        .Select((identifier, index) => new { ParameterName = $"@TicketId{index}", Value = identifier })
                        .ToList();

                    string inClause = string.Join(", ", parameters.Select(parameter => parameter.ParameterName));
                    string addOnQuery = $@"
                        SELECT ta.ticket_id, a.addon_id, a.name, a.base_price
                        FROM Tickets_AddOns ta
                        INNER JOIN AddOns a ON ta.addon_id = a.addon_id
                        WHERE ta.ticket_id IN ({inClause})";

                    using (var addOnCommand = new SqlCommand(addOnQuery, connection))
                    {
                        foreach (var parameter in parameters)
                        {
                            addOnCommand.Parameters.AddWithValue(parameter.ParameterName, parameter.Value);
                        }

                        using (var addOnReader = addOnCommand.ExecuteReader())
                        {
                            while (addOnReader.Read())
                            {
                                int ticketId = addOnReader.GetInt32(addOnReader.GetOrdinal("ticket_id"));
                                if (!ticketById.TryGetValue(ticketId, out var ticket))
                                {
                                    continue;
                                }

                                ticket.SelectedAddOns.Add(new AddOn
                                {
                                    AddOnId = addOnReader.GetInt32(addOnReader.GetOrdinal("addon_id")),
                                    Name = addOnReader.GetString(addOnReader.GetOrdinal("name")),
                                    BasePrice = (float)addOnReader.GetDecimal(addOnReader.GetOrdinal("base_price"))
                                });
                            }
                        }
                    }
                }
            }

            return tickets;
        }

        public void AddTicket(FlightTicket ticket)
        {
            using (var connection = this.databaseConnectionFactory.GetConnection())
            {
                connection.Open();
                string query = @"
                    INSERT INTO Tickets (user_id, flight_id, seat, price, status, passenger_first_name, passenger_last_name, passenger_email, passenger_phone)
                    OUTPUT INSERTED.ticket_id
                    VALUES (@UserId, @FlightId, @Seat, @Price, @Status, @PassengerFirstName, @PassengerLastName, @PassengerEmail, @PassengerPhone)";

                using (var insertTicketCommand = new SqlCommand(query, connection))
                {
                    insertTicketCommand.Parameters.AddWithValue("@UserId", ticket.User!.UserId);
                    insertTicketCommand.Parameters.AddWithValue("@FlightId", ticket.Flight!.FlightId);
                    insertTicketCommand.Parameters.AddWithValue("@Seat", ticket.Seat ?? (object)DBNull.Value);
                    insertTicketCommand.Parameters.AddWithValue("@Price", ticket.Price);
                    insertTicketCommand.Parameters.AddWithValue("@Status", ticket.Status ?? (object)DBNull.Value);
                    insertTicketCommand.Parameters.AddWithValue("@PassengerFirstName", ticket.PassengerFirstName ?? (object)DBNull.Value);
                    insertTicketCommand.Parameters.AddWithValue("@PassengerLastName", ticket.PassengerLastName ?? (object)DBNull.Value);
                    insertTicketCommand.Parameters.AddWithValue("@PassengerEmail", ticket.PassengerEmail ?? (object)DBNull.Value);
                    insertTicketCommand.Parameters.AddWithValue("@PassengerPhone", ticket.PassengerPhone ?? (object)DBNull.Value);

                    ticket.TicketId = (int)insertTicketCommand.ExecuteScalar()!;
                }
            }
        }

        public void UpdateTicketStatus(int ticketId, string status)
        {
            using (var connection = this.databaseConnectionFactory.GetConnection())
            {
                connection.Open();
                string query = "UPDATE Tickets SET status = @Status WHERE ticket_id = @TicketId";

                using (var updateTicketStatusCommand = new SqlCommand(query, connection))
                {
                    updateTicketStatusCommand.Parameters.AddWithValue("@TicketId", ticketId);
                    updateTicketStatusCommand.Parameters.AddWithValue("@Status", status);
                    updateTicketStatusCommand.ExecuteNonQuery();
                }
            }
        }

        public void AddTicketAddOns(int ticketId, IEnumerable<int> addOnIds)
        {
            if (addOnIds == null)
            {
                return;
            }

            using (var connection = this.databaseConnectionFactory.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string query = "INSERT INTO Tickets_AddOns (ticket_id, addon_id) VALUES (@TicketId, @AddOnId)";

                        using (var insertTicketAddOnCommand = new SqlCommand(query, connection, transaction))
                        {
                            insertTicketAddOnCommand.Parameters.Add("@TicketId", System.Data.SqlDbType.Int);
                            insertTicketAddOnCommand.Parameters.Add("@AddOnId", System.Data.SqlDbType.Int);

                            foreach (var addOnId in addOnIds)
                            {
                                insertTicketAddOnCommand.Parameters["@TicketId"].Value = ticketId;
                                insertTicketAddOnCommand.Parameters["@AddOnId"].Value = addOnId;
                                insertTicketAddOnCommand.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public IEnumerable<string> GetOccupiedSeats(int flightId)
        {
            var seats = new List<string>();
            using (var connection = this.databaseConnectionFactory.GetConnection())
            {
                connection.Open();
                string query = $"SELECT seat FROM Tickets WHERE flight_id = @FlightId AND status != '{CancelledStatus}' AND seat IS NOT NULL";

                using (var getOccupiedSeatsCommand = new SqlCommand(query, connection))
                {
                    getOccupiedSeatsCommand.Parameters.AddWithValue("@FlightId", flightId);

                    using (var reader = getOccupiedSeatsCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            seats.Add(reader.GetString(reader.GetOrdinal("seat")));
                        }
                    }
                }
            }

            return seats;
        }

        public async Task<bool> IsSeatAvailable(int flightId, string seat)
        {
            using var connection = this.databaseConnectionFactory.GetConnection();
            await connection.OpenAsync();
            string query = @"SELECT COUNT(*) FROM Tickets WHERE flight_id = @flightId AND seat = @seat AND status <> @cancelledStatus";

            using var checkSeatAvailabilityCommand = new SqlCommand(query, connection);
            checkSeatAvailabilityCommand.Parameters.AddWithValue("@flightId", flightId);
            checkSeatAvailabilityCommand.Parameters.AddWithValue("@seat", seat);
            checkSeatAvailabilityCommand.Parameters.AddWithValue("@cancelledStatus", CancelledStatus);

            int count = Convert.ToInt32(await checkSeatAvailabilityCommand.ExecuteScalarAsync() ?? 0);
            return count == 0;
        }

        public async Task<bool> SaveTicketsWithAddOnsAsync(List<FlightTicket> tickets)
        {
            using var connection = this.databaseConnectionFactory.GetConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var ticket in tickets)
                {
                    string insertTicketQuery = @"
                        INSERT INTO Tickets (user_id, flight_id, seat, price, status, passenger_first_name, passenger_last_name, passenger_email, passenger_phone)
                        OUTPUT INSERTED.ticket_id
                        VALUES (@userId, @flightId, @seat, @price, @status, @firstName, @lastName, @email, @phone)";

                    using var insertTicketCommand = new SqlCommand(insertTicketQuery, connection, transaction);
                    float persistedPrice = ticket.Price;
                    insertTicketCommand.Parameters.AddWithValue("@userId", ticket.User?.UserId ?? 0);
                    insertTicketCommand.Parameters.AddWithValue("@flightId", ticket.Flight?.FlightId ?? 0);
                    insertTicketCommand.Parameters.AddWithValue("@seat", ticket.Seat ?? (object)DBNull.Value);
                    insertTicketCommand.Parameters.AddWithValue("@price", (decimal)persistedPrice);
                    insertTicketCommand.Parameters.AddWithValue("@status", ticket.Status ?? (object)DBNull.Value);
                    insertTicketCommand.Parameters.AddWithValue("@firstName", ticket.PassengerFirstName ?? (object)DBNull.Value);
                    insertTicketCommand.Parameters.AddWithValue("@lastName", ticket.PassengerLastName ?? (object)DBNull.Value);
                    insertTicketCommand.Parameters.AddWithValue("@email", ticket.PassengerEmail ?? (object)DBNull.Value);
                    insertTicketCommand.Parameters.AddWithValue("@phone", ticket.PassengerPhone ?? (object)DBNull.Value);

                    var newTicketId = Convert.ToInt32(await insertTicketCommand.ExecuteScalarAsync() ?? 0);
                    ticket.TicketId = newTicketId;

                    if (ticket.SelectedAddOns != null && ticket.SelectedAddOns.Any())
                    {
                        string insertAddonQuery = @"
                            INSERT INTO Tickets_AddOns (ticket_id, addon_id)
                            VALUES (@ticketId, @addonId)";

                        foreach (var addon in ticket.SelectedAddOns)
                        {
                            using var insertAddOnCommand = new SqlCommand(insertAddonQuery, connection, transaction);
                            insertAddOnCommand.Parameters.AddWithValue("@ticketId", newTicketId);
                            insertAddOnCommand.Parameters.AddWithValue("@addonId", addon.AddOnId);
                            await insertAddOnCommand.ExecuteNonQueryAsync();
                        }
                    }
                }

                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}


