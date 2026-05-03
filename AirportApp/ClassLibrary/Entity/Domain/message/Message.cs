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
        public int ChatId { get; set; }
        public Chat Chat { get; set; } = null!;
        public int SenderId { get; set; }

        public Message() { }

        public Message(Chat chat, string text, int senderId)
        {
            Chat = chat;
            ChatId = chat.Id;
            Text = text;
            SenderId = senderId;
            Timestamp = DateTimeOffset.UtcNow;
        }

        // TODO: This constructor is currently used only for mapping from DB. Without this message_id and timestamp are unsettable.
        // Updated Mapping Constructor
        public Message(int id, int senderId, Chat chat, string text, DateTimeOffset timestamp)
        {
            Id = id;           
            SenderId = senderId;
            Chat = chat;
            ChatId = chat.Id;
            Text = text;        
            Timestamp = timestamp; 
        }

        public Chat GetChat()
        {
            return this.Chat;
        }

        // Interface functionality
        public string GetMessage()
        {
            return Text;
        }

        public ISender GetSender()
        {
            if (SenderId == BotEngine.CONSTANT_IDENTIFIER_FOR_DEFAULT_BOT_SYSTEM_USER)
            {
                // You might need to inject the strategy here or use a static reference
                return new BotEngine(null!); // 'null!' assumes the strategy is provided externally
            }

            // Return a User object based on the SenderId
            return new User(SenderId, "Unknown User", "unknown@email.com");
        }

        public int GetId()
        {
            return Id;
        }

        IEnumerable<FAQOption> IMessage.GetNextOptions()
        {
            return new List<FAQOption>();
        }

       
    }
}
