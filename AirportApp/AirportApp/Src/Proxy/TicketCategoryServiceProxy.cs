using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using AirportApp.Src.Service.Interfaces;

namespace AirportApp.Src.Proxy
{
    public class TicketCategoryServiceProxy : ITicketCategoryService
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/ticketcategory";

        public TicketCategoryServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<TicketCategory> GetCategoryByIdAsync(int categoryId)
        {
            return await httpClient.GetFromJsonAsync<TicketCategory>($"{BaseUrl}/{categoryId}");
        }

        public async Task<IEnumerable<TicketCategory>> GetAllCategoriesAsync()
        {
            return await httpClient.GetFromJsonAsync<IEnumerable<TicketCategory>>(BaseUrl);
        }
    }
}