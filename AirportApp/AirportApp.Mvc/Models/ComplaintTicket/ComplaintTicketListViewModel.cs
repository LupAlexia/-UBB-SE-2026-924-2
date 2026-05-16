using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Mvc.Models.ComplaintTicket
{
    public class ComplaintTicketListViewModel
    {
        public List<ComplaintTicketViewModel> Tickets { get; set; } = new List<ComplaintTicketViewModel>();

        public ComplaintTicketUrgencyLevelEnum? FilterUrgencyLevel { get; set; }

        public ComplaintTicketStatusEnum? FilterStatus { get; set; }

        public int? FilterCategoryId { get; set; }
    }
}
