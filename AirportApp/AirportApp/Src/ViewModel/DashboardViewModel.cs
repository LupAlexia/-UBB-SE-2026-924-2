using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interfaces;
using INavigationService = AirportApp.Src.Service.Interfaces.INavigationService;

namespace AirportApp.Src.ViewModel
{
    public partial class DashboardViewModel : ViewModelBase
    {
        private readonly IDashboardService dashboardService;
        private readonly ICancellationService cancellationService;
        private readonly INavigationService navigationService;

        public string WelcomeMessage => UserSession.CurrentUser != null
            ? $"Welcome, {UserSession.CurrentUser.Username}!"
            : "Welcome!";

        public ObservableCollection<FlightTicket> MyTickets { get; set; }
        public ObservableCollection<string> TicketFilters { get; }

        private string selectedTicketFilter = "Upcoming";
        public string SelectedTicketFilter
        {
            get => selectedTicketFilter;
            set
            {
                if (selectedTicketFilter == value)
                {
                    return;
                }

                selectedTicketFilter = value;
                OnPropertyChanged();
                _ = LoadUserTicketsAsync();
            }
        }

        private string cancellationMessage = string.Empty;
        public string CancellationMessage
        {
            get => cancellationMessage;
            set
            {
                cancellationMessage = value;
                OnPropertyChanged();
            }
        }

        private bool? cancellationSucceeded;
        public bool? CancellationSucceeded
        {
            get => cancellationSucceeded;
            set
            {
                cancellationSucceeded = value;
                OnPropertyChanged();
            }
        }

        private FlightTicket? pendingCancelTicket;
        public FlightTicket? PendingCancelTicket
        {
            get => pendingCancelTicket;
            set
            {
                pendingCancelTicket = value;
                OnPropertyChanged();
            }
        }

        public ICommand CancelTicketCommand { get; }
        public ICommand DownloadPdfCommand { get; }

        public DashboardViewModel(IDashboardService dashboardService, ICancellationService cancellationService, INavigationService navigationService)
        {
            this.dashboardService = dashboardService;
            this.cancellationService = cancellationService;
            this.navigationService = navigationService;

            MyTickets = new ObservableCollection<FlightTicket>();
            TicketFilters = new ObservableCollection<string> { "Upcoming", "Past" };

            CancelTicketCommand = new RelayCommand(ExecuteCancelTicket);
            DownloadPdfCommand = new RelayCommand(ExecuteDownloadPdf);

            _ = LoadUserTicketsAsync();
        }

        public async Task LoadUserTicketsAsync()
        {
            MyTickets.Clear();
            int? currentUserId = UserSession.CurrentUser?.Id;
            if (!currentUserId.HasValue)
            {
                return;
            }

            var filteredTickets = await dashboardService.GetUserTicketsAsync(currentUserId.Value, SelectedTicketFilter);
            foreach (var flightTicket in filteredTickets)
            {
                MyTickets.Add(flightTicket);
            }
        }

        private void ExecuteCancelTicket(object parameter)
        {
            CancellationSucceeded = null;
            CancellationMessage = string.Empty;

            if (parameter is not FlightTicket flightTicket)
            {
                return;
            }

            var (canCancel, reason) = cancellationService.CanCancelTicket(flightTicket);
            if (!canCancel)
            {
                CancellationSucceeded = false;
                CancellationMessage = reason;
                return;
            }

            PendingCancelTicket = flightTicket;
        }

        public async Task ConfirmCancellationAsync()
        {
            if (PendingCancelTicket == null)
            {
                return;
            }

            await cancellationService.CancelTicketAsync(PendingCancelTicket.Id);
            PendingCancelTicket = null;
            await LoadUserTicketsAsync();

            CancellationSucceeded = true;
            CancellationMessage = "The FlightTicket status was updated to Cancelled.";
        }

        public void DeclineCancellation()
        {
            PendingCancelTicket = null;
        }

        public async Task<bool> OnNavigatedToAsync()
        {
            if (UserSession.CurrentUser == null)
            {
                navigationService.NavigateTo(typeof(View.AuthPage));
                return false;
            }

            await LoadUserTicketsAsync();
            return true;
        }

        private void ExecuteDownloadPdf(object parameter)
        {
            if (parameter is FlightTicket flightTicket)
            {
                try
                {
                    string generatedFilePath = dashboardService.GenerateTicketPdf(flightTicket);

                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = generatedFilePath,
                        UseShellExecute = true
                    });
                }
                catch (Exception exception)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to generate PDF: {exception.Message}");
                }
            }
        }
    }
}
