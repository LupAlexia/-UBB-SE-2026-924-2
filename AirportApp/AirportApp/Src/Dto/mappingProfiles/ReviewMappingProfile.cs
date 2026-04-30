using AutoMapper;
using AirportApp.Src.Model.Review;
using AirportApp.Src.Dto;
using Microsoft.IdentityModel.Tokens;

namespace AirportApp.Src.Dto.MappingProfiles
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
