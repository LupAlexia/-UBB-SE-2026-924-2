namespace AirportApp.Mvc.Models.Review
{
    public class ReviewViewModel
    {
        public int Id { get; set; }

        public string Message { get; set; } = string.Empty;

        public int DutyFreeRating { get; set; }

        public int FlightExperienceRating { get; set; }

        public int StaffFriendlinessRating { get; set; }

        public int CleanlinessRating { get; set; }

        public int UserId { get; set; }

        public string UserFullName { get; set; } = string.Empty;

        public double AverageRating => (DutyFreeRating + FlightExperienceRating + StaffFriendlinessRating + CleanlinessRating) / 4.0;
    }
}
