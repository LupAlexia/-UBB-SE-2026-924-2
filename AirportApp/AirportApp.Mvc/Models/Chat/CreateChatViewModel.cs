using System.ComponentModel.DataAnnotations;

namespace AirportApp.Mvc.Models.Chat
{
    public class CreateChatViewModel
    {
        [Required]
        [Display(Name = "User")]
        public int UserId { get; set; }

        [Display(Name = "Initial Message")]
        public string? InitialMessage { get; set; }
    }
}
