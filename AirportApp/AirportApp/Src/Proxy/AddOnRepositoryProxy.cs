using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.Src.Proxy
{
    public class AddOnRepositoryProxy : IAddOnRepository
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/addon";

        public AddOnRepositoryProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<IEnumerable<AddOn>> GetAllAddOnsAsync()
        {
            return await httpClient.GetFromJsonAsync<IEnumerable<AddOn>>(BaseUrl)
                   ?? new List<AddOn>();
        }

        public async Task<IEnumerable<AddOn>> GetAddOnsByIdsAsync(IEnumerable<int> addOnIds)
        {
            try
            {
                var idList = addOnIds.ToList();
                var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/by-ids", idList);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<IEnumerable<AddOn>>()
                       ?? new List<AddOn>();
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException("Failed to retrieve add-ons by IDs from server.", ex);
            }
        }
    }
}
