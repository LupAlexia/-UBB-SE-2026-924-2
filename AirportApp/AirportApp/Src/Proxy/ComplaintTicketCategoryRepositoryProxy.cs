using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.Src.Proxy
{
    public class ComplaintTicketCategoryRepositoryProxy : ITicketCategoryRepository
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/ticketcategory";

        public ComplaintTicketCategoryRepositoryProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<ComplaintTicketCategory>> GetAllAsync()
        {
            return await httpClient.GetFromJsonAsync<IEnumerable<ComplaintTicketCategory>>(BaseUrl)
                   ?? new List<ComplaintTicketCategory>();
        }

        public async Task<ComplaintTicketCategory> GetByIdAsync(int categoryId)
        {
            return await httpClient.GetFromJsonAsync<ComplaintTicketCategory>($"{BaseUrl}/{categoryId}")
                   ?? throw new KeyNotFoundException($"Category with id {categoryId} not found.");
        }
    }
}