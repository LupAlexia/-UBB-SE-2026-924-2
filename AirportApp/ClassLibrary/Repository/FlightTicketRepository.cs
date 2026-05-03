using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository
{
    public class FlightTicketRepository : IFlightTicketRepository
    {
        private const string CancelledStatus = "Cancelled";

        private readonly AirportDbContext dataBaseContext;

        public FlightTicketRepository(AirportDbContext dataBaseContext)
        {
            this.dataBaseContext = dataBaseContext;
        }

        public IEnumerable<FlightTicket> GetTicketsByUserId(int userId)
        {
<<<<<<< HEAD
            return this.dataBaseContext.flightTickets
                .Where(ticket => ticket.User.UserId == userId)
                .Include(ticket => ticket.User)
                .Include(ticket => ticket.Flight)
                    .ThenInclude(flight => flight.Route)
                        .ThenInclude(route => route.Airport)
                .Include(ticket => ticket.Flight)
                    .ThenInclude(flight => flight.Route)
                        .ThenInclude(route => route.Company)
                .Include(ticket => ticket.Flight)
                    .ThenInclude(flight => flight.Gate)
                .Include(ticket => ticket.SelectedAddOns)
                .ToList();
=======
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
                            var user = new Customer { Id = reader.GetInt32(reader.GetOrdinal("user_id")) };

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
                                Id = reader.GetInt32(reader.GetOrdinal("flight_id")),
                                FlightNumber = reader.IsDBNull(reader.GetOrdinal("flight_number")) ? null : reader.GetString(reader.GetOrdinal("flight_number")),
                                Date = reader.GetDateTime(reader.GetOrdinal("flight_date")),
                                Route = route,
                                Gate = gate
                            };

                            var ticket = new FlightTicket
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ticket_id")),
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
                            ticketById[ticket.Id] = ticket;
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
                                    Id = addOnReader.GetInt32(addOnReader.GetOrdinal("addon_id")),
                                    Name = addOnReader.GetString(addOnReader.GetOrdinal("name")),
                                    BasePrice = (float)addOnReader.GetDecimal(addOnReader.GetOrdinal("base_price"))
                                });
                            }
                        }
                    }
                }
            }

            return tickets;
