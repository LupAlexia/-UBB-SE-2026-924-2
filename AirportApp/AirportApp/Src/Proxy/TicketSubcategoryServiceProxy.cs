using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using AirportApp.Src.Service.Interfaces;

namespace AirportApp.Src.Proxy
{
    public class TicketSubcategoryServiceProxy : ITicketSubcategoryService
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/ticketsubcategory";

        public TicketSubcategoryServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<TicketSubcategory> GetSubcategoryByIdAsync(int subcategoryId)
        {
            return await httpClient.GetFromJsonAsync<TicketSubcategory>($"{BaseUrl}/{subcategoryId}");
        }

        public async Task<IEnumerable<TicketSubcategory>> GetSubcategoriesByCategoryIdAsync(int categoryId)
        {
            return await httpClient.GetFromJsonAsync<IEnumerable<TicketSubcategory>>($"{BaseUrl}/by-category/{categoryId}");
        }
    }
}