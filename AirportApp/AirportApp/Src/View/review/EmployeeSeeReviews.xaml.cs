using AirportApp.Src.ViewModel.Review;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace AirportApp.Src.View.Review
{
    public sealed partial class EmployeeSeeReviews : Page
    {
        public AllReviewsViewModel ViewModel { get; }

        public EmployeeSeeReviews()
        {
            this.InitializeComponent();

            ViewModel = (App.Current as App).Services.GetService<AllReviewsViewModel>();

            this.DataContext = ViewModel;
        }
    }
}