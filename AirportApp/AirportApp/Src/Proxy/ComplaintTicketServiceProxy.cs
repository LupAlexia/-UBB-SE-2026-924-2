using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.Src.Service.Interfaces;
using User = AirportApp.ClassLibrary.Entity.Domain.User; // alias ca in service, pentru a evita confuzia cu User-ul din ViewModel parca
using AirportApp.Src.ViewModel;
namespace AirportApp.Src.Proxy
{
    public class ComplaintTicketServiceProxy : IComplaintTicketService
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/ticket";

        public ComplaintTicketServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<ComplaintTicket>> GetAllTicketsAsync()
        {
            return await httpClient.GetFromJsonAsync<IEnumerable<ComplaintTicket>>(BaseUrl);
        }

        public async Task<ComplaintTicket> GetTicketByIdAsync(int ticketId)
        {
            return await httpClient.GetFromJsonAsync<ComplaintTicket>($"{BaseUrl}/{ticketId}");
        }

        public async Task AddTicketAsync(ComplaintTicket ticketEntity)
        {
            var dto = new CreateTicketDTO(
                creatorId: ticketEntity.Creator.Id,
                categoryId: ticketEntity.Category.Id,
                subcategoryId: ticketEntity.Subcategory.Id,
                subject: ticketEntity.Subject,
                description: ticketEntity.Description,
                creationTimestamp: ticketEntity.CreationTimestamp,
                currentStatus: ticketEntity.CurrentStatus,
                urgencyLevel: ticketEntity.UrgencyLevel);

            var response = await httpClient.PostAsJsonAsync(BaseUrl, dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task CreateTicketAsync(int ticketId, User ticketCreator, ComplaintTicketStatusEnum initialStatus,
            ComplaintTicketCategory category, ComplaintTicketSubcategory subcategory,
            string subject, string description, DateTime creationTimestamp,
            ComplaintTicketUrgencyLevelEnum? initialUrgencyLevel = null)
        {
            var ticket = new ComplaintTicket(ticketId, ticketCreator, initialStatus, category,
                subcategory, subject, description, creationTimestamp, initialUrgencyLevel);
            await AddTicketAsync(ticket);
        }

        public async Task UpdateStatusAsync(int ticketId, ComplaintTicketStatusEnum newStatus)
        {
            var updateRequest = new { currentStatus = newStatus };
            var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{ticketId}/status", updateRequest);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateTicketByIdAsync(int id, ComplaintTicket ticket)
        {
            var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", ticket);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteTicketByIdAsync(int ticketId)
        {
            var response = await httpClient.DeleteAsync($"{BaseUrl}/{ticketId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateUrgencyLevelAsync(int ticketId, ComplaintTicketUrgencyLevelEnum newUrgencyLevel)
        {
            var request = new { urgencyLevel = newUrgencyLevel };
            var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{ticketId}/urgency", request);
            response.EnsureSuccessStatusCode();
        }

        // logica pura, fara DB, ramane local
        public IEnumerable<TicketDTO> FilterTicketsByStatus(IEnumerable<TicketDTO> tickets, TicketFilterStatusEnum filter)
        {
            return filter switch
            {
                TicketFilterStatusEnum.OPEN => tickets.Where(t => t.currentStatus == ComplaintTicketStatusEnum.OPEN),
                TicketFilterStatusEnum.IN_PROGRESS => tickets.Where(t => t.currentStatus == ComplaintTicketStatusEnum.IN_PROGRESS),
                TicketFilterStatusEnum.RESOLVED => tickets.Where(t => t.currentStatus == ComplaintTicketStatusEnum.RESOLVED),
                _ => tickets
            };
        }
    }
}