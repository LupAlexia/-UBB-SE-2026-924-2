using System.ComponentModel.DataAnnotations;

namespace AirportApp.Mvc.Models.AddOn
{
    public class EditAddOnViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Add-On Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Base Price")]
        public float BasePrice { get; set; }
    }
}
