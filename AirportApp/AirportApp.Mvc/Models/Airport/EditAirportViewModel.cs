using System.ComponentModel.DataAnnotations;

namespace AirportApp.Mvc.Models.Airport
{
    public class EditAirportViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Airport code must be exactly 3 characters.")]
        [Display(Name = "Airport Code")]
        public string AirportCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Display(Name = "City")]
        public string City { get; set; } = string.Empty;
    }
}
