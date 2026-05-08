using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Message;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.Src.GoodProxy
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
            // trimitem doar campurile simple, fara obiecte nested
            var dto = new
            {
                id = 0,
                text = elem.Text,
                timestamp = elem.Timestamp == default ? DateTimeOffset.UtcNow : elem.Timestamp,
                chatId = elem.ChatId != 0 ? elem.ChatId : elem.Chat?.Id ?? 0,
                senderUserId = elem.SenderUserId ?? elem.SenderUser?.Id,
                senderEmployeeId = elem.SenderEmployeeId ?? elem.SenderEmployee?.Id
            };

            var response = await httpClient.PostAsJsonAsync(BaseUrl, dto);
            response.EnsureSuccessStatusCode();
            return 0;
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
        {
            var dtos = await httpClient.GetFromJsonAsync<IEnumerable<MessageWithSenderDTO>>($"{BaseUrl}/chat/{chatId}/with-senders");

            return dtos.Select(dto => new Message
            {
                Id = dto.Id,
                Text = dto.Text,
                Timestamp = dto.Timestamp,
                ChatId = dto.ChatId,
                SenderUserId = dto.SenderUserId,
                SenderEmployeeId = dto.SenderEmployeeId
            });
        }

        public async Task<IEnumerable<Message>> GetMessagesSinceAsync(int chatId, int firstMessageId)
            => await httpClient.GetFromJsonAsync<IEnumerable<Message>>($"{BaseUrl}/chat/{chatId}/since/{firstMessageId}");
    }
}