>>>>>>> 051c05a48fe748e4590e602386a410d82a6ad572
        }

        public void AddTicket(FlightTicket ticket)
        {
<<<<<<< HEAD
            this.dataBaseContext.Add(ticket);
            this.dataBaseContext.SaveChanges();
=======
            using (var connection = this.databaseConnectionFactory.GetConnection())
            {
                connection.Open();
                string query = @"
                    INSERT INTO Tickets (user_id, flight_id, seat, price, status, passenger_first_name, passenger_last_name, passenger_email, passenger_phone)
                    OUTPUT INSERTED.ticket_id
                    VALUES (@UserId, @FlightId, @Seat, @Price, @Status, @PassengerFirstName, @PassengerLastName, @PassengerEmail, @PassengerPhone)";

                using (var insertTicketCommand = new SqlCommand(query, connection))
                {
                    insertTicketCommand.Parameters.AddWithValue("@UserId", ticket.User!.Id);
                    insertTicketCommand.Parameters.AddWithValue("@FlightId", ticket.Flight!.Id);
                    insertTicketCommand.Parameters.AddWithValue("@Seat", ticket.Seat ?? (object)DBNull.Value);
                    insertTicketCommand.Parameters.AddWithValue("@Price", ticket.Price);
                    insertTicketCommand.Parameters.AddWithValue("@Status", ticket.Status ?? (object)DBNull.Value);
                    insertTicketCommand.Parameters.AddWithValue("@PassengerFirstName", ticket.PassengerFirstName ?? (object)DBNull.Value);
                    insertTicketCommand.Parameters.AddWithValue("@PassengerLastName", ticket.PassengerLastName ?? (object)DBNull.Value);
                    insertTicketCommand.Parameters.AddWithValue("@PassengerEmail", ticket.PassengerEmail ?? (object)DBNull.Value);
                    insertTicketCommand.Parameters.AddWithValue("@PassengerPhone", ticket.PassengerPhone ?? (object)DBNull.Value);

                    ticket.Id = (int)insertTicketCommand.ExecuteScalar()!;
                }
            }
>>>>>>> 051c05a48fe748e4590e602386a410d82a6ad572
        }

        public void UpdateTicketStatus(int ticketId, string status)
        {
            var ticket = this.dataBaseContext.flightTickets.FirstOrDefault(t => t.TicketId == ticketId);
            if (ticket != null)
            {
                ticket.Status = status;
                this.dataBaseContext.SaveChanges();
            }
        }

        public void AddTicketAddOns(int ticketId, IEnumerable<int> addOnIds)
        {
            if (addOnIds == null || !addOnIds.Any())
            {
                return;
            }

            var ticket = this.dataBaseContext.flightTickets
                .Include(t => t.SelectedAddOns)
                .FirstOrDefault(t => t.TicketId == ticketId);

            if (ticket == null)
            {
                return;
            }

            foreach (var addOnId in addOnIds)
            {
                var addOn = this.dataBaseContext.addOns.FirstOrDefault(a => a.AddOnId == addOnId);
                if (addOn != null && !ticket.SelectedAddOns.Contains(addOn))
                {
                    ticket.SelectedAddOns.Add(addOn);
                }
            }

            this.dataBaseContext.SaveChanges();
        }

        public IEnumerable<string> GetOccupiedSeats(int flightId)
        {
            return this.dataBaseContext.flightTickets
                .Where(ticket => ticket.Flight.FlightId == flightId 
                    && ticket.Status != CancelledStatus 
                    && ticket.Seat != null)
                .Select(ticket => ticket.Seat)
                .ToList();
        }

        public async Task<bool> IsSeatAvailable(int flightId, string seat)
        {
            int count = await this.dataBaseContext.flightTickets
                .Where(ticket => ticket.Flight.FlightId == flightId 
                    && ticket.Seat == seat 
                    && ticket.Status != CancelledStatus)
                .CountAsync();

            return count == 0;
        }

        public async Task<bool> SaveTicketsWithAddOnsAsync(List<FlightTicket> tickets)
        {
            try
            {
                foreach (var ticket in tickets)
                {
<<<<<<< HEAD
                    this.dataBaseContext.flightTickets.Add(ticket);
=======
                    string insertTicketQuery = @"
                        INSERT INTO Tickets (user_id, flight_id, seat, price, status, passenger_first_name, passenger_last_name, passenger_email, passenger_phone)
                        OUTPUT INSERTED.ticket_id
                        VALUES (@userId, @flightId, @seat, @price, @status, @firstName, @lastName, @email, @phone)";

                    using var insertTicketCommand = new SqlCommand(insertTicketQuery, connection, transaction);
                    float persistedPrice = ticket.Price;
                    insertTicketCommand.Parameters.AddWithValue("@userId", ticket.User?.Id ?? 0);
                    insertTicketCommand.Parameters.AddWithValue("@flightId", ticket.Flight?.Id ?? 0);
                    insertTicketCommand.Parameters.AddWithValue("@seat", ticket.Seat ?? (object)DBNull.Value);
                    insertTicketCommand.Parameters.AddWithValue("@price", (decimal)persistedPrice);
                    insertTicketCommand.Parameters.AddWithValue("@status", ticket.Status ?? (object)DBNull.Value);
                    insertTicketCommand.Parameters.AddWithValue("@firstName", ticket.PassengerFirstName ?? (object)DBNull.Value);
                    insertTicketCommand.Parameters.AddWithValue("@lastName", ticket.PassengerLastName ?? (object)DBNull.Value);
                    insertTicketCommand.Parameters.AddWithValue("@email", ticket.PassengerEmail ?? (object)DBNull.Value);
                    insertTicketCommand.Parameters.AddWithValue("@phone", ticket.PassengerPhone ?? (object)DBNull.Value);

                    var newTicketId = Convert.ToInt32(await insertTicketCommand.ExecuteScalarAsync() ?? 0);
                    ticket.Id = newTicketId;

                    if (ticket.SelectedAddOns != null && ticket.SelectedAddOns.Any())
                    {
                        string insertAddonQuery = @"
                            INSERT INTO Tickets_AddOns (ticket_id, addon_id)
                            VALUES (@ticketId, @addonId)";

                        foreach (var addon in ticket.SelectedAddOns)
                        {
                            using var insertAddOnCommand = new SqlCommand(insertAddonQuery, connection, transaction);
                            insertAddOnCommand.Parameters.AddWithValue("@ticketId", newTicketId);
                            insertAddOnCommand.Parameters.AddWithValue("@addonId", addon.Id);
                            await insertAddOnCommand.ExecuteNonQueryAsync();
                        }
                    }
>>>>>>> 051c05a48fe748e4590e602386a410d82a6ad572
                }

                await this.dataBaseContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}


