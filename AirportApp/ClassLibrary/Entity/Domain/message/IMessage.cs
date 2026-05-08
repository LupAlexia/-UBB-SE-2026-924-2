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
        int Id { get; }
        string Text { get; }
        DateTimeOffset Timestamp { get; }
        ISender GetSender();

        Chat GetChat();
        IEnumerable<FAQOption> GetNextOptions();
    }
}
