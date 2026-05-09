using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.Src.Service.Interfaces;

namespace AirportApp.Src.Proxy
{
    public class CancellationServiceProxy : ICancellationService
    {
        private const string FlightTicketBaseUrl = "api/flightticket";
        private const string CancelledStatus = "Cancelled";

        private readonly HttpClient httpClient;

        public CancellationServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        // Pure logic — status and date checks, no DB access
        public (bool CanCancel, string Reason) CanCancelTicket(FlightTicket ticket)
        {
            if (ticket == null)
            {
                return (false, "Ticket not found.");
            }

            if (string.Equals(ticket.Status, CancelledStatus, StringComparison.OrdinalIgnoreCase))
            {
                return (false, "This ticket is already cancelled.");
            }

            if (ticket.Flight != null && ticket.Flight.Date < DateTime.Now)
            {
                return (false, "This flight is already in the past and cannot be cancelled.");
            }

            return (true, string.Empty);
        }

        // HTTP call replaces: ticketRepository.UpdateTicketStatusAsync
        public async Task CancelTicketAsync(int ticketId)
        {
            HttpResponseMessage response = await this.httpClient
                .PutAsJsonAsync($"{FlightTicketBaseUrl}/{ticketId}/status", CancelledStatus);
            response.EnsureSuccessStatusCode();
        }
    }
}
