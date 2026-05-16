using System.ComponentModel.DataAnnotations;

namespace AirportApp.Mvc.Models.Membership
{
    public class EditMembershipViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Membership Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0, 100)]
        [Display(Name = "Flight Discount (%)")]
        public float FlightDiscountPercentage { get; set; }
    }
}
