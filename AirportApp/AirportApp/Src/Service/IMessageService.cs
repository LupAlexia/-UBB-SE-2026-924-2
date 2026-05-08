using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Faq.Bot;
using AirportApp.ClassLibrary.Entity.Domain.Message;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Src.Service
{
    public interface IMessageService
    {
        Task<BotMessage> SendMessageAsync(int chatId, Sender sender, FAQOption selectedOption);
        Task<IMessage> GetMessageAsync(int chatId, int messageId);
        Task<IEnumerable<Message>> GetAllMessagesAsync(int chatId);
    }
}
