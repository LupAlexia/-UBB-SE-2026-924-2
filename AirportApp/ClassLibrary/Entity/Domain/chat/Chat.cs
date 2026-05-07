using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MessageEntity = AirportApp.ClassLibrary.Entity.Domain.Message.Message;

namespace AirportApp.ClassLibrary.Entity.Domain.Chats
{
    [Table("Chats")]
    public class Chat
    {
        // 1. EF Core convention for Primary Key
        [Key]
        [Column("Chat_Id")]
        public int Id { get; set; }

        // 2. Navigation Property instead of just int UserId
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
        [Required]
        [Column("User_Id")]
        public int UserId { get; set; }
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
            UserId = user.UserId;
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
