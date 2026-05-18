using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interfaces;

namespace AirportApp.ClassLibrary.Proxy.ServiceProxies
{
    public class CancellationServiceProxy : ICancellationService
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/flightticket";

        public CancellationServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public (bool CanCancel, string Reason) CanCancelTicket(FlightTicket ticket)
        {
            if (ticket == null)
            {
                return (false, "Ticket not found.");
            }

            if (string.Equals(ticket.Status, "Cancelled", StringComparison.OrdinalIgnoreCase))
            {
                return (false, "This ticket is already cancelled.");
            }

            if (ticket.Flight != null && ticket.Flight.Date < DateTime.Now)
            {
                return (false, "This flight is already in the past and cannot be cancelled.");
            }

            return (true, string.Empty);
        }

        public async Task CancelTicketAsync(int ticketId)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{ticketId}/status", "Cancelled");
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to cancel ticket {ticketId}.", httpRequestException);
            }
        }
    }
}
