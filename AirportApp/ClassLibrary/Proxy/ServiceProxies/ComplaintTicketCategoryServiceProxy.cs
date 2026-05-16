using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interfaces;

namespace AirportApp.ClassLibrary.Proxy.ServiceProxies
{
    public class ComplaintTicketCategoryServiceProxy : IComplaintTicketCategoryService
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/ticketcategory";

        public ComplaintTicketCategoryServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<ComplaintTicketCategory> GetCategoryByIdAsync(int categoryId)
        {
            try
            {
                ComplaintTicketCategory category = await httpClient.GetFromJsonAsync<ComplaintTicketCategory>($"{BaseUrl}/{categoryId}");
                if (category == null)
                {
                    throw new KeyNotFoundException($"Ticket category with id {categoryId} was not found.");
                }

                return category;
            }
            catch (HttpRequestException httpRequestException) when (httpRequestException.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new KeyNotFoundException($"Ticket category with id {categoryId} was not found.");
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Server communication error while retrieving ticket category {categoryId}.", httpRequestException);
            }
        }

        public async Task<IEnumerable<ComplaintTicketCategory>> GetAllCategoriesAsync()
        {
            try
            {
                IEnumerable<ComplaintTicketCategory> categories = await httpClient.GetFromJsonAsync<IEnumerable<ComplaintTicketCategory>>(BaseUrl);
                return categories ?? new List<ComplaintTicketCategory>();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException("Failed to retrieve all ticket categories.", httpRequestException);
            }
        }
    }
}
