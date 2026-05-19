using System;
using System.Linq;
using AirportApp.Src.View.General;
using AirportApp.Src.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace AirportApp.Src.View
{
    public sealed partial class FlightSearchPage : Page
    {
        public FlightSearchViewModel ViewModel { get; }

        public FlightSearchPage()
        {
            this.InitializeComponent();

            ViewModel = new FlightSearchViewModel(App.FlightSearchService, App.NavigationService, App.PricingService);
            this.DataContext = ViewModel;
        }

        private void PassengersInput_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs eventArgs)
        {
            if (eventArgs.NewText.Any(character => !char.IsDigit(character)))
            {
                eventArgs.Cancel = true;
            }
        }

        private void DatePicker_CalendarViewDayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs eventArgs)
        {
            if (eventArgs.Item.Date.Date < DateTimeOffset.Now.Date)
            {
                eventArgs.Item.IsBlackout = true;
            }
        }

        private void ClearDateButton_Click(object? sender, RoutedEventArgs eventArgs)
        {
            ViewModel.FlightDate = null;
        }

        protected override void OnNavigatedTo(NavigationEventArgs eventArgs)
        {
            var app = (App)Application.Current;
            if (app.User == null && app.Employee == null)
            {
                Frame.Navigate(typeof(ChoosingPage));
                return;
            }

            base.OnNavigatedTo(eventArgs);
            ViewModel.OnNavigatedTo(eventArgs.Parameter);
        }
    }
}




