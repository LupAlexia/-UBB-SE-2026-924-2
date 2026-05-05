using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Message;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.Src.Proxy
{
    public class MessageRepositoryProxy : IMessageRepository
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/message";

        public MessageRepositoryProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<Message> GetByIdAsync(int id)
            => await httpClient.GetFromJsonAsync<Message>($"{BaseUrl}/{id}");

        public async Task<IEnumerable<Message>> GetAllAsync()
            => await httpClient.GetFromJsonAsync<IEnumerable<Message>>(BaseUrl);

        public async Task<int> CreateNewEntityAsync(Message elem)
        {
            var response = await httpClient.PostAsJsonAsync(BaseUrl, elem);
            response.EnsureSuccessStatusCode();
            var created = await response.Content.ReadFromJsonAsync<Message>();
            return created!.Id;
        }

        public async Task UpdateByIdAsync(int id, Message elem)
        {
            var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", elem);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var response = await httpClient.DeleteAsync($"{BaseUrl}/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<Message>> GetByChatIdAsync(int chatId)
            => await httpClient.GetFromJsonAsync<IEnumerable<Message>>($"{BaseUrl}/chat/{chatId}");

        public async Task<IEnumerable<Message>> GetMessagesSinceAsync(int chatId, int firstMessageId)
            => await httpClient.GetFromJsonAsync<IEnumerable<Message>>($"{BaseUrl}/chat/{chatId}/since/{firstMessageId}");
    }
}