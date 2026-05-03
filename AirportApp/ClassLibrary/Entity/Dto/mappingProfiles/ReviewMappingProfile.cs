using AutoMapper;
using AirportApp.ClassLibrary.Entity.Domain.Review;
using Microsoft.IdentityModel.Tokens;

namespace AirportApp.ClassLibrary.Entity.Dto.MappingProfiles
{
    public class ReviewMappingProfile : Profile
    {
        public ReviewMappingProfile()
        {
            System.Diagnostics.Debug.WriteLine("ReviewMappingProfile Loaded!");
            CreateMap<Review, ReviewDTO>()

            .ConstructUsing(source => new ReviewDTO(
                source.GetId(),
                source.GetUser().UserId,
                source.GetUser().RetrieveConfiguredDisplayFullNameForBot(),
                source.GetMessage(),
                source.GetDutyFreeRating(),
                source.GetFlightExperienceRating(),
                source.GetStaffFriendlinessRating(),
                source.GetCleanlinessRating(),
                CalculateOverallAverage(source))); // Replaces manual math in loop
        }

        private static float CalculateOverallAverage(Review review)
        {
            float sumOfRatings = review.GetDutyFreeRating() +
                                 review.GetFlightExperienceRating() +
                                 review.GetStaffFriendlinessRating() +
                                 review.GetCleanlinessRating();

            return sumOfRatings / 4.0f;
        }
    }
}
