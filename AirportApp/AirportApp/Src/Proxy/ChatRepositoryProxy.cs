using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
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

        public async Task<IEnumerable<Chat>> GetAllAsync()
        {
            return await httpClient.GetFromJsonAsync<IEnumerable<Chat>>(BaseUrl)
                   ?? new List<Chat>();
        }

        public async Task<Chat> GetByIdAsync(int id)
        {
            return await httpClient.GetFromJsonAsync<Chat>($"{BaseUrl}/{id}")
                   ?? throw new KeyNotFoundException($"Chat with id {id} not found.");
        }

        public async Task<int> CreateNewEntityAsync(Chat chat)
        {
            var dto = new CreateChatDTO(chat.User?.Id ?? 0, chat.Status);

            var response = await httpClient.PostAsJsonAsync(BaseUrl, dto);
            response.EnsureSuccessStatusCode();
            var created = await response.Content.ReadFromJsonAsync<Chat>();
            return created!.Id;
        }

        public async Task UpdateByIdAsync(int id, Chat chat)
        {
            var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", chat);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var response = await httpClient.DeleteAsync($"{BaseUrl}/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}