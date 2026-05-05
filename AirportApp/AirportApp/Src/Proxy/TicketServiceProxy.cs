using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.Src.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using User = AirportApp.ClassLibrary.Entity.Domain.User; // alias ca in service, pentru a evita confuzia cu User-ul din ViewModel parca
using AirportApp.Src.ViewModel;
namespace AirportApp.Src.Proxy
{
    public class TicketServiceProxy : ITicketService
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/ticket";

        public TicketServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<Ticket>> GetAllTicketsAsync()
        {
            return await httpClient.GetFromJsonAsync<IEnumerable<Ticket>>(BaseUrl);
        }

        public async Task<Ticket> GetTicketByIdAsync(int ticketId)
        {
            return await httpClient.GetFromJsonAsync<Ticket>($"{BaseUrl}/{ticketId}");
        }

        public async Task AddTicketAsync(Ticket ticketEntity)
        {
            var dto = new CreateTicketDTO(
                CreatorId: ticketEntity.CreatorId,
                CategoryId: ticketEntity.CategoryId,
                SubcategoryId: ticketEntity.SubcategoryId,
                Subject: ticketEntity.Subject,
                Description: ticketEntity.Description,
                CreationTimestamp: ticketEntity.CreationTimestamp,
                CurrentStatus: ticketEntity.CurrentStatus,
                UrgencyLevel: ticketEntity.UrgencyLevel
            );

            var response = await httpClient.PostAsJsonAsync(BaseUrl, dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task CreateTicketAsync(int ticketId, User ticketCreator, TicketStatusEnum initialStatus,
            TicketCategory category, TicketSubcategory subcategory,
            string subject, string description, DateTime creationTimestamp,
            TicketUrgencyLevelEnum? initialUrgencyLevel = null)
        {

            var ticket = new Ticket(ticketId, ticketCreator, initialStatus, category,
                subcategory, subject, description, creationTimestamp, initialUrgencyLevel);
            await AddTicketAsync(ticket);
        }

        public async Task UpdateStatusAsync(int ticketId, TicketStatusEnum newStatus)
        {
            var updateRequest = new { currentStatus = newStatus };
            var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{ticketId}/status", updateRequest);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateTicketByIdAsync(int id, Ticket ticket)
        {
            var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", ticket);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteTicketByIdAsync(int ticketId)
        {
            var response = await httpClient.DeleteAsync($"{BaseUrl}/{ticketId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateUrgencyLevelAsync(int ticketId, TicketUrgencyLevelEnum newUrgencyLevel)
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
                TicketFilterStatusEnum.OPEN => tickets.Where(t => t.currentStatus == TicketStatusEnum.OPEN),
                TicketFilterStatusEnum.IN_PROGRESS => tickets.Where(t => t.currentStatus == TicketStatusEnum.IN_PROGRESS),
                TicketFilterStatusEnum.RESOLVED => tickets.Where(t => t.currentStatus == TicketStatusEnum.RESOLVED),
                _ => tickets
            };
        }

    }
}