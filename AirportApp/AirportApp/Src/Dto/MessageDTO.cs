using System;
using System.Collections.Generic;
using AirportApp.Src.Model.Faq.Bot;

namespace AirportApp.Src.Dto
{
        public class MessageDTO
        {
            public int MessageId { get; set; }
            public int ChatId { get; set; }
            public int SenderId { get; set; }
            public string MessageText { get; set; }
            public DateTimeOffset Timestamp { get; set; }
            public IEnumerable<FAQOption> FaqOptions { get; set; }

            public bool IsOutgoing { get; set; }
            public string SenderName { get; set; }

            public MessageDTO()
            {
            }

            public MessageDTO(int messageId, int chatId, int senderId, string senderName, string messageText, DateTimeOffset timestamp, IEnumerable<AirportApp.Src.Model.Faq.Bot.FAQOption> faqOptions)
            {
                MessageId = messageId;
                ChatId = chatId;
                SenderId = senderId;
                MessageText = messageText;
                Timestamp = timestamp;
                FaqOptions = faqOptions;
                SenderName = senderName;
                IsOutgoing = false;
            }
        }
}
