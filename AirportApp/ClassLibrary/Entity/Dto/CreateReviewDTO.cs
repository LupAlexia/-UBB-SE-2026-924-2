using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportApp.ClassLibrary.Entity.Dto
{
    public record CreateReviewDTO(
        int userId,
        string message,
        int dutyFreeRating,
        int flightExperienceRating,
        int staffFriendlinessRating,
        int cleanlinessRating);
}
