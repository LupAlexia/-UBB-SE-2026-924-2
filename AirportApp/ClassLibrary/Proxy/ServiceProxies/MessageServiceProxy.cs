using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interfaces;

namespace AirportApp.ClassLibrary.Proxy.ServiceProxies
{
    public class MessageServiceProxy : IMessageService
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/message";

        public MessageServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<BotMessage> SendMessageAsync(int chatId, Sender sender, FAQOption selectedOption)
        {
            try
            {
                var request = new SendMessageRequestDTO
                {
                    ChatId = chatId,
                    SenderId = sender.RetrieveUniqueDatabaseIdentifierForBot(),
                    OptionLabel = selectedOption.Label,
                    NextNodeId = selectedOption.NextOption?.NodeId
                };

                HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{BaseUrl}/send", request);
                response.EnsureSuccessStatusCode();

                var replyDTO = await response.Content.ReadFromJsonAsync<BotReplyDTO>();
                if (replyDTO == null)
                {
                    throw new InvalidOperationException("Received null bot reply from server.");
                }

                var botMessage = new BotMessage
                {
                    Id = replyDTO.MessageId,
                    Text = replyDTO.Text,
                    Timestamp = replyDTO.Timestamp,
                    FAQOptions = replyDTO.FAQOptions?.Select(o => MapFAQOptionFromDTO(o)).ToList() ?? new List<FAQOption>()
                };

                return botMessage;
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException("Failed to send message through the service proxy.", httpRequestException);
            }
        }

        private static FAQOption MapFAQOptionFromDTO(FAQOptionDTO dto)
        {
            return new FAQOption
            {
                OptionId = dto.OptionId,
                Label = dto.Label,
                NextOption = dto.NextNodeId.HasValue ? new FAQNode
                {
                    NodeId = dto.NextNodeId.Value
                }
                : null
            };
        }

        public Task<IMessage> GetMessageAsync(int chatId, int messageId)
        {
            throw new NotSupportedException("GetMessageAsync is not available through the service proxy.");
        }

        public async Task<IEnumerable<Message>> GetAllMessagesAsync(int chatId)
        {
            try
            {
                List<MessageDTO> dtos = await httpClient.GetFromJsonAsync<List<MessageDTO>>($"{BaseUrl}/chat/{chatId}");
                if (dtos == null)
                {
                    return new List<Message>();
                }

                return dtos.Select(dto => MapMessageFromDTO(dto)).ToList();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to retrieve messages for chat {chatId}.", httpRequestException);
            }
        }

        public async Task<IEnumerable<Message>> GetAllAsync()
        {
            try
            {
                IEnumerable<Message> messages = await httpClient.GetFromJsonAsync<IEnumerable<Message>>(BaseUrl);
                return messages ?? new List<Message>();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException("Failed to retrieve all messages.", httpRequestException);
            }
        }

        public async Task<Message> GetByIdAsync(int id)
        {
            try
            {
                Message message = await httpClient.GetFromJsonAsync<Message>($"{BaseUrl}/{id}");
                if (message == null)
                {
                    throw new KeyNotFoundException($"Message with id {id} was not found.");
                }

                return message;
            }
            catch (HttpRequestException httpRequestException) when (httpRequestException.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new KeyNotFoundException($"Message with id {id} was not found.");
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Server communication error while retrieving message {id}.", httpRequestException);
            }
        }

        public async Task<int> CreateMessageAsync(int chatId, int senderId, string text, DateTimeOffset timestamp)
        {
            try
            {
                var messageCreationData = new CreateMessageDTO(chatId, text, senderId, timestamp);
                HttpResponseMessage response = await httpClient.PostAsJsonAsync(BaseUrl, messageCreationData);
                response.EnsureSuccessStatusCode();

                string locationHeader = response.Headers.Location?.ToString();
                if (locationHeader != null)
                {
                    string lastSegment = locationHeader.Substring(locationHeader.LastIndexOf('/') + 1);
                    if (int.TryParse(lastSegment, out int createdId))
                    {
                        return createdId;
                    }
                }

                return 0;
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException("Failed to create message through the service proxy.", httpRequestException);
            }
        }

        public async Task UpdateByIdAsync(int id, Message message)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", message);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to update message {id}.", httpRequestException);
            }
        }

        public async Task DeleteByIdAsync(int id)
        {
            try
            {
                HttpResponseMessage response = await httpClient.DeleteAsync($"{BaseUrl}/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to delete message {id}.", httpRequestException);
            }
        }

        public async Task<IEnumerable<Message>> GetByChatIdAsync(int chatId)
        {
            try
            {
                List<MessageDTO> dtos = await httpClient.GetFromJsonAsync<List<MessageDTO>>($"{BaseUrl}/chat/{chatId}");
                if (dtos == null)
                {
                    return new List<Message>();
                }

                return dtos.Select(dto => MapMessageFromDTO(dto)).ToList();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to retrieve messages for chat {chatId}.", httpRequestException);
            }
        }

        public async Task<IEnumerable<Message>> GetMessagesSinceAsync(int chatId, int firstMessageId)
        {
            try
            {
                IEnumerable<Message> messages = await httpClient.GetFromJsonAsync<IEnumerable<Message>>($"{BaseUrl}/chat/{chatId}/since/{firstMessageId}");
                return messages ?? new List<Message>();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to retrieve messages since {firstMessageId} for chat {chatId}.", httpRequestException);
            }
        }

        private static Message MapMessageFromDTO(MessageDTO dto)
        {
            return new Message
            {
                Id = dto.MessageId,
                Text = dto.MessageText ?? string.Empty,
                Timestamp = dto.Timestamp,
                Chat = new Chat { Id = dto.ChatId },
                Sender = new User(dto.SenderId, string.Empty, string.Empty)
            };
        }
    }
}
