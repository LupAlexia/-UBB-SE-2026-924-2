using System;
using System.Collections.Generic;

namespace AirportApp.ClassLibrary.Entity.Dto
{
    public class SendMessageRequestDTO
    {
        public int ChatId { get; set; }
        public int SenderId { get; set; }
        public string OptionLabel { get; set; } = string.Empty;
        public int? NextNodeId { get; set; }
    }

    public class FAQNodeDTO
    {
        public int NodeId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public List<FAQOptionDTO> Options { get; set; } = new ();
        public bool IsFinalAnswer { get; set; }
    }

    public class FAQOptionDTO
    {
        public int OptionId { get; set; }
        public string Label { get; set; } = string.Empty;
        public int? NextNodeId { get; set; }
    }

    public class BotReplyDTO
    {
        public int MessageId { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTimeOffset Timestamp { get; set; }
        public List<FAQOptionDTO> FAQOptions { get; set; } = new ();
    }
}
