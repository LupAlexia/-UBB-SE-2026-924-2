using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.Src.Service.Interfaces;

namespace AirportApp.Src.Proxy
{
    public class FlightSearchServiceProxy : IFlightSearchService
    {
        private const string DepartureRouteType = "Departure";
        private const string ArrivalRouteType = "Arrival";
        private const string FlightBaseUrl = "api/flight";

        private readonly HttpClient httpClient;

        public FlightSearchServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        // HTTP calls replace: flightRepository.GetFlightsByRouteAsync + flightRepository.GetOccupiedSeatCountAsync
        public async Task<IEnumerable<Flight>> SearchFlightsAsync(string location, bool isDeparture, DateTime? date, int? passengers)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                return new List<Flight>();
            }

            string flightType = isDeparture ? DepartureRouteType : ArrivalRouteType;
            string dateQuery = date.HasValue ? $"&date={date.Value:yyyy-MM-ddTHH:mm:ss}" : string.Empty;

            // HTTP GET replaces flightRepository.GetFlightsByRouteAsync
            var flights = await this.httpClient
                .GetFromJsonAsync<IEnumerable<Flight>>(
                    $"{FlightBaseUrl}/search?location={Uri.EscapeDataString(location)}&routeType={flightType}{dateQuery}");

            if (flights == null || !flights.Any())
            {
                return new List<Flight>();
            }

            var flightList = flights.ToList();

            if (passengers.HasValue && passengers.Value > 0)
            {
                var filteredFlights = new List<Flight>();
                foreach (var flight in flightList)
                {
                    // HTTP GET replaces flightRepository.GetOccupiedSeatCountAsync
                    int occupiedSeats = await this.httpClient
                        .GetFromJsonAsync<int>($"{FlightBaseUrl}/{flight.Id}/occupied-seat-count");

                    int availableSeats = (flight.Route?.Capacity ?? 0) - occupiedSeats;
                    if (availableSeats >= passengers.Value)
                    {
                        filteredFlights.Add(flight);
                    }
                }

                return filteredFlights;
            }

            return flightList;
        }

        // Pure logic — no DB access
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
    }
}
