namespace AirportApp.ClassLibrary.Entity.Domain.Faq.Bot
{
    public class FAQOptionEntity
    {
        // composite key: NodeId + Label
        public int NodeId { get; set; }
        public string Label { get; set; } = string.Empty;
        public int NextOptionId { get; set; }
    }
}
