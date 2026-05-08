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
        [Key]
        [Column("Message_Id")]
        public int Id { get; set; }

        [Required]
        [Column("Message_Text", TypeName = "NVARCHAR(MAX)")]
        public string Text { get; set; } = string.Empty;

        [Required]
        [Column("Timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        public Chat Chat { get; set; } = null!;
        public Sender Sender { get; set; } = null!;

        public Message()
        {
        }

        public Message(Sender sender, Chat chat, string messageText)
        {
            this.Chat = chat;
            this.Text = messageText;
            this.Timestamp = DateTimeOffset.UtcNow;
            this.Sender = sender;
        }

        public Message(Chat chat, string text, Sender sender)
        {
            Chat = chat;
            Text = text;
            // if (sender is User user)
            // {
            //    SenderUser = user;
            // }
            // else if (sender is EmpNamespace.Employee emp)
            // {
            //    SenderEmployee = emp;
            // }
            Sender = sender;
            Timestamp = DateTimeOffset.UtcNow;
        }

        // TODO: This constructor is currently used only for mapping from DB. Without this message_id and timestamp are unsettable.
        // Updated Mapping Constructor
        public Message(int id, Sender sender, Chat chat, string text, DateTimeOffset timestamp)
        {
            Id = id;
            // if (sender is User user)
            // {
            //    SenderUser = user;
            // }
            // else if (sender is EmpNamespace.Employee emp)
            // {
            //    SenderEmployee = emp;
            // }
            Sender = sender;
            Chat = chat;
            Text = text;
            Timestamp = timestamp;
        }
        public string GetMessage()
        {
            return Text;
        }

        public ISender GetSender() => Sender!;

        public int GetId()
        {
            return Id;
        }

        public Chat GetChat() => Chat!;

        IEnumerable<FAQOption> IMessage.GetNextOptions()
        {
            return new List<FAQOption>();
        }

        public DateTimeOffset GetTimeStamp()
        {
            return Timestamp;
        }
    }
}
