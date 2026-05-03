using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using AirportApp.ClassLibrary.Entity.Domain.Chats;
using AirportApp.ClassLibrary.Entity.Domain.Faq.Bot;

// TODO : Maybe merge this with the regular message or pull general data in IMessage and make it abstract class instead of interface
// At this point it is not a contract of functionality but an identity
namespace AirportApp.ClassLibrary.Entity.Domain.Message
{
    public class BotMessage : IMessage
    {
        // 1. EF Core Auto-Properties
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTimeOffset Timestamp { get; set; }

        // 2. Navigation Properties
        public int ChatId { get; set; }
        public Chat Chat { get; set; } = null!;

        public int SenderId { get; set; }

        //public int SenderId { get; set; }
        //private int messageId;
        //private ISender sender;
        //private Chat chat;
        //private DateTimeOffset timestamp;
        //private string messageText;
        // private IEnumerable<FAQOption> faqOptions;
        public List<FAQOption> FAQOptions { get; set; } = new List<FAQOption>();

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
        public BotMessage(int id, int senderId, Chat chat, string text, List<FAQOption> options, DateTimeOffset timestamp)
        {
            Id = id;
            SenderId = senderId;
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
        private BotMessage(int id, ISender sender, Chat chat, string text, IEnumerable<FAQOption> options, DateTimeOffset timestamp)
    : this(id, sender.RetrieveUniqueDatabaseIdentifierForBot(), chat, text, options.ToList(), timestamp)
        {
            // No code needed here because the primary constructor handles all assignments!
        }

        public Chat GetChat()
        {
            return Chat;
        }

        public string GetMessage()
        {
            return Text;
        }

        public ISender GetSender()
        {
            return new BotEngine(null!);
        }

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
            private int _senderId;
            private Chat _chat = null!;
            private string _messageText = string.Empty;
            private List<FAQOption> _faqOptions = new();
            private DateTimeOffset _timestamp = DateTimeOffset.UtcNow;

            // Primary constructor: expects the identifiers EF needs
            public BotMessageBuilder(int senderId, Chat chat, int id)
            {
                _senderId = senderId;
                _chat = chat;
                _id = id;
            }

            // Overload: keeps compatibility for when you have an ISender object
            public BotMessageBuilder(ISender sender, Chat chat, int id)
                : this(sender.RetrieveUniqueDatabaseIdentifierForBot(), chat, id) { }

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

            public BotMessage Build()
            {
                return new BotMessage(_id, _senderId, _chat, _messageText, _faqOptions, _timestamp);
            }
        }
    }
}
