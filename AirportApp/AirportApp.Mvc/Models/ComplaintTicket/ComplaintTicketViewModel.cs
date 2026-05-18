using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Mvc.Models.ComplaintTicket
{
    public class ComplaintTicketViewModel
    {
        public int Id { get; set; }

        public string Subject { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime CreationTimestamp { get; set; }

        public ComplaintTicketUrgencyLevelEnum UrgencyLevel { get; set; }

        public string UrgencyLevelName => UrgencyLevel.ToString();

        public ComplaintTicketStatusEnum CurrentStatus { get; set; }

        public string StatusName => CurrentStatus.ToString();

        public int CreatorId { get; set; }

        public string CreatorName { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public int SubcategoryId { get; set; }

        public string SubcategoryName { get; set; } = string.Empty;
    }
}
