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
    public class ComplaintTicketServiceProxy : IComplaintTicketService
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/ticket";

        public ComplaintTicketServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task CreateTicketAsync(int ticketId, User ticketCreator, ComplaintTicketStatusEnum initialStatus,
            ComplaintTicketCategory category, ComplaintTicketSubcategory subcategory,
            string subject, string description, DateTime creationTimestamp,
            ComplaintTicketUrgencyLevelEnum? initialUrgencyLevel = null)
        {
            try
            {
                var ticketCreationData = new CreateTicketDTO(
                    ticketCreator.Id,
                    category.Id,
                    subcategory.Id,
                    subject,
                    description,
                    creationTimestamp,
                    initialStatus,
                    initialUrgencyLevel ?? ComplaintTicketUrgencyLevelEnum.LOW);

                HttpResponseMessage response = await httpClient.PostAsJsonAsync(BaseUrl, ticketCreationData);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException("Failed to create complaint ticket.", httpRequestException);
            }
        }

        public async Task AddTicketAsync(ComplaintTicket ticketEntity)
        {
            try
            {
                var ticketCreationData = new CreateTicketDTO(
                    ticketEntity.Creator.Id,
                    ticketEntity.Category.Id,
                    ticketEntity.Subcategory.Id,
                    ticketEntity.Subject,
                    ticketEntity.Description,
                    ticketEntity.CreationTimestamp,
                    ticketEntity.CurrentStatus,
                    ticketEntity.UrgencyLevel);

                HttpResponseMessage response = await httpClient.PostAsJsonAsync(BaseUrl, ticketCreationData);
                response.EnsureSuccessStatusCode();

                ComplaintTicket createdTicket = await response.Content.ReadFromJsonAsync<ComplaintTicket>();
                if (createdTicket != null)
                {
                    ticketEntity.Id = createdTicket.Id;
                }
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException("Failed to add complaint ticket.", httpRequestException);
            }
        }

        public async Task DeleteTicketByIdAsync(int ticketId)
        {
            try
            {
                HttpResponseMessage response = await httpClient.DeleteAsync($"{BaseUrl}/{ticketId}");
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to delete complaint ticket {ticketId}.", httpRequestException);
            }
        }

        public async Task<ComplaintTicket> GetTicketByIdAsync(int ticketId)
        {
            try
            {
                ComplaintTicket ticket = await httpClient.GetFromJsonAsync<ComplaintTicket>($"{BaseUrl}/{ticketId}");
                if (ticket == null)
                {
                    throw new KeyNotFoundException($"Complaint ticket with id {ticketId} was not found.");
                }

                return ticket;
            }
            catch (HttpRequestException httpRequestException) when (httpRequestException.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new KeyNotFoundException($"Complaint ticket with id {ticketId} was not found.");
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Server communication error while retrieving ticket {ticketId}.", httpRequestException);
            }
        }

        public async Task<IEnumerable<ComplaintTicket>> GetAllTicketsAsync()
        {
            try
            {
                IEnumerable<ComplaintTicket> tickets = await httpClient.GetFromJsonAsync<IEnumerable<ComplaintTicket>>(BaseUrl);
                return tickets ?? new List<ComplaintTicket>();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException("Failed to retrieve all complaint tickets.", httpRequestException);
            }
        }

        public async Task UpdateTicketByIdAsync(int id, ComplaintTicket ticket)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", ticket);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to update complaint ticket {id}.", httpRequestException);
            }
        }

        public async Task UpdateUrgencyLevelAsync(int ticketId, ComplaintTicketUrgencyLevelEnum newUrgencyLevel)
        {
            try
            {
                var requestBody = new { UrgencyLevel = newUrgencyLevel };
                HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{ticketId}/urgency", requestBody);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to update urgency level for ticket {ticketId}.", httpRequestException);
            }
        }

        public async Task UpdateStatusAsync(int ticketId, ComplaintTicketStatusEnum newStatus)
        {
            try
            {
                var requestBody = new { CurrentStatus = newStatus };
                HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{ticketId}/status", requestBody);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to update status for ticket {ticketId}.", httpRequestException);
            }
        }

        public IEnumerable<TicketDTO> FilterTicketsByStatus(IEnumerable<TicketDTO> tickets, TicketFilterStatusEnum filter)
        {
            throw new NotSupportedException("FilterTicketsByStatus is not available through the service proxy.");
        }
    }
}
