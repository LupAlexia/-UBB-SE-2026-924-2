using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Chats;
using AirportApp.ClassLibrary.Entity.Domain.Faq.Bot;

namespace AirportApp.ClassLibrary.Entity.Domain.Message
{
    public interface IMessage
    {
        //public string GetMessage();
        //public IEnumerable<FAQOption> GetNextOptions();

        //public ISender GetSender();

        //public int GetId();

        //public Chat GetChat();

        //public DateTimeOffset GetTimeStamp();
        int Id { get; }
        string Text { get; }
        DateTimeOffset Timestamp { get; }
        int ChatId { get; }
        Chat Chat { get; }
        int SenderId { get; }

        ISender GetSender();
        IEnumerable<FAQOption> GetNextOptions();
    }
}
