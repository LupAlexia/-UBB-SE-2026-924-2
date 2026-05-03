using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.ClassLibrary.Repository
{
    public class FlightRepository : IFlightRepository
    {
        private readonly IDatabaseConnectionFactory databaseConnectionFactory;

        public FlightRepository(IDatabaseConnectionFactory databaseConnectionFactory)
        {
            this.databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
        }

        public Flight? GetFlightById(int id)
        {
            Flight? flight = null;
            using (var connection = this.databaseConnectionFactory.GetConnection())
            {
                connection.Open();
                string query = @"
                    SELECT f.id as flight_id, f.date, f.flight_number,
                           r.id as route_id, r.route_type, r.departure_time, r.arrival_time, r.capacity,
                           a.id as airport_id, a.city, a.code as airport_code,
                           c.id as company_id, c.name as company_name,
                           g.id as gate_id, g.name as gate_name
                    FROM Flights f
                    INNER JOIN Routes r ON f.route_id = r.id
                    INNER JOIN Airports a ON r.airport_id = a.id
                    INNER JOIN Companies c ON r.company_id = c.id
                    LEFT JOIN Gates g ON f.gate_id = g.id
                    WHERE f.id = @FlightId";

                using (var getFlightCommand = new SqlCommand(query, connection))
                {
                    getFlightCommand.Parameters.AddWithValue("@FlightId", id);
                    using (var reader = getFlightCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            flight = this.MapFlight(reader);
                        }
                    }
                }
            }

            return flight;
        }

        public IEnumerable<Flight> GetFlightsByRoute(string location, string routeType, DateTime? date)
        {
            var flights = new List<Flight>();
            using (var connection = this.databaseConnectionFactory.GetConnection())
            {
                connection.Open();
                string query = @"
                    SELECT f.id as flight_id, f.date, f.flight_number,
                           r.id as route_id, r.route_type, r.departure_time, r.arrival_time, r.capacity,
                           a.id as airport_id, a.city, a.code as airport_code,
                           c.id as company_id, c.name as company_name,
                           g.id as gate_id, g.name as gate_name
                    FROM Flights f
                    INNER JOIN Routes r ON f.route_id = r.id
                    INNER JOIN Airports a ON r.airport_id = a.id
                    INNER JOIN Companies c ON r.company_id = c.id
                    LEFT JOIN Gates g ON f.gate_id = g.id
                    WHERE (@Date IS NULL OR CAST(f.date AS DATE) = CAST(@Date AS DATE))
                      AND r.route_type = @RouteType
                      AND (a.city = @Location OR a.code = @Location)";

                using (var getFlightsCommand = new SqlCommand(query, connection))
                {
                    getFlightsCommand.Parameters.AddWithValue("@Location", location);
                    getFlightsCommand.Parameters.AddWithValue("@RouteType", routeType);
                    getFlightsCommand.Parameters.AddWithValue("@Date", (object?)date ?? DBNull.Value);

                    using (var reader = getFlightsCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            flights.Add(this.MapFlight(reader));
                        }
                    }
                }
            }

            return flights;
        }

        public int GetOccupiedSeatCount(int flightId)
        {
            using (var connection = this.databaseConnectionFactory.GetConnection())
            {
                connection.Open();
                string query = @"
                    SELECT COUNT(*) 
                    FROM Tickets 
                    WHERE flight_id = @FlightId 
                      AND status <> 'Cancelled'";

                using (var getOccupiedSeatCountCommand = new SqlCommand(query, connection))
                {
                    getOccupiedSeatCountCommand.Parameters.AddWithValue("@FlightId", flightId);
                    return (int)getOccupiedSeatCountCommand.ExecuteScalar() !;
                }
            }
        }

        private Flight MapFlight(SqlDataReader reader)
        {
            var company = new Company
            {
                CompanyId = reader.GetInt32(reader.GetOrdinal("company_id")),
                Name = reader.GetString(reader.GetOrdinal("company_name"))
            };

            var airport = new Airport
            {
                AirportId = reader.GetInt32(reader.GetOrdinal("airport_id")),
                City = reader.IsDBNull(reader.GetOrdinal("city")) ? null : reader.GetString(reader.GetOrdinal("city")),
                AirportCode = reader.IsDBNull(reader.GetOrdinal("airport_code")) ? null : reader.GetString(reader.GetOrdinal("airport_code"))
            };

            var route = new Route
            {
                RouteId = reader.GetInt32(reader.GetOrdinal("route_id")),
                Company = company,
                Airport = airport,
                RouteType = reader.IsDBNull(reader.GetOrdinal("route_type")) ? null : reader.GetString(reader.GetOrdinal("route_type")),
                DepartureTime = reader.GetDateTime(reader.GetOrdinal("departure_time")),
                ArrivalTime = reader.GetDateTime(reader.GetOrdinal("arrival_time")),
                Capacity = reader.IsDBNull(reader.GetOrdinal("capacity")) ? 0 : reader.GetInt32(reader.GetOrdinal("capacity"))
            };

            Gate? gate = null;
            if (!reader.IsDBNull(reader.GetOrdinal("gate_id")))
            {
                gate = new Gate
                {
                    GateId = reader.GetInt32(reader.GetOrdinal("gate_id")),
                    GateName = reader.IsDBNull(reader.GetOrdinal("gate_name")) ? null : reader.GetString(reader.GetOrdinal("gate_name"))
                };
            }

            return new Flight
            {
                FlightId = reader.GetInt32(reader.GetOrdinal("flight_id")),
                Route = route,
                Gate = gate,
                Date = reader.GetDateTime(reader.GetOrdinal("date")),
                FlightNumber = reader.IsDBNull(reader.GetOrdinal("flight_number")) ? null : reader.GetString(reader.GetOrdinal("flight_number"))
            };
        }
    }
}
