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
            throw new NotSupportedException("CanCancelTicket is not available through the service proxy.");
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
