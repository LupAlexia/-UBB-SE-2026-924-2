using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<Message> GetByIdAsync(int id)
            => await httpClient.GetFromJsonAsync<Message>($"{BaseUrl}/{id}");

        public async Task<IEnumerable<Message>> GetAllAsync()
        {
            var dtos = await httpClient.GetFromJsonAsync<IEnumerable<MessageDTO>>($"{BaseUrl}");
            return MapDtosToMessages(dtos);
        }

        public async Task<int> CreateNewEntityAsync(Message elem)
        {
            // trimitem doar campurile simple, fara obiecte nested
            var dto = new
            {
                id = 0,
                text = elem.Text,
                timestamp = elem.Timestamp == default ? DateTimeOffset.UtcNow : elem.Timestamp,
                chatId = elem.Chat.Id != 0 ? elem.Chat.Id : elem.Chat?.Id ?? 0,
                senderId = elem.Sender.RetrieveUniqueDatabaseIdentifierForBot()
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
            try
            {
                var dtos = await httpClient.GetFromJsonAsync<IEnumerable<MessageDTO>>($"{BaseUrl}/chat/{chatId}");
                return MapDtosToMessages(dtos);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetByChatIdAsync failed for chatId {chatId}: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<Message>> GetMessagesSinceAsync(int chatId, int firstMessageId)
        {
            var dtos = await httpClient.GetFromJsonAsync<IEnumerable<MessageDTO>>($"{BaseUrl}/chat/{chatId}/since/{firstMessageId}");
            return MapDtosToMessages(dtos);
        }

        private static IEnumerable<Message> MapDtosToMessages(IEnumerable<MessageDTO> dtos)
        {
            return dtos?.Select(dto => new Message
            {
                Id = dto.MessageId,
                Text = dto.MessageText,
                Timestamp = dto.Timestamp,
                Chat = new Chat { Id = dto.ChatId },
                Sender = dto.SenderId == BotEngineIdentity.CONSTANT_IDENTIFIER_FOR_DEFAULT_BOT_SYSTEM_USER
                    ? new BotEngineIdentity(null)
                    : new User(dto.SenderId, string.Empty, string.Empty)
            }) ?? new List<Message>();
        }
    }
}