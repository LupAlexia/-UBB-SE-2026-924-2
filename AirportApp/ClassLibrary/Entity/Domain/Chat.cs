using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MessageEntity = AirportApp.ClassLibrary.Entity.Domain.Message;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    [Table("Chats")]
    public class Chat
    {
        [Key]
        [Column("Chat_Id")]
        public int Id { get; set; }

        public User User { get; set; }

        [Required]
        [Column("Chat_Status")]
        public ChatStatus Status { get; set; }

        // public List<IMessage> Messages { get; set; }
        // 3. ICollection for better EF compatibility
        public ICollection<MessageEntity> Messages { get; set; } = new List<MessageEntity>();

        // 4. Parameterless constructor for EF Core
        public Chat()
        {
        }

        public Chat(int id, User user, ChatStatus chatStatus)
        {
            Id = id;
            User = user;
            Status = chatStatus;
        }

        public void AddMessage(MessageEntity message)
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
