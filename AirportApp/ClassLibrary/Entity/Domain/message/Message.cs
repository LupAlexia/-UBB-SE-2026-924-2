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
        // 1. EF Core Auto-Properties
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTimeOffset Timestamp { get; set; }


        // 2. Navigation Properties
        // This links the Message to a Chat in the database
       
        public Chat Chat { get; set; } = null!;
        public ISender Sender { get; set; } = null!;

        public Message() { }

        public Message(Chat chat, string text, ISender sender)
        {
            Chat = chat;
            Text = text;
            Sender = sender;
            Timestamp = DateTimeOffset.UtcNow;
        }

        // TODO: This constructor is currently used only for mapping from DB. Without this message_id and timestamp are unsettable.
        // Updated Mapping Constructor
        public Message(int id, ISender sender, Chat chat, string text, DateTimeOffset timestamp)
        {
            Id = id;           
            Sender = sender;
            Chat = chat;
            Text = text;        
            Timestamp = timestamp; 
        }
        public string GetMessage()
        {
            return Text;
        }

        public ISender GetSender() => Sender;

        public int GetId()
        {
            return Id;
        }

        public Chat GetChat() => Chat;

        IEnumerable<FAQOption> IMessage.GetNextOptions()
        {
            return new List<FAQOption>();
        }

       
    }
}
