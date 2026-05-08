using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.Src.Proxy
{
    public class ComplaintTicketRepositoryProxy : ITicketRepository
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/ticket";

        public ComplaintTicketRepositoryProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<ComplaintTicket>> GetAllAsync()
        {
            return await httpClient.GetFromJsonAsync<IEnumerable<ComplaintTicket>>(BaseUrl)
                   ?? new List<ComplaintTicket>();
        }

        public async Task<ComplaintTicket> GetByIdAsync(int id)
        {
            return await httpClient.GetFromJsonAsync<ComplaintTicket>($"{BaseUrl}/{id}")
                   ?? throw new KeyNotFoundException($"Ticket with id {id} not found.");
        }

        public async Task<int> CreateNewEntityAsync(ComplaintTicket ticket)
        {
            var dto = new CreateTicketDTO(
                creatorId: ticket.CreatorId,
                categoryId: ticket.CategoryId,
                subcategoryId: ticket.SubcategoryId,
                subject: ticket.Subject,
                description: ticket.Description,
                creationTimestamp: ticket.CreationTimestamp,
                currentStatus: ticket.CurrentStatus,
                urgencyLevel: ticket.UrgencyLevel);

            var response = await httpClient.PostAsJsonAsync(BaseUrl, dto);
            response.EnsureSuccessStatusCode();
            var created = await response.Content.ReadFromJsonAsync<ComplaintTicket>();
            return created!.Id;
        }

        public async Task UpdateByIdAsync(int id, ComplaintTicket ticket)
        {
            var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", ticket);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateStatusByIdAsync(int id, ComplaintTicketStatusEnum newStatus)
        {
            var request = new { currentStatus = newStatus };
            var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{id}/status", request);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateUrgencyLevelByIdAsync(int id, ComplaintTicketUrgencyLevelEnum newUrgencyLevel)
        {
            var request = new { urgencyLevel = newUrgencyLevel };
            var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{id}/urgency", request);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var response = await httpClient.DeleteAsync($"{BaseUrl}/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}