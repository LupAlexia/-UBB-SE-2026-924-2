namespace AirportApp.ClassLibrary.Entity.Domain.Review
{
    public class Review
    {
        //private int id;
        //private User user;
        //private string message;
        //private int dutyFreeRating;
        //private int flightExperienceRating;
        //private int staffFriendlinessRating;
        //private int cleanlinessRating;

        // 1. EF Core Auto-Properties
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public int DutyFreeRating { get; set; }
        public int FlightExperienceRating { get; set; }
        public int StaffFriendlinessRating { get; set; }
        public int CleanlinessRating { get; set; }

        // 2. Navigation Properties (EF Core link to User)
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // 3. EF Core Required Parameterless Constructor
        public Review() { }

        //public Review(int id, User user, string message, int dutyFreeRating, int flightExperienceRating, int staffFriendlinesRating, int cleanlinessRating)
        //{
        //    this.id = id;
        //    this.user = user;
        //    this.message = message;
        //    this.dutyFreeRating = dutyFreeRating;
        //    this.flightExperienceRating = flightExperienceRating;
        //    this.staffFriendlinessRating = staffFriendlinesRating;
        //    this.cleanlinessRating = cleanlinessRating;
        //}
        public Review(int id, User user, string message, int dutyFreeRating,
                      int flightExperienceRating, int staffFriendlinessRating, int cleanlinessRating)
        {
            Id = id;
            User = user;
            UserId = user.UserId;
            Message = message;
            DutyFreeRating = dutyFreeRating;
            FlightExperienceRating = flightExperienceRating;
            StaffFriendlinessRating = staffFriendlinessRating;
            CleanlinessRating = cleanlinessRating;
        }

        // GETTERSm
        //public int GetId()
        //{
        //    return this.id;
        //}

        //public User GetUser()
        //{
        //    return this.user;
        //}
        //public string GetMessage()
        //{
        //    return this.message;
        //}
        //public int GetDutyFreeRating()
        //{
        //    return this.dutyFreeRating;
        //}
        //public int GetFlightExperienceRating()
        //{
        //    return this.flightExperienceRating;
        //}
        //public int GetStaffFriendlinessRating()
        //{
        //    return this.staffFriendlinessRating;
        //}
        //public int GetCleanlinessRating()
        //{
        //    return this.cleanlinessRating;
        //}
    }
}
