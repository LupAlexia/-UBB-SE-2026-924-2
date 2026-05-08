using System;
using System.Collections.Generic;
using AirportApp.ClassLibrary.Entity.Domain.Faq.Bot;
using AirportApp.ClassLibrary.Entity.Domain.Message;

namespace AirportApp.ClassLibrary.Entity.Dto
{
    public class MessageDTO
        {
            public int MessageId { get; set; }
            public int ChatId { get; set; }
            public ISender Sender { get; set; }
            public string MessageText { get; set; }
            public DateTimeOffset Timestamp { get; set; }
            public IEnumerable<FAQOption> FaqOptions { get; set; }

            public bool IsOutgoing { get; set; }

            public MessageDTO()
            {
            }

            public MessageDTO(int messageId, int chatId, ISender sender, string senderName, string messageText, DateTimeOffset timestamp, IEnumerable<FAQOption> faqOptions)
            {
                MessageId = messageId;
                ChatId = chatId;
                Sender = sender;
                MessageText = messageText;
                Timestamp = timestamp;
                FaqOptions = faqOptions;
                IsOutgoing = false;
            }
        }
}
