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
                return await httpClient.GetFromJsonAsync<Flight>($"{BaseUrl}/{id}");
            }
            catch (HttpRequestException ex)
            {
                throw new KeyNotFoundException($"Flight with id {id} not found.", ex);
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

                return await httpClient.GetFromJsonAsync<IEnumerable<Flight>>(query)
                       ?? new List<Flight>();
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
