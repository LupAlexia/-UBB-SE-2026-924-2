using System.ComponentModel.DataAnnotations;

namespace AirportApp.Mvc.Models.Review
{
    public class EditReviewViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Message")]
        public string Message { get; set; } = string.Empty;

        [Required]
        [Range(1, 5)]
        [Display(Name = "Duty Free Rating")]
        public int DutyFreeRating { get; set; }

        [Required]
        [Range(1, 5)]
        [Display(Name = "Flight Experience Rating")]
        public int FlightExperienceRating { get; set; }

        [Required]
        [Range(1, 5)]
        [Display(Name = "Staff Friendliness Rating")]
        public int StaffFriendlinessRating { get; set; }

        [Required]
        [Range(1, 5)]
        [Display(Name = "Cleanliness Rating")]
        public int CleanlinessRating { get; set; }

        [Required]
        [Display(Name = "User")]
        public int UserId { get; set; }
    }
}
