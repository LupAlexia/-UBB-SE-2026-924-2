using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Input;
using AirportApp.Src.Domain;
using AirportApp.Src.Service;
using AirportApp.Src.Model;

namespace AirportApp.Src.ViewModel
{
    public class BookingViewModel : ViewModelBase
    {
        private const int DefaultFlightCapacity = 180;
        private readonly IBookingService bookingService;
        private readonly IPricingService pricingService;
        private readonly INavigationService navigationService;
        private readonly RelayCommand confirmBookingCommand;
        private bool isSaving;
        private bool passengersValid;

        private Flight currentFlight = null!;
        public Flight CurrentFlight
        {
            get => currentFlight;
            set
            {
                currentFlight = value;
                OnPropertyChanged();
            }
        }

        private User currentUser = null!;
        public User CurrentUser
        {
            get => currentUser;
            set
            {
                currentUser = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<PassengerFormViewModel> passengersList = new ObservableCollection<PassengerFormViewModel>();
        public ObservableCollection<PassengerFormViewModel> Passengers
        {
            get => passengersList;
            set
            {
                passengersList = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<AddOn> availableAddOns = new ObservableCollection<AddOn>();
        public ObservableCollection<AddOn> AvailableAddOns
        {
            get => availableAddOns;
            set
            {
                availableAddOns = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> occupiedSeats = new ObservableCollection<string>();
        public ObservableCollection<string> OccupiedSeats
        {
            get => occupiedSeats;
            set
            {
                occupiedSeats = value;
                OnPropertyChanged();
            }
        }

        private float basePriceTotal;
        public float BasePriceTotal
        {
            get => basePriceTotal;
            set
            {
                basePriceTotal = value;
                OnPropertyChanged();
            }
        }

        private float basePricePerPerson;
        public float BasePricePerPerson
        {
            get => basePricePerPerson;
            set
            {
                basePricePerPerson = value;
                OnPropertyChanged();
            }
        }

        private float finalTotalPrice;
        public float FinalTotalPrice
        {
            get => finalTotalPrice;
            set
            {
                finalTotalPrice = value;
                OnPropertyChanged();
            }
        }

        private float addOnsTotal;
        public float AddOnsTotal
        {
            get => addOnsTotal;
            set
            {
                addOnsTotal = value;
                OnPropertyChanged();
            }
        }

        private float membershipSavings;
        public float MembershipSavings
        {
            get => membershipSavings;
            set
            {
                membershipSavings = value;
                OnPropertyChanged();
            }
        }

        public string BasePricePerPersonDisplay => $"{BasePricePerPerson:0.00} €";
        public string BasePriceTotalDisplay => $"{BasePriceTotal:0.00} €";
        public string AddOnsTotalDisplay => $"{AddOnsTotal:0.00} €";
        public string MembershipSavingsDisplay => $"-{MembershipSavings:0.00} €";
        public string FinalTotalPriceDisplay => $"{FinalTotalPrice:0.00} €";

        private string validationMessage = string.Empty;
        public string ValidationMessage
        {
            get => validationMessage;
            set
            {
                validationMessage = value;
                OnPropertyChanged();
            }
        }

        private int maximumPassengers;
        public int MaximumPassengers
        {
            get => maximumPassengers;
            set
            {
                maximumPassengers = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanAddPassenger));
            }
        }

        public bool CanAddPassenger => Passengers.Count < MaximumPassengers;
        public bool CanRemovePassenger => Passengers.Count > 1;
        public bool CanConfirmBooking =>
            !isSaving &&
            CurrentUser != null &&
            CurrentFlight != null &&
            Passengers.Count > 0 &&
            passengersValid;

        public event EventHandler? BookingConfirmed;

        public BookingViewModel(IBookingService bookingService, IPricingService pricingService, INavigationService navigationService)
        {
            this.bookingService = bookingService;
            this.pricingService = pricingService;
            this.navigationService = navigationService;

            AddPassengerCommand = new RelayCommand(parameter => AddPassenger());
            RemovePassengerCommand = new RelayCommand(parameter => RemovePassenger(parameter as PassengerFormViewModel));
            confirmBookingCommand = new RelayCommand(async parameter => await ConfirmBookingAsync(), parameter => CanConfirmBooking);
            ConfirmBookingCommand = confirmBookingCommand;
        }

        public ICommand AddPassengerCommand { get; }
        public ICommand RemovePassengerCommand { get; }
        public ICommand ConfirmBookingCommand { get; }

        public List<SeatDescriptor> SeatMapLayout { get; private set; } = new List<SeatDescriptor>();

        public int SeatMapRowCount { get; private set; }

        public async Task<bool> OnNavigatedToAsync(object parameter)
        {
            var parsed = bookingService.ParseBookingParameters(parameter);

            if (parsed == null)
            {
                if (parameter is object[] arr && arr.Length > 0 && arr[0] is Flight fallbackFlight)
                {
                    User? fallbackUser = null;
                    int requested = 0;
                    foreach (var item in arr)
                    {
                        if (fallbackUser == null && item is User u)
                        {
                            fallbackUser = u;
                        }
                        else if (requested == 0 && item is int rp)
                        {
                            requested = rp;
                        }
                    }

                    parsed = new BookingParametersResult
                    {
                        Flight = fallbackFlight,
                        User = fallbackUser,
                        RequestedPassengers = requested
                    };
                }
                else if (parameter is Flight singleFlight)
                {
                    parsed = new BookingParametersResult
                    {
                        Flight = singleFlight,
                       // User = UserSession.CurrentUser,
                        RequestedPassengers = 0
                    };
                }
            }

            if (parsed == null || parsed.Flight == null)
            {
                return false;
            }

            if (parsed.User == null)
            {
                bookingService.StorePendingBooking(parsed.Flight, parsed.RequestedPassengers);
                navigationService.NavigateTo(typeof(View.AuthPage));
                return false;
            }

            await InitializeAsync(parsed.Flight, parsed.User, parsed.RequestedPassengers);
            return true;
        }

        public async Task InitializeAsync(Flight flight, User user, int requestedPassengerCount = 0)
        {
            CurrentFlight = flight;
            CurrentUser = user;

            var addOns = await bookingService.GetAvailableAddOnsAsync();
            AvailableAddOns.Clear();
            foreach (var addOn in addOns)
            {
                AvailableAddOns.Add(addOn);
            }

            var seats = await bookingService.GetOccupiedSeatsAsync(flight?.FlightId ?? 0);
            OccupiedSeats.Clear();
            foreach (var seat in seats)
            {
                OccupiedSeats.Add(seat);
            }

            int capacity = flight?.Route?.Capacity ?? DefaultFlightCapacity;
            MaximumPassengers = bookingService.CalculateMaxPassengers(capacity, OccupiedSeats.Count, requestedPassengerCount);

            Passengers.Clear();
            int initialCount = bookingService.GetInitialPassengerCount(MaximumPassengers, requestedPassengerCount);

            if (initialCount < 1)
            {
                initialCount = Math.Min(MaximumPassengers, Math.Max(1, requestedPassengerCount));
            }

            for (int index = 0; index < initialCount; index++)
            {
                var passenger = new PassengerFormViewModel();
                RegisterPassenger(passenger);
                Passengers.Add(passenger);
            }

            UpdatePassengerLabels();
            UpdatePrices();
            OnPropertyChanged(nameof(CanAddPassenger));
            OnPropertyChanged(nameof(CanRemovePassenger));
            RefreshBookingState();
            BuildSeatMapLayout();
        }

        public void BuildSeatMapLayout()
        {
            int capacity = CurrentFlight?.Route?.Capacity ?? DefaultFlightCapacity;
            var (layout, rowCount) = bookingService.BuildSeatMapLayout(capacity);
            SeatMapLayout = layout;
            SeatMapRowCount = rowCount;
            OnPropertyChanged(nameof(SeatMapLayout));
            OnPropertyChanged(nameof(SeatMapRowCount));
        }

        private void AddPassenger()
        {
            if (!CanAddPassenger)
            {
                return;
            }

            var passenger = new PassengerFormViewModel();
            RegisterPassenger(passenger);
            Passengers.Add(passenger);
            UpdatePassengerLabels();
            UpdatePrices();
            OnPropertyChanged(nameof(CanAddPassenger));
            OnPropertyChanged(nameof(CanRemovePassenger));
            RefreshBookingState();
        }

        private void RemovePassenger(PassengerFormViewModel? passenger)
        {
            if (passenger != null && Passengers.Count > 1)
            {
                Passengers.Remove(passenger);
                UpdatePassengerLabels();
                UpdatePrices();
                OnPropertyChanged(nameof(CanAddPassenger));
                OnPropertyChanged(nameof(CanRemovePassenger));
                RefreshBookingState();
            }
        }

        private void UpdatePassengerLabels()
        {
            for (int index = 0; index < Passengers.Count; index++)
            {
                Passengers[index].PassengerLabel = $"Passenger {index + 1}";
            }
        }

        private void RegisterPassenger(PassengerFormViewModel passenger)
        {
            if (passenger.SelectedAddOns == null)
            {
                passenger.SelectedAddOns = new ObservableCollection<AddOn>();
            }

            passenger.SelectedAddOns.CollectionChanged += (sender, eventArgs) => UpdatePrices();
            passenger.PropertyChanged += (sender, eventArgs) =>
            {
                if (eventArgs.PropertyName == nameof(passenger.SelectedSeat) ||
                    eventArgs.PropertyName == nameof(passenger.FirstName) ||
                    eventArgs.PropertyName == nameof(passenger.LastName) ||
                    eventArgs.PropertyName == nameof(passenger.Email))
                {
                    RefreshBookingState();
                }

                if (eventArgs.PropertyName == nameof(passenger.SelectedSeat))
                {
                    UpdatePrices();
                }
            };
        }

        private System.Collections.Generic.List<PassengerData> MapPassengersToData()
        {
            return Passengers.Select(passenger => new PassengerData
            {
                FirstName = passenger.FirstName,
                LastName = passenger.LastName,
                Email = passenger.Email,
                Phone = passenger.Phone,
                SelectedSeat = passenger.SelectedSeat,
                SelectedAddOns = passenger.SelectedAddOns?.ToList() ?? new List<AddOn>()
            }).ToList();
        }

        private void RefreshBookingState()
        {
            ValidationMessage = string.Empty;

            if (CurrentUser == null)
            {
                ValidationMessage = "Please sign in to continue.";
                passengersValid = false;
            }
            else
            {
                var passengerData = MapPassengersToData();
                ValidationMessage = bookingService.ValidatePassengers(passengerData);
                passengersValid = string.IsNullOrEmpty(ValidationMessage);
            }

            OnPropertyChanged(nameof(CanConfirmBooking));
            confirmBookingCommand.RaiseCanExecuteChanged();
        }

        public void UpdatePrices()
        {
            if (CurrentFlight == null)
            {
                return;
            }

            float basePrice = pricingService.CalculateBasePrice(CurrentFlight);
            var passengerData = MapPassengersToData();
            var tickets = bookingService.CreateTickets(CurrentFlight, CurrentUser, passengerData, basePrice);
            var breakdown = pricingService.CalculatePriceBreakdown(CurrentFlight, CurrentUser, tickets);

            BasePricePerPerson = breakdown.BasePricePerPerson;
            BasePriceTotal = breakdown.BasePriceTotal;
            AddOnsTotal = breakdown.AddOnsTotal;
            MembershipSavings = breakdown.MembershipSavings;
            FinalTotalPrice = breakdown.FinalTotal;

            OnPropertyChanged(nameof(BasePricePerPersonDisplay));
            OnPropertyChanged(nameof(BasePriceTotalDisplay));
            OnPropertyChanged(nameof(AddOnsTotalDisplay));
            OnPropertyChanged(nameof(MembershipSavingsDisplay));
            OnPropertyChanged(nameof(FinalTotalPriceDisplay));

            RefreshBookingState();
        }

        private async Task ConfirmBookingAsync()
        {
            if (!CanConfirmBooking)
            {
                return;
            }

            float basePrice = pricingService.CalculateBasePrice(CurrentFlight);
            var passengerData = MapPassengersToData();
            var tickets = bookingService.CreateTickets(CurrentFlight, CurrentUser, passengerData, basePrice);
            foreach (var ticket in tickets)
            {
                ticket.Price = pricingService.CalculateTotalPrice(ticket);
            }

            isSaving = true;
            OnPropertyChanged(nameof(CanConfirmBooking));
            confirmBookingCommand.RaiseCanExecuteChanged();

            bool success = await bookingService.SaveTicketsAsync(tickets);

            isSaving = false;
            OnPropertyChanged(nameof(CanConfirmBooking));
            confirmBookingCommand.RaiseCanExecuteChanged();

            ValidationMessage = success ? "Booking confirmed successfully." : "Booking could not be saved. Please try again.";

            if (success)
            {
                BookingConfirmed?.Invoke(this, EventArgs.Empty);
            }
        }

        public void SelectSeat(PassengerFormViewModel targetPassenger, string seat)
        {
            var currentSeats = Passengers.Select(p => p.SelectedSeat ?? string.Empty).ToList();
            int targetIndex = Passengers.IndexOf(targetPassenger);
            var updatedSeats = bookingService.ApplySeatSelection(currentSeats, targetIndex, seat);
            for (int index = 0; index < Passengers.Count; index++)
            {
                Passengers[index].SelectedSeat = updatedSeats[index];
            }
        }

        public void UpdatePassengerAddOns(PassengerFormViewModel passenger, IEnumerable<AddOn> addedAddOns, IEnumerable<AddOn> removedAddOns)
        {
            bookingService.ApplyAddOnUpdates(passenger.SelectedAddOns, addedAddOns, removedAddOns);
        }
    }
}
