using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.Src.Proxy
{
    public class FlightTicketRepositoryProxy : IFlightTicketRepository
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/flightticket";

        public FlightTicketRepositoryProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<IEnumerable<FlightTicket>> GetTicketsByUserIdAsync(int userId)
        {
            try
            {
                return await httpClient.GetFromJsonAsync<IEnumerable<FlightTicket>>($"{BaseUrl}/user/{userId}")
                       ?? new List<FlightTicket>();
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to retrieve tickets for user {userId}.", ex);
            }
        }

        public async Task AddTicketAsync(FlightTicket ticket)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync(BaseUrl, ticket);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException("Failed to add ticket to server.", ex);
            }
        }

        public async Task UpdateTicketStatusAsync(int ticketId, string status)
        {
            try
            {
                var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{ticketId}/status", status);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to update status for ticket {ticketId}.", ex);
            }
        }

        public async Task AddTicketAddOnsAsync(int ticketId, IEnumerable<int> addOnIds)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/{ticketId}/addons", addOnIds);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to add add-ons to ticket {ticketId}.", ex);
            }
        }

        public async Task<IEnumerable<string>> GetOccupiedSeatsAsync(int flightId)
        {
            try
            {
                return await httpClient.GetFromJsonAsync<IEnumerable<string>>($"{BaseUrl}/flight/{flightId}/occupied-seats")
                       ?? new List<string>();
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to retrieve occupied seats for flight {flightId}.", ex);
            }
        }

        public async Task<bool> IsSeatAvailableAsync(int flightId, string seat)
        {
            try
            {
                var result = await httpClient.GetFromJsonAsync<bool>($"{BaseUrl}/flight/{flightId}/seat-available/{Uri.EscapeDataString(seat)}");
                return result;
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to check seat availability for flight {flightId}.", ex);
            }
        }

        public async Task<bool> SaveTicketsWithAddOnsAsync(List<FlightTicket> tickets, List<List<int>> addOnIds = null)
        {
            try
            {
                var request = new SaveTicketsRequest
                {
                    Tickets = tickets,
                    AddOnIds = addOnIds ?? new List<List<int>>()
                };

                var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/batch", request);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<bool>();
                return result;
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException("Failed to save tickets with add-ons to server.", ex);
            }
        }
    }
}