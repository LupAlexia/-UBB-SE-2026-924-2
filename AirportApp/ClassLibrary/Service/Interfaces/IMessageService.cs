using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interfaces
{
    public interface IMessageService
    {
        Task<BotMessage> SendMessageAsync(int chatId, Sender sender, FAQOption selectedOption);
        Task<IMessage> GetMessageAsync(int chatId, int messageId);
        Task<IEnumerable<Message>> GetAllMessagesAsync(int chatId);
        Task<IEnumerable<Message>> GetAllAsync();
        Task<Message> GetByIdAsync(int id);
        Task<int> CreateMessageAsync(int chatId, int senderId, string text, DateTimeOffset timestamp);
        Task UpdateByIdAsync(int id, Message message);
        Task DeleteByIdAsync(int id);
        Task<IEnumerable<Message>> GetByChatIdAsync(int chatId);
        Task<IEnumerable<Message>> GetMessagesSinceAsync(int chatId, int firstMessageId);
    }
}