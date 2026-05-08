using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Faq.Bot;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.Src.Proxy
{
    public class FAQNodeRepositoryProxy : IRepository<int, FAQNode>
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/decisiontree";

        public FAQNodeRepositoryProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<FAQNode> GetByIdAsync(int id)
            => await httpClient.GetFromJsonAsync<FAQNode>($"{BaseUrl}/{id}");

        public async Task<IEnumerable<FAQNode>> GetAllAsync()
            => await httpClient.GetFromJsonAsync<IEnumerable<FAQNode>>(BaseUrl);

        public async Task<int> CreateNewEntityAsync(FAQNode elem)
        {
            var response = await httpClient.PostAsJsonAsync(BaseUrl, elem);
            response.EnsureSuccessStatusCode();
            return elem.faqNodeId;
        }

        public async Task UpdateByIdAsync(int id, FAQNode elem)
        {
            var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", elem);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var response = await httpClient.DeleteAsync($"{BaseUrl}/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}