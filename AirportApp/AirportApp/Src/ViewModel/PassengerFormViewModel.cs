using System.Collections.ObjectModel;

namespace AirportApp.Src.ViewModel
{
    public class PassengerFormViewModel : ViewModelBase
    {
        private string passengerLabel = "Passenger";
        public string PassengerLabel
        {
            get => passengerLabel;
            set
            {
                passengerLabel = value;
                OnPropertyChanged();
            }
        }

        private string firstName = string.Empty;
        public string FirstName
        {
            get => firstName;
            set
            {
                firstName = value;
                OnPropertyChanged();
            }
        }

        private string lastName = string.Empty;
        public string LastName
        {
            get => lastName;
            set
            {
                lastName = value;
                OnPropertyChanged();
            }
        }

        private string email = string.Empty;
        public string Email
        {
            get => email;
            set
            {
                email = value;
                OnPropertyChanged();
            }
        }

        private string phone = string.Empty;
        public string Phone
        {
            get => phone;
            set
            {
                phone = value;
                OnPropertyChanged();
            }
        }

        private string selectedSeat = string.Empty;
        public string SelectedSeat
        {
            get => selectedSeat;
            set
            {
                selectedSeat = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Domain.AddOn> SelectedAddOns { get; set; } = new ObservableCollection<Domain.AddOn>();
    }
}
