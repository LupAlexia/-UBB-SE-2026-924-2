using AirportApp.Src.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace AirportApp.Src.View.Review
{
    public sealed partial class EmployeeSeeReviewsPage : Page
    {
        public AllReviewsViewModel ViewModel { get; }

        public EmployeeSeeReviewsPage()
        {
            this.InitializeComponent();

            ViewModel = (App.Current as App).Services.GetService<AllReviewsViewModel>();

            this.DataContext = ViewModel;
        }
    }
}