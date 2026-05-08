using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Faq.Bot;
using AirportApp.ClassLibrary.Entity.Domain.Message;

namespace AirportApp.Src.Service.Interfaces
{
    public interface IMessageService
    {
        Task<BotMessage> SendMessageAsync(int chatId, ISender sender, FAQOption selectedOption);
        Task<IMessage> GetMessageAsync(int chatId, int messageId);
        Task<IEnumerable<Message>> GetAllMessagesAsync(int chatId);
    }
}
