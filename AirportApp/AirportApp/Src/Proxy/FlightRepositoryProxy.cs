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
                var flightTransferObject = await httpClient.GetFromJsonAsync<AirportApp.ClassLibrary.Entity.Dto.FlightDTO>($"{BaseUrl}/{id}");
                if (flightTransferObject == null)
                {
                    return null;
                }

                return new Flight
                {
                    Id = flightTransferObject.id,
                    Gate = new Gate
                    {
                        Id = flightTransferObject.gateId
                    },
                    Date = flightTransferObject.date,
                    FlightNumber = flightTransferObject.flightNumber,
                    Route = flightTransferObject.route != null ? new Route
                    {
                        Id = flightTransferObject.route.id,
                        RouteType = flightTransferObject.route.routeType,
                        DepartureTime = flightTransferObject.route.departureTime,
                        ArrivalTime = flightTransferObject.route.arrivalTime,
                        Capacity = flightTransferObject.route.capacity,
                        Airport = flightTransferObject.route.airport != null ? new Airport { Id = flightTransferObject.route.airport.id, AirportCode = flightTransferObject.route.airport.airportCode, City = flightTransferObject.route.airport.city } : null!,
                        Company = flightTransferObject.route.company != null ? new Company { Id = flightTransferObject.route.company.id, Name = flightTransferObject.route.company.name } : null!
                    }
                    : null
                };
            }
            catch (HttpRequestException httpRequestException) when (httpRequestException.StatusCode == System.Net.HttpStatusCode.NotFound)
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

                var flightTransferObjectList = await httpClient.GetFromJsonAsync<IEnumerable<AirportApp.ClassLibrary.Entity.Dto.FlightDTO>>(query);
                if (flightTransferObjectList == null)
                {
                    return new List<Flight>();
                }

                var flights = new List<Flight>();
                foreach (var flightTransferObject in flightTransferObjectList)
                {
                    flights.Add(new Flight
                    {
                        Id = flightTransferObject.id,
                        Gate = new Gate
                        {
                            Id = flightTransferObject.gateId
                        },
                        Date = flightTransferObject.date,
                        FlightNumber = flightTransferObject.flightNumber,
                        Route = flightTransferObject.route != null ? new Route
                        {
                            Id = flightTransferObject.route.id,
                            RouteType = flightTransferObject.route.routeType,
                            DepartureTime = flightTransferObject.route.departureTime,
                            ArrivalTime = flightTransferObject.route.arrivalTime,
                            Capacity = flightTransferObject.route.capacity,
                            Airport = flightTransferObject.route.airport != null ? new Airport { Id = flightTransferObject.route.airport.id, AirportCode = flightTransferObject.route.airport.airportCode, City = flightTransferObject.route.airport.city } : null!,
                            Company = flightTransferObject.route.company != null ? new Company { Id = flightTransferObject.route.company.id, Name = flightTransferObject.route.company.name } : null!
                        }
                        : null
                    });
                }
                return flights;
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to retrieve flights by route from server.", httpRequestException);
            }
        }

        public async Task<int> GetOccupiedSeatCountAsync(int flightId)
        {
            try
            {
                var result = await httpClient.GetFromJsonAsync<int>($"{BaseUrl}/{flightId}/occupied-seat-count");
                return result;
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to retrieve occupied seat count for flight {flightId}.", httpRequestException);
            }
        }
    }
}
