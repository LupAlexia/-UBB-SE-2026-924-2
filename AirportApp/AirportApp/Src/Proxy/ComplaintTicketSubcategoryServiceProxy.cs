using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.Src.Service.Interfaces;

namespace AirportApp.Src.Proxy
{
    public class ComplaintTicketSubcategoryServiceProxy : IComplaintTicketSubcategoryService
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/ticketsubcategory";

        public ComplaintTicketSubcategoryServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<ComplaintTicketSubcategory> GetSubcategoryByIdAsync(int subcategoryId)
        {
            return await httpClient.GetFromJsonAsync<ComplaintTicketSubcategory>($"{BaseUrl}/{subcategoryId}");
        }

        public async Task<IEnumerable<ComplaintTicketSubcategory>> GetSubcategoriesByCategoryIdAsync(int categoryId)
        {
            return await httpClient.GetFromJsonAsync<IEnumerable<ComplaintTicketSubcategory>>($"{BaseUrl}/by-category/{categoryId}");
        }
    }
}