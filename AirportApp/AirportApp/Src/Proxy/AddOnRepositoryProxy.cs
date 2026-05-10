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
            var addOnTransferObjectList =await httpClient.GetFromJsonAsync<IEnumerable<AirportApp.ClassLibrary.Entity.Dto.AddOnDTO>>(BaseUrl);
            if (addOnTransferObjectList == null)
            {
                return new List<AddOn>();
            }

            return addOnTransferObjectList.Select(addOnTransferObject => new AddOn(addOnTransferObject.id, addOnTransferObject.name, addOnTransferObject.basePrice)).ToList();
        }

        public async Task<IEnumerable<AddOn>> GetAddOnsByIdsAsync(IEnumerable<int> addOnIds)
        {
            try
            {
                var idList = addOnIds.ToList();
                var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/by-ids", idList);
                response.EnsureSuccessStatusCode();
                var addOnTransferObjectList =await response.Content.ReadFromJsonAsync<IEnumerable<AirportApp.ClassLibrary.Entity.Dto.AddOnDTO>>();
                if (addOnTransferObjectList == null)
                {
                    return new List<AddOn>();
                }

                return addOnTransferObjectList.Select(addOnTransferObject => new AddOn(addOnTransferObject.id, addOnTransferObject.name, addOnTransferObject.basePrice)).ToList();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException("Failed to retrieve add-ons by IDs from server.", httpRequestException);
            }
        }
    }
}