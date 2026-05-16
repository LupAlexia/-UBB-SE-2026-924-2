using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interfaces;

namespace AirportApp.ClassLibrary.Proxy.ServiceProxies
{
    public class ChatServiceProxy : IChatService
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/chat";

        public ChatServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<Chat> OpenChatAsync(User userToOpenChatFor)
        {
            try
            {
                var chatCreationData = new CreateChatDTO(userToOpenChatFor.Id, ChatStatus.Active);
                HttpResponseMessage response = await httpClient.PostAsJsonAsync(BaseUrl, chatCreationData);
                response.EnsureSuccessStatusCode();

                Chat createdChat = await response.Content.ReadFromJsonAsync<Chat>();
                return createdChat;
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException("Failed to open chat through the service proxy.", httpRequestException);
            }
        }

        public async Task CloseChatAsync(int chatId)
        {
            try
            {
                HttpResponseMessage response = await httpClient.DeleteAsync($"{BaseUrl}/{chatId}");
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to close chat {chatId}.", httpRequestException);
            }
        }

        public async Task<IEnumerable<Chat>> GetAllChatsAsync()
        {
            try
            {
                IEnumerable<Chat> chats = await httpClient.GetFromJsonAsync<IEnumerable<Chat>>(BaseUrl);
                return chats ?? new List<Chat>();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException("Failed to retrieve all chats.", httpRequestException);
            }
        }

        public async Task<Chat> GetChatByIdAsync(int id)
        {
            try
            {
                Chat chat = await httpClient.GetFromJsonAsync<Chat>($"{BaseUrl}/{id}");
                if (chat == null)
                {
                    throw new KeyNotFoundException($"Chat with id {id} was not found.");
                }

                return chat;
            }
            catch (HttpRequestException httpRequestException) when (httpRequestException.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new KeyNotFoundException($"Chat with id {id} was not found.");
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Server communication error while retrieving chat {id}.", httpRequestException);
            }
        }

        public async Task UpdateChatAsync(int id, Chat chat)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", chat);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to update chat {id}.", httpRequestException);
            }
        }
    }
}
