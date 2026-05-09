using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.Src.Service.Interfaces;

namespace AirportApp.Src.Proxy
{
    public class ComplaintTicketCategoryServiceProxy : IComplaintTicketCategoryService
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/ticketcategory";

        public ComplaintTicketCategoryServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<ComplaintTicketCategory> GetCategoryByIdAsync(int categoryId)
        {
            return await httpClient.GetFromJsonAsync<ComplaintTicketCategory>($"{BaseUrl}/{categoryId}");
        }

        public async Task<IEnumerable<ComplaintTicketCategory>> GetAllCategoriesAsync()
        {
            return await httpClient.GetFromJsonAsync<IEnumerable<ComplaintTicketCategory>>(BaseUrl);
        }
    }
}