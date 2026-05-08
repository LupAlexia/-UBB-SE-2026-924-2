using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AirportApp.Src.View.General
{
    public sealed partial class UserHomePage : Page
    {
        public UserHomePage()
        {
            InitializeComponent();
        }

        private void CustomerSupportButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LandingPage));
        }

        private void ManageTicketsButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(TravelPage));
        }

        private void SwitchToEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            // Intoarce userul la ChoosingPage ca sa se poata loga ca employee
            Frame.Navigate(typeof(ChoosingPage));
        }
    }
}
