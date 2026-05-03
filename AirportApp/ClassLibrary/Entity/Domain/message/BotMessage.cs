using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using AirportApp.ClassLibrary.Entity.Domain.Chats;
using AirportApp.ClassLibrary.Entity.Domain.Faq.Bot;

// TODO : Maybe merge this with the regular message or pull general data in IMessage and make it abstract class instead of interface
// At this point it is not a contract of functionality but an identity
namespace AirportApp.ClassLibrary.Entity.Domain.Message
{
    [Table("BotMessages")]
    public class BotMessage : IMessage
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

        [Required]
        [Column("Chat_Id")]
        public int ChatId { get; set; }

        [ForeignKey(nameof(ChatId))]

        public Chat Chat { get; set; } = null!;

        [Required]
        [Column("Bot_Id")]
        public int BotId { get; set; }

        [NotMapped]
        public ISender Sender { get; set; } = null!;

       
        public List<FAQOption> FAQOptions { get; set; } = new();

        public BotMessage() { }

        //private BotMessage(int messageId, ISender sender, Chat chat, string messageText, IEnumerable<FAQOption> options)
        //{
        //    this.messageId = messageId;
        //    this.sender = sender;
        //    this.chat = chat;
        //    this.messageText = messageText;
        //    this.timestamp = DateTimeOffset.UtcNow;
        //    this.faqOptions = options;
        //}
        public BotMessage(int id, ISender sender, Chat chat, string text, List<FAQOption> options, DateTimeOffset timestamp)
        {
            Id = id;
            Sender = sender;
            BotId = sender.RetrieveUniqueDatabaseIdentifierForBot();
            Chat = chat;
            ChatId = chat.Id;
            Text = text;
            FAQOptions = options;
            Timestamp = timestamp;
        }

        //private BotMessage(int messageId, ISender sender, Chat chat, string messageText, IEnumerable<FAQOption> options, DateTimeOffset timestamp) : this(messageId, sender, chat, messageText, options)
        //{
        //    this.Timestamp = timestamp;
        //}
       

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
        //public class BotMessageBuilder
        //{
        //    private int messageId;
        //    private int senderId;
        //    private Chat chat = null!;
        //    private string messageText = string.Empty;
        //    private List<FAQOption> faqOptions = new();
        //    private DateTimeOffset timestamp = DateTimeOffset.UtcNow;

        //    public BotMessageBuilder(int senderId, Chat chat, int messageId, FAQNode nodeToMessage)
        //        : this(senderId, chat, messageId)
        //    {
        //        this.messageText = nodeToMessage.questionText;
        //        this.faqOptions = nodeToMessage.options.ToList();
        //    }

        //    public BotMessageBuilder(ISender sender, Chat chat, int messageId)
        //    {
        //        this.messageText = string.Empty;
        //        this.messageId = messageId;
        //        this.sender = sender;
        //        this.chat = chat;
        //        this.faqOptions = new List<FAQOption>();
        //        this.timestamp = DateTimeOffset.UtcNow;
        //    }

        //    public BotMessageBuilder WithTimestamp(DateTimeOffset timestamp)
        //    {
        //        this.timestamp = timestamp;
        //        return this;
        //    }

        //    public BotMessageBuilder WithMessage(string setMessage)
        //    {
        //        this.messageText = setMessage;
        //        return this;
        //    }

        //    public BotMessageBuilder WithId(int setId)
        //    {
        //        this.messageId = setId;
        //        return this;
        //    }
        public class BotMessageBuilder
        {
            // These private fields hold the state during the building process
            private int _id;
            private ISender _sender = new BotEngineIdentity(null!);
            private Chat _chat = null!;
            private string _messageText = string.Empty;
            private List<FAQOption> _faqOptions = new();
            private DateTimeOffset _timestamp = DateTimeOffset.UtcNow;

            // Primary constructor: expects the identifiers EF needs
            public BotMessageBuilder(ISender sender, Chat chat, int id)
            {
                _sender = sender;
                _chat = chat;
                _id = id;
            }

           
            // Fluent method: Set Timestamp
            public BotMessageBuilder WithTimestamp(DateTimeOffset timestamp)
            {
                _timestamp = timestamp;
                return this;
            }

            // Fluent method: Set Message text
            public BotMessageBuilder WithMessage(string setMessage)
            {
                _messageText = setMessage;
                return this;
            }

            // Fluent method: Set ID
            public BotMessageBuilder WithId(int setId)
            {
                _id = setId;
                return this;
            }

            public BotMessageBuilder AddOption(FAQOption addedOption)
            {
                _faqOptions.Add(addedOption);
                return this;
            }

            public BotMessageBuilder AddOptions(IEnumerable<FAQOption> setOptions)
            {
                _faqOptions.Clear();
                _faqOptions.AddRange(setOptions);
                return this;
            }

            public BotMessageBuilder(ISender sender, Chat chat, int id, FAQNode nodeToMessage)
        : this(sender, chat, id) // Calls your existing 3-argument constructor
            {
                // Automatically set the text and options from the node
                this._messageText = nodeToMessage.QuestionText; // Ensure property name matches FAQNode
                this._faqOptions = nodeToMessage.Options.ToList();
            }

            public BotMessage Build()
            {
                return new BotMessage(_id, _sender, _chat, _messageText, _faqOptions, _timestamp);
            }
        }
    }
}
