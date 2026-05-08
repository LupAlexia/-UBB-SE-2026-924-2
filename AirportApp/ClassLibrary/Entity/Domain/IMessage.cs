using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportApp.ClassLibrary.Entity.Domain
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
