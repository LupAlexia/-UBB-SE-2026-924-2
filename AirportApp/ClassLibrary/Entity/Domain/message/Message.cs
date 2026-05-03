using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Chats;
using AirportApp.ClassLibrary.Entity.Domain.Faq.Bot;
using EmpNamespace = AirportApp.ClassLibrary.Entity.Domain.Employee;

namespace AirportApp.ClassLibrary.Entity.Domain.Message
{
    [Table("Messages")]
    public class Message : IMessage
    {
        // 1. EF Core Auto-Properties
        [Key]
        [Column("Message_Id")]
        public int Id { get; set; }

        [Required]
        [Column("Message_Text", TypeName = "NVARCHAR(MAX)")]
        public string Text { get; set; } = string.Empty;

        [Required]
        [Column("Timestamp")]
        public DateTimeOffset Timestamp { get; set; }


        // 2. Navigation Properties
        // This links the Message to a Chat in the database

        [Required]
        [Column("Chat_Id")]
        public int ChatId { get; set; }

        [ForeignKey(nameof(ChatId))]
        public Chat Chat { get; set; } = null!;

        // Explicit Sender IDs
        [Column("Sender_User_Id")]
        public int? SenderUserId { get; set; }

        [ForeignKey(nameof(SenderUserId))]
        public User? SenderUser { get; set; }

        [Column("Sender_Employee_Id")]
        public int? SenderEmployeeId { get; set; }

        [ForeignKey(nameof(SenderEmployeeId))]
        public EmpNamespace.Employee ? SenderEmployee { get; set; }
        [NotMapped]
        public ISender Sender => (ISender?)SenderUser ?? (ISender?)SenderEmployee!;

        public Message() { }

        public Message(Chat chat, string text, ISender sender)
        {
            Chat = chat;
            ChatId = chat.Id;
            Text = text;
            if (sender is User user) SenderUser = user;
            else if (sender is EmpNamespace.Employee emp) SenderEmployee = emp;
            Timestamp = DateTimeOffset.UtcNow;
        }

        // TODO: This constructor is currently used only for mapping from DB. Without this message_id and timestamp are unsettable.
        // Updated Mapping Constructor
        public Message(int id, ISender sender, Chat chat, string text, DateTimeOffset timestamp)
        {
            Id = id;
            if (sender is User user) SenderUser = user;
            else if (sender is EmpNamespace.Employee emp) SenderEmployee = emp;
            Chat = chat;
            ChatId = chat.Id;
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
