using System.ComponentModel.DataAnnotations;

namespace AirportApp.Mvc.Models.Chat
{
    public class SendMessageViewModel
    {
        [Required]
        public int ChatId { get; set; }

        [Required]
        [Display(Name = "Sender")]
        public int SenderId { get; set; }

        [Required]
        [Display(Name = "Message")]
        public string Text { get; set; } = string.Empty;
    }
}
