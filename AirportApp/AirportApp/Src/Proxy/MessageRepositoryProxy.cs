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
    public class MessageRepositoryProxy : IMessageRepository
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/message";

        public MessageRepositoryProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<Message>> GetAllAsync()
        {
            return await httpClient.GetFromJsonAsync<IEnumerable<Message>>(BaseUrl)
                   ?? new List<Message>();
        }

        public async Task<Message> GetByIdAsync(int id)
        {
            return await httpClient.GetFromJsonAsync<Message>($"{BaseUrl}/{id}")
                   ?? throw new KeyNotFoundException($"Message with id {id} not found.");
        }

        public async Task<int> CreateNewEntityAsync(Message message)
        {
            var dto = new
            {
                text = message.Text,
                timestamp = message.Timestamp == default ? DateTimeOffset.UtcNow : message.Timestamp,
                chatId = message.ChatId != 0 ? message.ChatId : message.Chat?.Id ?? 0,
                senderUserId = message.SenderUserId ?? message.SenderUser?.Id,
                senderEmployeeId = message.SenderEmployeeId ?? message.SenderEmployee?.Id
            };

            var response = await httpClient.PostAsJsonAsync(BaseUrl, dto);
            response.EnsureSuccessStatusCode();
            var created = await response.Content.ReadFromJsonAsync<Message>();
            return created?.Id ?? 0;
        }

        public async Task UpdateByIdAsync(int id, Message message)
        {
            var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", message);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var response = await httpClient.DeleteAsync($"{BaseUrl}/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<Message>> GetByChatIdAsync(int chatId)
        {
            return await httpClient.GetFromJsonAsync<IEnumerable<Message>>($"{BaseUrl}/chat/{chatId}")
                   ?? new List<Message>();
        }

        public async Task<IEnumerable<Message>> GetMessagesSinceAsync(int chatId, int firstMessageId)
        {
            return await httpClient.GetFromJsonAsync<IEnumerable<Message>>($"{BaseUrl}/chat/{chatId}/since/{firstMessageId}")
                   ?? new List<Message>();
        }
    }
}