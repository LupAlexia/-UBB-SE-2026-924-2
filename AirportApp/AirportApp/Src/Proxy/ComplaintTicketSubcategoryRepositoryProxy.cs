using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.Src.Proxy
{
    public class ComplaintTicketSubcategoryRepositoryProxy : ITicketSubcategoryRepository
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/ticketsubcategory";

        public ComplaintTicketSubcategoryRepositoryProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<ComplaintTicketSubcategory>> GetAllAsync()
        {
            return await httpClient.GetFromJsonAsync<IEnumerable<ComplaintTicketSubcategory>>(BaseUrl)
                   ?? new List<ComplaintTicketSubcategory>();
        }

        public async Task<ComplaintTicketSubcategory> GetByIdAsync(int subcategoryId)
        {
            return await httpClient.GetFromJsonAsync<ComplaintTicketSubcategory>($"{BaseUrl}/{subcategoryId}")
                   ?? throw new KeyNotFoundException($"Subcategory with id {subcategoryId} not found.");
        }

        public async Task<IEnumerable<ComplaintTicketSubcategory>> GetByCategoryIdAsync(int categoryId)
        {
            return await httpClient.GetFromJsonAsync<IEnumerable<ComplaintTicketSubcategory>>($"{BaseUrl}/by-category/{categoryId}")
                   ?? new List<ComplaintTicketSubcategory>();
        }
    }
}