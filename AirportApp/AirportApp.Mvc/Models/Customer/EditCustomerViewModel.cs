using System.ComponentModel.DataAnnotations;

namespace AirportApp.Mvc.Models.Customer
{
    public class EditCustomerViewModel
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        [Display(Name = "Phone")]
        public string? Phone { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Display(Name = "Membership")]
        public int? MembershipId { get; set; }
    }
}
