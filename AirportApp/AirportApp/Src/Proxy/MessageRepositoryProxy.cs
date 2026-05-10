using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
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
            var messageTransferObjectList = await httpClient.GetFromJsonAsync<IEnumerable<MessageDTO>>(BaseUrl)
                       ?? new List<MessageDTO>();

            return messageTransferObjectList.Select(MapToMessage).ToList();
        }

        public async Task<Message> GetByIdAsync(int id)
        {
            var messageTransferObject = await httpClient.GetFromJsonAsync<MessageDTO>($"{BaseUrl}/{id}")
                      ?? throw new KeyNotFoundException($"Message with id {id} not found.");

            return MapToMessage(messageTransferObject);
        }

        public async Task<int> CreateNewEntityAsync(Message message)
        {
            var messageCreationData = new
            {
                text = message.Text,
                timestamp = message.Timestamp == default ? DateTimeOffset.UtcNow : message.Timestamp,
                chatId = message.Chat.Id != 0 ? message.Chat.Id : message.Chat?.Id ?? 0,
                senderId = message.Sender.RetrieveUniqueDatabaseIdentifierForBot()
            };

            var response = await httpClient.PostAsJsonAsync(BaseUrl, messageCreationData);
            response.EnsureSuccessStatusCode();

            var location = response.Headers.Location;
            if (location != null)
            {
                var lastSegment = location.Segments.LastOrDefault()?.Trim('/');
                if (int.TryParse(lastSegment, out var createdId))
                {
                    return createdId;
                }
            }

            return 0;
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
            var messageTransferObjectList = await httpClient.GetFromJsonAsync<IEnumerable<MessageDTO>>($"{BaseUrl}/chat/{chatId}")
                       ?? new List<MessageDTO>();

            return messageTransferObjectList.Select(MapToMessage).ToList();
        }

        public async Task<IEnumerable<Message>> GetMessagesSinceAsync(int chatId, int firstMessageId)
        {
            var messageTransferObjectList = await httpClient.GetFromJsonAsync<IEnumerable<MessageDTO>>($"{BaseUrl}/chat/{chatId}/since/{firstMessageId}")
                       ?? new List<MessageDTO>();

            return messageTransferObjectList.Select(MapToMessage).ToList();
        }

        private static Message MapToMessage(MessageDTO messageTransferObject)
        {
            var sender = messageTransferObject.Sender as Sender ?? (messageTransferObject.SenderId == BotEngineIdentity.CONSTANT_IDENTIFIER_FOR_DEFAULT_BOT_SYSTEM_USER
                ? new BotEngineIdentity(null)
                : new User { Id = messageTransferObject.SenderId });

            return new Message(messageTransferObject.MessageId, sender, new Chat { Id = messageTransferObject.ChatId }, messageTransferObject.MessageText, messageTransferObject.Timestamp);
        }
    }
}