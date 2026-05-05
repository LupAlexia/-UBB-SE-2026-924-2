using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportApp.ClassLibrary.Entity.Dto
{
    public record CreateReviewDTO(
        int UserId,
        string Message,
        int DutyFreeRating,
        int FlightExperienceRating,
        int StaffFriendlinessRating,
        int CleanlinessRating
    );
}
