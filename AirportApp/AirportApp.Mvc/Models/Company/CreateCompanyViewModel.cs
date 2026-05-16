using System.ComponentModel.DataAnnotations;

namespace AirportApp.Mvc.Models.Company
{
    public class CreateCompanyViewModel
    {
        [Required]
        [MaxLength(100)]
        [Display(Name = "Company Name")]
        public string Name { get; set; } = string.Empty;
    }
}
