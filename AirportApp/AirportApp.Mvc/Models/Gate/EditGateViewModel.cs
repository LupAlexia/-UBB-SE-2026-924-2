using System.ComponentModel.DataAnnotations;

namespace AirportApp.Mvc.Models.Gate
{
    public class EditGateViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(10)]
        [Display(Name = "Gate Name")]
        public string GateName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Airport")]
        public int AirportId { get; set; }
    }
}
