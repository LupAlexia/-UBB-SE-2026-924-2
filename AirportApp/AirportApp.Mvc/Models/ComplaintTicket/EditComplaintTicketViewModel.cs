using System.ComponentModel.DataAnnotations;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Mvc.Models.ComplaintTicket
{
    public class EditComplaintTicketViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        [Display(Name = "Subject")]
        public string Subject { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Status")]
        public ComplaintTicketStatusEnum CurrentStatus { get; set; }

        [Required]
        [Display(Name = "Urgency Level")]
        public ComplaintTicketUrgencyLevelEnum UrgencyLevel { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required]
        [Display(Name = "Subcategory")]
        public int SubcategoryId { get; set; }
    }
}
