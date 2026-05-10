using AirportApp.Src.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AirportApp.Src.View.General
{
    public sealed partial class UpperBar : UserControl
    {
        public AirportApp.Src.ViewModel.UpperBarViewModel ViewModel { get; }

        public UpperBar()
        {
            this.InitializeComponent();

            ViewModel = (App.Current as App).Services.GetService<AirportApp.Src.ViewModel.UpperBarViewModel>();
            this.DataContext = ViewModel;
        }

        private DependencyObject FindParentFrame()
        {
            DependencyObject parent = this.Parent;
            while (parent != null && !(parent is Frame))
            {
                parent = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetParent(parent);
            }
            return parent;
        }

        public void OnChatRequested(object sender, RoutedEventArgs arguments)
        {
            if (FindParentFrame() is Frame frame)
            {
                frame.Navigate(ViewModel.ChatPageType);
            }
        }

        public void OnLandingRequested(object sender, RoutedEventArgs arguments)
        {
            if (FindParentFrame() is Frame frame)
            {
                frame.Navigate(ViewModel.LandingPageType);
            }
        }

        public void OnFAQRequested(object sender, RoutedEventArgs arguments)
        {
            if (FindParentFrame() is Frame frame)
            {
                frame.Navigate(ViewModel.FAQPageType);
            }
        }

        public void OnTicketsRequested(object sender, RoutedEventArgs arguments)
        {
            if (FindParentFrame() is Frame frame)
            {
                frame.Navigate(ViewModel.TicketsPageType);
            }
        }

        public void OnReviewsRequested(object sender, RoutedEventArgs arguments)
        {
            if (FindParentFrame() is Frame frame)
            {
                frame.Navigate(ViewModel.ReviewsPageType);
            }
        }

        public void OnHomeRequested(object sender, RoutedEventArgs arguments)
        {
            // Navigate to choosing page using the Window's root content safe for WinUI 3
            if (this.XamlRoot.Content is Frame rootFrame)
            {
                rootFrame.Navigate(ViewModel.ChoosingPageType);
            }
        }
    }
}