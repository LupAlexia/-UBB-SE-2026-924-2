using System.ComponentModel.DataAnnotations;

namespace AirportApp.Mvc.Models.FlightTicket
{
    public class CreateFlightTicketViewModel
    {
        [Required]
        [Display(Name = "Flight")]
        public int FlightId { get; set; }

        [Required]
        [Display(Name = "Customer")]
        public int UserId { get; set; }

        [Required]
        [MaxLength(10)]
        [Display(Name = "Seat")]
        public string Seat { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Price")]
        public float Price { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "First Name")]
        public string PassengerFirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        [Display(Name = "Last Name")]
        public string PassengerLastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        [Display(Name = "Email")]
        public string PassengerEmail { get; set; } = string.Empty;

        [MaxLength(20)]
        [Display(Name = "Phone")]
        public string PassengerPhone { get; set; } = string.Empty;

        public List<int> SelectedAddOnIds { get; set; } = new List<int>();
    }
}
