using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AutoMapper;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.Src.Service;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AirportApp.Src.ViewModel
{
    public partial class LandingViewModel : ObservableObject
    {
        private readonly IReviewService reviewService;
        private readonly IMapper mapper;

        public ObservableCollection<ReviewDTO> Reviews { get; } = new ();

        public LandingViewModel(IReviewService reviewService, IMapper mapper)
        {
            this.reviewService = reviewService;
            this.mapper = mapper;
            _ = LoadReviewsAsync();
        }

        public async Task LoadReviewsAsync()
        {
            var allReviews = await reviewService.GetAllAsync();
            Reviews.Clear();

            if (allReviews == null)
            {
                return;
            }

            foreach (var review in allReviews)
            {
                string realName = review.User.RetrieveConfiguredDisplayFullNameForBot();

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