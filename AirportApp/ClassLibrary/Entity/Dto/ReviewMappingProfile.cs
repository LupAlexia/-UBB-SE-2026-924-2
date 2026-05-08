using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Entity.Dto
{
    public class ReviewMappingProfile : Profile
    {
        public ReviewMappingProfile()
        {
            System.Diagnostics.Debug.WriteLine("ReviewMappingProfile Loaded!");
            CreateMap<Review, ReviewDTO>()

            .ConstructUsing(source => new ReviewDTO(
                source.Id,
                source.User.UserId,
                source.User.RetrieveConfiguredDisplayFullNameForBot(),
                source.Message,
                source.DutyFreeRating,
                source.FlightExperienceRating,
                source.StaffFriendlinessRating,
                source.CleanlinessRating,
                CalculateOverallAverage(source))); // Replaces manual math in loop
        }

        private static float CalculateOverallAverage(Review review)
        {
            float sumOfRatings = review.DutyFreeRating +
                                 review.FlightExperienceRating +
                                 review.StaffFriendlinessRating +
                                 review.CleanlinessRating;

            return sumOfRatings / 4.0f;
        }
    }
}
