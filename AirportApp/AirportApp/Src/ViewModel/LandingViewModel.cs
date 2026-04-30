using System.Collections.ObjectModel;
using AutoMapper;
using AirportApp.Src.Dto;
using AirportApp.Src.Service;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AirportApp.Src.ViewModel
{
    public partial class LandingViewModel : ObservableObject
    {
        private readonly ReviewService reviewService;
        private readonly IMapper mapper;

        public ObservableCollection<ReviewDTO> Reviews { get; } = new ();

        public LandingViewModel(ReviewService reviewService, IMapper mapper)
        {
            this.reviewService = reviewService;
            this.mapper = mapper;
            LoadReviews();
        }

        public void LoadReviews()
        {
            var allReviews = reviewService.GetAll();
            Reviews.Clear();

            foreach (var review in allReviews)
            {
                string realName = review.GetUser().RetrieveConfiguredDisplayFullNameForBot();

                float averageRating = reviewService.CalculateAverageRating(review);

                var reviewDateTime = mapper.Map<ReviewDTO>(review);

                var finalDateTime = reviewDateTime with
                {
                    userName = realName,
                    overallRating = averageRating
                };

                Reviews.Add(finalDateTime);
            }
        }
    }
}