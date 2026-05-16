using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.Src.Service.Interfaces;
using AirportApp.ClassLibrary.Service.Interfaces;

namespace AirportApp.Src.ViewModel
{
    public class FlightSearchViewModel : ViewModelBase
    {
        private readonly IFlightSearchService searchService;
        private readonly INavigationService navigationService;
        private readonly IPricingService pricingService;

        private string location = string.Empty;
        public string Location
        {
            get => location;
            set
            {
                location = value;
                OnPropertyChanged();
            }
        }

        private bool isDeparture = true;
        public bool IsDeparture
        {
            get => isDeparture;
            set
            {
                isDeparture = value;
                OnPropertyChanged();
            }
        }

        private DateTimeOffset? flightDate;
        public DateTimeOffset? FlightDate
        {
            get => flightDate;
            set
            {
                flightDate = value;
                OnPropertyChanged();
            }
        }

        private string passengers = string.Empty;
        public string Passengers
        {
            get => passengers;
            set
            {
                passengers = value;
                OnPropertyChanged();
            }
        }

        private string searchResultMessage = string.Empty;
        public string SearchResultMessage
        {
            get => searchResultMessage;
            set
            {
                searchResultMessage = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<FlightDisplayModel> AvailableFlights { get; set; }

        public ICommand SearchCommand { get; }
        public ICommand BookFlightCommand { get; }

        public FlightSearchViewModel(IFlightSearchService searchService, INavigationService navigationService, IPricingService pricingService)
        {
            this.searchService = searchService;
            this.navigationService = navigationService;
            this.pricingService = pricingService;
            AvailableFlights = new ObservableCollection<FlightDisplayModel>();

            SearchCommand = new RelayCommand(async parameter => await ExecuteSearchAsync());
            BookFlightCommand = new RelayCommand(parameter => ExecuteBookFlight(parameter as FlightDisplayModel));
        }

        public void OnNavigatedTo(object parameter)
        {
            if (parameter is User user)
            {
                // UserSession.CurrentUser = user;
            }
        }

        private async Task ExecuteSearchAsync()
        {
            AvailableFlights.Clear();
            SearchResultMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Location))
            {
                return;
            }

            DateTime? date = FlightDate?.Date;
            int? requestedPassengers = searchService.ParsePassengerCount(Passengers);

            var results = await searchService.SearchFlightsAsync(Location, IsDeparture, date, requestedPassengers);
            bool hasResults = false;

            foreach (var flight in results)
            {
                AvailableFlights.Add(new FlightDisplayModel(flight, pricingService.CalculateBasePrice(flight)));
                hasResults = true;
            }

            if (!hasResults)
            {
                SearchResultMessage = "No flights found for the selected criteria.";
            }
        }

        private void ExecuteBookFlight(FlightDisplayModel? selectedFlightDisplay)
        {
            if (selectedFlightDisplay?.Flight == null)
            {
                return;
            }

            int passengerCount = searchService.ParsePassengerCount(Passengers) ?? 0;
            var bookingParameters = new object[] { selectedFlightDisplay.Flight, passengerCount };

            if (UserSession.CurrentUser == null)
            {
                UserSession.PendingBookingParameters = bookingParameters;
                navigationService.NavigateTo(typeof(View.AuthPage));
                return;
            }

            navigationService.NavigateTo(typeof(View.BookingPage), bookingParameters);
        }
    }
}
