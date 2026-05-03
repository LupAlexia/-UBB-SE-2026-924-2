using System;
using System.Collections.Generic;
using System.ComponentModel;
using AirportApp.ClassLibrary.Entity.Domain.Message;
namespace AirportApp.ClassLibrary.Entity.Domain.Chats
{
    public class Chat
    {
        public int ChatId { get; set; }
        public int UserId { get; set; }
        public ChatStatus Status { get; set; }

        public List<IMessage> Messages { get; set; }

        public Chat(int chatId, int userId, ChatStatus chatStatus)
        {
            ChatId = chatId;
            UserId = userId;
            Status = chatStatus;
            Messages = new List<IMessage>();
        }

        public void AddMessage(IMessage message)
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
