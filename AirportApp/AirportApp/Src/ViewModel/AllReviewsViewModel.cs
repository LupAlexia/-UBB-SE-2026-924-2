using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AirportApp.ClassLibrary.Entity.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using ReviewEntity = AirportApp.ClassLibrary.Entity.Domain.Review;
using AirportApp.ClassLibrary.Service.Interfaces;

namespace AirportApp.Src.ViewModel
{
    public partial class AllReviewsViewModel : ObservableObject
    {
        private readonly IReviewService reviewService;
        private readonly IMapper mapper;

        public ObservableCollection<ReviewDTO> Reviews { get; } = new ();

        [ObservableProperty]
        private int totalReviews;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FormattedAverageDutyFree))]
        private double averageDutyFree;
        public string FormattedAverageDutyFree => AverageDutyFree.ToString("0.0");

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FormattedAverageFlightExperience))]
        private double averageFlightExperience;
        public string FormattedAverageFlightExperience => AverageFlightExperience.ToString("0.0");

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FormattedAverageStaffFriendliness))]
        private double averageStaffFriendliness;
        public string FormattedAverageStaffFriendliness => AverageStaffFriendliness.ToString("0.0");

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FormattedAverageCleanliness))]
        private double averageCleanliness;
        public string FormattedAverageCleanliness => AverageCleanliness.ToString("0.0");

        public AllReviewsViewModel(IReviewService reviewService, IMapper mapper)
        {
            this.reviewService = reviewService;
            this.mapper = mapper;
            _ = LoadDataAsync();
        }

        public async Task LoadDataAsync()
        {
            var reviewsFromDb = await reviewService.GetAllAsync();
            Reviews.Clear();

            if (reviewsFromDb == null || reviewsFromDb.Count == 0)
            {
                return;
            }

            TotalReviews = reviewsFromDb.Count;

            CalculateCategoryAverages(reviewsFromDb);

            var mappedReviews = mapper.Map<List<ReviewDTO>>(reviewsFromDb);

            foreach (var reviewDataTransferObject in mappedReviews)
            {
                Reviews.Add(reviewDataTransferObject);
            }
        }

        private void CalculateCategoryAverages(List<ReviewEntity> reviews)
        {
            AverageDutyFree = reviews.Average(review => review.DutyFreeRating);
            AverageFlightExperience = reviews.Average(review => review.FlightExperienceRating);
            AverageStaffFriendliness = reviews.Average(review => review.StaffFriendlinessRating);
            AverageCleanliness = reviews.Average(review => review.CleanlinessRating);
        }
    }
}
