using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AirportApp.Src.ViewModel
{
    public partial class AddReviewViewModel : ObservableObject
    {
        private readonly IReviewService reviewService;

        public event EventHandler<(string Title, string Message)>? AlertRequested;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitReviewCommand))]
        [NotifyPropertyChangedFor(nameof(DutyText))]
        private int dutyRating;
        public string DutyText => DutyRating > 0 ? $"{DutyRating}/5" : "Not rated";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitReviewCommand))]
        [NotifyPropertyChangedFor(nameof(FlightText))]
        private int flightRating;
        public string FlightText => FlightRating > 0 ? $"{FlightRating}/5" : "Not rated";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitReviewCommand))]
        [NotifyPropertyChangedFor(nameof(StaffText))]
        private int staffRating;
        public string StaffText => StaffRating > 0 ? $"{StaffRating}/5" : "Not rated";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitReviewCommand))]
        [NotifyPropertyChangedFor(nameof(CleanText))]
        private int cleanRating;
        public string CleanText => CleanRating > 0 ? $"{CleanRating}/5" : "Not rated";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitReviewCommand))]
        [NotifyPropertyChangedFor(nameof(CharCountText))]
        private string reviewMessage = string.Empty;
        public string CharCountText => $"{ReviewMessage?.Length ?? 0} characters";

        public AddReviewViewModel(IReviewService reviewService)
        {
            this.reviewService = reviewService;
        }

        private bool CanSubmitReview()
        {
            return DutyRating > 0 &&
                   FlightRating > 0 &&
                   StaffRating > 0 &&
                   CleanRating > 0 &&
                   !string.IsNullOrWhiteSpace(ReviewMessage);
        }

        [RelayCommand(CanExecute = nameof(CanSubmitReview))]
        private async Task SubmitReviewAsync()
        {
            try
            {
                var application = App.Current as App;
                User? currentUser = application?.User;

                if (currentUser == null)
                {
                    AlertRequested?.Invoke(this, ("Not Logged In", "Oopsie Daisy! You must be logged in to leave a review"));
                    return;
                }

                await reviewService.CreateReviewAsync(1, currentUser, ReviewMessage, DutyRating, FlightRating, StaffRating, CleanRating);

                DutyRating = 0;
                FlightRating = 0;
                StaffRating = 0;
                CleanRating = 0;
                ReviewMessage = string.Empty;

                AlertRequested?.Invoke(this, ("Success", "Your review has been successfully submitted!"));
            }
            catch (Exception exceptionThrown)
            {
                AlertRequested?.Invoke(this, ("Oopsie Daisy! Error", $"We couldn't submit your review: {exceptionThrown.Message}"));
            }
        }
    }
}