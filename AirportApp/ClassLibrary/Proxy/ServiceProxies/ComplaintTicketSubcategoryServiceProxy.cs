using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interfaces;

namespace AirportApp.ClassLibrary.Proxy.ServiceProxies
{
    public class ComplaintTicketSubcategoryServiceProxy : IComplaintTicketSubcategoryService
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/ticketsubcategory";

        public ComplaintTicketSubcategoryServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<ComplaintTicketSubcategory> GetSubcategoryByIdAsync(int subcategoryId)
        {
            try
            {
                ComplaintTicketSubcategory subcategory = await httpClient.GetFromJsonAsync<ComplaintTicketSubcategory>($"{BaseUrl}/{subcategoryId}");
                if (subcategory == null)
                {
                    throw new KeyNotFoundException($"Ticket subcategory with id {subcategoryId} was not found.");
                }

                return subcategory;
            }
            catch (HttpRequestException httpRequestException) when (httpRequestException.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new KeyNotFoundException($"Ticket subcategory with id {subcategoryId} was not found.");
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Server communication error while retrieving ticket subcategory {subcategoryId}.", httpRequestException);
            }
        }

        public async Task<IEnumerable<ComplaintTicketSubcategory>> GetSubcategoriesByCategoryIdAsync(int categoryId)
        {
            try
            {
                IEnumerable<ComplaintTicketSubcategory> subcategories = await httpClient.GetFromJsonAsync<IEnumerable<ComplaintTicketSubcategory>>($"{BaseUrl}/by-category/{categoryId}");
                return subcategories ?? new List<ComplaintTicketSubcategory>();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to retrieve subcategories for category {categoryId}.", httpRequestException);
            }
        }
    }
}
