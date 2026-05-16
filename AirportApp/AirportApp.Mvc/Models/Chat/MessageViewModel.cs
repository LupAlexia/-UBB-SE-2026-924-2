namespace AirportApp.Mvc.Models.Chat
{
    public class MessageViewModel
    {
        public int Id { get; set; }

        public string Text { get; set; } = string.Empty;

        public DateTimeOffset Timestamp { get; set; }

        public int SenderId { get; set; }

        public string SenderFullName { get; set; } = string.Empty;

        public string SenderType { get; set; } = string.Empty;

        public int ChatId { get; set; }
    }
}
