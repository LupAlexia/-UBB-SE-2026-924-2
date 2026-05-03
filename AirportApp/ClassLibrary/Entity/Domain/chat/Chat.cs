using System;
using System.Collections.Generic;
using System.ComponentModel;
using AirportApp.ClassLibrary.Entity.Domain.Message;
namespace AirportApp.ClassLibrary.Entity.Domain.Chats
{
    public class Chat
    {
        // 1. EF Core convention for Primary Key
        public int Id { get; set; }

        // 2. Navigation Property instead of just int UserId
        public User User { get; set; } = null!; 
        public int UserId { get; set; } // foreign key
        public ChatStatus Status { get; set; }

        //public List<IMessage> Messages { get; set; }

        // 3. ICollection for better EF compatibility
        public ICollection<Message> Messages { get; set; } = new List<Message>();

        // 4. Parameterless constructor for EF Core
        public Chat() { }

        //public Chat(int chatId, int userId, ChatStatus chatStatus)
        //{
        //    Id = chatId;
        //    UserId = userId;
        //    Status = chatStatus;
        //    Messages = new List<IMessage>();
        //}

        public Chat(int id, User user, ChatStatus chatStatus)
        {
            Id = id;
            User = user;
            UserId = user.Id;
            Status = chatStatus;
        }

        public void AddMessage(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message), "message is empty");
            }
            Messages.Add(message);
        }

        public int MessageCount()
        {
            return Messages.Count;
        }

        public void CloseChat()
        {
            Status = ChatStatus.Closed;
        }
    }
}
