using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Mvc.Models.Chat
{
    public class ChatViewModel
    {
        public int Id { get; set; }

        public ChatStatus Status { get; set; }

        public string StatusName => Status.ToString();

        public int UserId { get; set; }

        public string UserFullName { get; set; } = string.Empty;

        public int MessageCount { get; set; }

        public DateTimeOffset? LastMessageTimestamp { get; set; }
    }
}
