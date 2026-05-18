using System.ComponentModel.DataAnnotations;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Mvc.Models.FAQ
{
    public class CreateFAQEntryViewModel
    {
        [Required]
        [MaxLength(500)]
        [Display(Name = "Question")]
        public string Question { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Answer")]
        public string Answer { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Category")]
        public FAQCategoryEnum Category { get; set; }
    }
}
