using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interfaces;

namespace AirportApp.ClassLibrary.Proxy.ServiceProxies
{
    public class FlightSearchServiceProxy : IFlightSearchService
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/flight";

        public FlightSearchServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<IEnumerable<Flight>> SearchFlightsAsync(string location, bool isDeparture, DateTime? date, int? passengers)
        {
            string routeType = isDeparture ? "Departure" : "Arrival";
            return await GetFlightsByRouteAsync(location, routeType, date);
        }

        public int? ParsePassengerCount(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            if (int.TryParse(input, out var parsed) && parsed > 0)
            {
                return parsed;
            }

            return 1;
        }

        public async Task<Flight?> GetFlightByIdAsync(int id)
        {
            try
            {
                FlightDTO flightTransferObject = await httpClient.GetFromJsonAsync<FlightDTO>($"{BaseUrl}/{id}");
                if (flightTransferObject == null)
                {
                    return null;
                }

                return MapFlightFromTransferObject(flightTransferObject);
            }
            catch (HttpRequestException httpRequestException) when (httpRequestException.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Server communication error while retrieving flight {id}.", httpRequestException);
            }
        }

        public async Task<IEnumerable<Flight>> GetFlightsByRouteAsync(string location, string routeType, DateTime? date)
        {
            try
            {
                string query = $"{BaseUrl}/search?location={Uri.EscapeDataString(location)}&routeType={Uri.EscapeDataString(routeType)}";
                if (date.HasValue)
                {
                    query += $"&date={date:yyyy-MM-dd}";
                }

                IEnumerable<FlightDTO> flightTransferObjectList = await httpClient.GetFromJsonAsync<IEnumerable<FlightDTO>>(query);
                if (flightTransferObjectList == null)
                {
                    return new List<Flight>();
                }

                var flights = new List<Flight>();
                foreach (FlightDTO flightTransferObject in flightTransferObjectList)
                {
                    flights.Add(MapFlightFromTransferObject(flightTransferObject));
                }

                return flights;
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException("Failed to retrieve flights by route.", httpRequestException);
            }
        }

        public async Task<int> GetOccupiedSeatCountAsync(int flightId)
        {
            try
            {
                int result = await httpClient.GetFromJsonAsync<int>($"{BaseUrl}/{flightId}/occupied-seat-count");
                return result;
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to retrieve occupied seat count for flight {flightId}.", httpRequestException);
            }
        }

        private static Flight MapFlightFromTransferObject(FlightDTO flightTransferObject)
        {
            return new Flight
            {
                Id = flightTransferObject.id,
                Gate = new Gate
                {
                    Id = flightTransferObject.gateId
                },
                Date = flightTransferObject.date,
                FlightNumber = flightTransferObject.flightNumber,
                Route = flightTransferObject.route != null
                    ? new Route
                    {
                        Id = flightTransferObject.route.id,
                        RouteType = flightTransferObject.route.routeType,
                        DepartureTime = flightTransferObject.route.departureTime,
                        ArrivalTime = flightTransferObject.route.arrivalTime,
                        Capacity = flightTransferObject.route.capacity,
                        Airport = flightTransferObject.route.airport != null
                            ? new Airport
                            {
                                Id = flightTransferObject.route.airport.id,
                                AirportCode = flightTransferObject.route.airport.airportCode,
                                City = flightTransferObject.route.airport.city
                            }
                            : null!,
                        Company = flightTransferObject.route.company != null
                            ? new Company
                            {
                                Id = flightTransferObject.route.company.id,
                                Name = flightTransferObject.route.company.name
                            }
                            : null!
                    }
                    : null
            };
        }
    }
}
