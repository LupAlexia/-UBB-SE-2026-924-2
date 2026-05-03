using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Chats;
using AirportApp.ClassLibrary.Entity.Domain.Faq.Bot;

namespace AirportApp.ClassLibrary.Entity.Domain.Message
{
    public class Message : IMessage
    {
        private int message_id;
        private ISender sender;
        private Chat chat;
        private DateTimeOffset timestamp;
        private string messageText;

        public Message(ISender sender, Chat chat, string messageText)
        {
            this.sender = sender;
            this.chat = chat;
            this.messageText = messageText;
            this.timestamp = DateTimeOffset.UtcNow;
        }

        // TODO: This constructor is currently used only for mapping from DB. Without this message_id and timestamp are unsettable.
        public Message(int id, ISender sender, Chat chat, string messageText, DateTimeOffset timestamp)
        {
            this.message_id = id;
            this.sender = sender;
            this.chat = chat;
            this.messageText = messageText;
            this.timestamp = timestamp;
        }

        public Chat GetChat()
        {
            return this.chat;
        }

        // Interface functionality
        public string GetMessage()
        {
            return this.messageText;
        }

        public ISender GetSender()
        {
            return sender;
        }

        public int GetId()
        {
            return this.message_id;
        }

        IEnumerable<FAQOption> IMessage.GetNextOptions()
        {
            return new List<FAQOption>();
        }

        DateTimeOffset IMessage.GetTimeStamp()
        {
            return timestamp;
        }

        Chat IMessage.GetChat()
        {
            return this.chat;
        }
    }
}
