using System.ComponentModel.DataAnnotations;

namespace AirportApp.Mvc.Models.Route
{
    public class CreateRouteViewModel
    {
        [Required]
        [MaxLength(50)]
        [Display(Name = "Route Type")]
        public string RouteType { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Departure Time")]
        public DateTime DepartureTime { get; set; }

        [Required]
        [Display(Name = "Arrival Time")]
        public DateTime ArrivalTime { get; set; }

        [Required]
        [Display(Name = "Capacity")]
        public int Capacity { get; set; }

        [Required]
        [Display(Name = "Company")]
        public int CompanyId { get; set; }

        [Required]
        [Display(Name = "Airport")]
        public int AirportId { get; set; }
    }
}
