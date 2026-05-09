using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.Src.Proxy
{
    public class FlightRepositoryProxy : IFlightRepository
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/flight";

        public FlightRepositoryProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<Flight?> GetFlightByIdAsync(int id)
        {
            try
            {
                var dto = await httpClient.GetFromJsonAsync<AirportApp.ClassLibrary.Entity.Dto.FlightDTO>($"{BaseUrl}/{id}");
                if (dto == null)
                {
                    return null;
                }

                return new Flight
                {
                    Id = dto.id,
                    RouteId = dto.routeId,
                    GateId = dto.gateId,
                    Date = dto.date,
                    FlightNumber = dto.flightNumber,
                    Route = dto.route != null ? new Route
                    {
                        Id = dto.route.id,
                        RouteType = dto.route.routeType,
                        DepartureTime = dto.route.departureTime,
                        ArrivalTime = dto.route.arrivalTime,
                        Capacity = dto.route.capacity,
                        Airport = dto.route.airport != null ? new Airport { Id = dto.route.airport.id, AirportCode = dto.route.airport.airportCode, City = dto.route.airport.city } : null!,
                        Company = dto.route.company != null ? new Company { Id = dto.route.company.id, Name = dto.route.company.name } : null!
                    }
                    : null
                };
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Server communication error while retrieving flight {id}.", ex);
            }
        }

        public async Task<IEnumerable<Flight>> GetFlightsByRouteAsync(string location, string routeType, DateTime? date)
        {
            try
            {
                var query = $"{BaseUrl}/search?location={Uri.EscapeDataString(location)}&routeType={Uri.EscapeDataString(routeType)}";
                if (date.HasValue)
                {
                    query += $"&date={date:yyyy-MM-dd}";
                }

                var dtos = await httpClient.GetFromJsonAsync<IEnumerable<AirportApp.ClassLibrary.Entity.Dto.FlightDTO>>(query);
                if (dtos == null)
                {
                    return new List<Flight>();
                }

                var flights = new List<Flight>();
                foreach (var dto in dtos)
                {
                    flights.Add(new Flight
                    {
                        Id = dto.id,
                        RouteId = dto.routeId,
                        GateId = dto.gateId,
                        Date = dto.date,
                        FlightNumber = dto.flightNumber,
                        Route = dto.route != null ? new Route
                        {
                            Id = dto.route.id,
                            RouteType = dto.route.routeType,
                            DepartureTime = dto.route.departureTime,
                            ArrivalTime = dto.route.arrivalTime,
                            Capacity = dto.route.capacity,
                            Airport = dto.route.airport != null ? new Airport { Id = dto.route.airport.id, AirportCode = dto.route.airport.airportCode, City = dto.route.airport.city } : null!,
                            Company = dto.route.company != null ? new Company { Id = dto.route.company.id, Name = dto.route.company.name } : null!
                        }
                        : null
                    });
                }
                return flights;
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to retrieve flights by route from server.", ex);
            }
        }

        public async Task<int> GetOccupiedSeatCountAsync(int flightId)
        {
            try
            {
                var result = await httpClient.GetFromJsonAsync<int>($"{BaseUrl}/{flightId}/occupied-seat-count");
                return result;
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to retrieve occupied seat count for flight {flightId}.", ex);
            }
        }
    }
}
