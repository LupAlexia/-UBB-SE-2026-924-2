using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.Src.Proxy
{
    public class DecisionTreeRepositoryProxy : IRepository<int, FAQNode>
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/decisiontree";

        public DecisionTreeRepositoryProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<FAQNode>> GetAllAsync()
        {
            return await httpClient.GetFromJsonAsync<IEnumerable<FAQNode>>(BaseUrl)
                   ?? new List<FAQNode>();
        }

        public async Task<FAQNode> GetByIdAsync(int id)
        {
            var node = await httpClient.GetFromJsonAsync<FAQNode>($"{BaseUrl}/{id}")
                      ?? throw new KeyNotFoundException($"FAQNode with id {id} was not found.");
            return node;
        }

        public async Task<int> CreateNewEntityAsync(FAQNode node)
        {
            var response = await httpClient.PostAsJsonAsync(BaseUrl, node);
            response.EnsureSuccessStatusCode();
            var created = await response.Content.ReadFromJsonAsync<FAQNode>();
            return created?.NodeId ?? 0;
        }

        public async Task UpdateByIdAsync(int id, FAQNode node)
        {
            var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", node);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var response = await httpClient.DeleteAsync($"{BaseUrl}/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}