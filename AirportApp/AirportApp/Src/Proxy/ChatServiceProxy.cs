using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Domain.Chats;
using AirportApp.Src.Service;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace AirportApp.Src.Proxy
{
    public class ChatServiceProxy : IChatService
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/chat";

        public ChatServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<Chat> OpenChatAsync(int userId)
        {
            // construim chat-ul si il trimitem la API
            var newChat = new Chat(0, new User { Id = userId }, ChatStatus.Active);
            var response = await httpClient.PostAsJsonAsync(BaseUrl, newChat);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Chat>();
        }

        public async Task CloseChatAsync(int chatId)
        {
            // get chat, close, put
            var chat = await httpClient.GetFromJsonAsync<Chat>($"{BaseUrl}/{chatId}");
            chat.CloseChat();
            var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{chatId}", chat);
            response.EnsureSuccessStatusCode();
        }
    }
}