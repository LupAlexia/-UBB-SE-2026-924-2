using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

// TODO : Maybe merge this with the regular message or pull general data in IMessage and make it abstract class instead of interface
// At this point it is not a contract of functionality but an identity
namespace AirportApp.ClassLibrary.Entity.Domain
{
    [Table("BotMessages")]
    public class BotMessage : IMessage
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

        public List<FAQOption> FAQOptions { get; set; } = new ();

        public BotMessage()
        {
        }

        public BotMessage(int id, Sender sender, Chat chat, string text, List<FAQOption> options, DateTimeOffset timestamp)
        {
            Id = id;
            Sender = sender;
            Chat = chat;
            Text = text;
            FAQOptions = options;
            Timestamp = timestamp;
        }

        public Chat GetChat()
        {
            return Chat;
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

        public IEnumerable<FAQOption> GetNextOptions()
        {
            return FAQOptions;
        }

        public DateTimeOffset GetTimeStamp()
        {
            return Timestamp;
        }

        public class BotMessageBuilder
        {
            private int id;
            private Sender sender = new BotEngineIdentity(null!);
            private Chat chat = null!;
            private string messageText = string.Empty;
            private List<FAQOption> faqOptions = new ();
            private DateTimeOffset timestamp = DateTimeOffset.UtcNow;

            public BotMessageBuilder(Sender sender, Chat chat, int id)
            {
                this.sender = sender;
                this.chat = chat;
                this.id = id;
            }
            public BotMessageBuilder WithTimestamp(DateTimeOffset timestamp)
            {
                this.timestamp = timestamp;
                return this;
            }
            public BotMessageBuilder WithMessage(string setMessage)
            {
                messageText = setMessage;
                return this;
            }
            public BotMessageBuilder WithId(int setId)
            {
                id = setId;
                return this;
            }

            public BotMessageBuilder AddOption(FAQOption addedOption)
            {
                faqOptions.Add(addedOption);
                return this;
            }

            public BotMessageBuilder AddOptions(IEnumerable<FAQOption> setOptions)
            {
                faqOptions.Clear();
                faqOptions.AddRange(setOptions);
                return this;
            }

            public BotMessageBuilder(Sender sender, Chat chat, int id, FAQNode nodeToMessage)
        : this(sender, chat, id)
            {
                this.messageText = nodeToMessage.QuestionText;
                this.faqOptions = nodeToMessage.Options.ToList();
            }

            public BotMessage Build()
            {
                return new BotMessage(id, sender, chat, messageText, faqOptions, timestamp);
            }
        }
    }
}
