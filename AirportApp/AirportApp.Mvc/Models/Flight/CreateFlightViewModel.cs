using System.ComponentModel.DataAnnotations;

namespace AirportApp.Mvc.Models.Flight
{
    public class CreateFlightViewModel
    {
        [Required]
        [MaxLength(10)]
        [Display(Name = "Flight Number")]
        public string FlightNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Departure Date")]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = "Route")]
        public int RouteId { get; set; }

        [Required]
        [Display(Name = "Gate")]
        public int GateId { get; set; }
    }
}
