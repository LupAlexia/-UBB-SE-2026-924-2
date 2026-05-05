using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AirportApp.ClassLibrary.Entity.Domain.Review
{
    [Table("Reviews")]
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
        [Key]
        [Column("Review_Id")]
        public int Id { get; set; }

        [Required]
        [Column("Message", TypeName = "NVARCHAR(MAX)")]
        public string Message { get; set; } = string.Empty;
        [Range(1, 5)]
        [Column("Duty_Free_Rating")]
        public int DutyFreeRating { get; set; }

        [Range(1, 5)]
        [Column("Flight_Experience_Rating")]
        public int FlightExperienceRating { get; set; }

        [Range(1, 5)]
        [Column("Staff_Friendliness_Rating")]
        public int StaffFriendlinessRating { get; set; }

        [Range(1, 5)]
        [Column("Cleanliness_Rating")]
        public int CleanlinessRating { get; set; }

        // 2. Navigation Properties (EF Core link to User)
        [Required]
        [Column("User_Id")]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
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
            User = user ?? throw new ArgumentException("User cannot be null");
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
