using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Chats;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.Src.Proxy
{
    public class ChatRepositoryProxy : IRepository<int, Chat>
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/chat";

        public ChatRepositoryProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<Chat> GetByIdAsync(int id)
            => await httpClient.GetFromJsonAsync<Chat>($"{BaseUrl}/{id}");

        public async Task<IEnumerable<Chat>> GetAllAsync()
            => await httpClient.GetFromJsonAsync<IEnumerable<Chat>>(BaseUrl);

        public async Task<int> CreateNewEntityAsync(Chat elem)
        {
            var response = await httpClient.PostAsJsonAsync(BaseUrl, elem);
            response.EnsureSuccessStatusCode();
            var created = await response.Content.ReadFromJsonAsync<Chat>();
            return created!.Id;
        }

        public async Task UpdateByIdAsync(int id, Chat elem)
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