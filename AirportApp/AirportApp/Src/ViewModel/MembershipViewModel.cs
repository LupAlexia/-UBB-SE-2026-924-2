using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.Src.Service.Interfaces;
using AirportApp.ClassLibrary.Service.Interfaces;

namespace AirportApp.Src.ViewModel
{
    public class MembershipDisplayModel
    {
        private const string BronzeMembershipColor = "#CD7F32";
        private const string SilverMembershipColor = "#A9A9A9";
        private const string GoldMembershipColor = "#DAA520";
        private const string DefaultMembershipColor = "#2bb8c0";

        public int MembershipId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string DiscountText { get; set; } = string.Empty;

        public string CardColor { get; set; } = string.Empty;

        public ObservableCollection<string> AddonBenefits { get; set; }

        public MembershipDisplayModel(Membership memebershipToDisplay)
        {
            MembershipId = memebershipToDisplay.Id;
            Name = memebershipToDisplay.Name;
            DiscountText = $"{memebershipToDisplay.FlightDiscountPercentage}% Off Flights";

            CardColor = Name.ToLower() switch
            {
                "bronze" => BronzeMembershipColor,
                "silver" => SilverMembershipColor,
                "gold" => GoldMembershipColor,
                _ => DefaultMembershipColor
            };

            AddonBenefits = new ObservableCollection<string>();
            if (memebershipToDisplay.AddonDiscounts != null)
            {
                foreach (var discount in memebershipToDisplay.AddonDiscounts)
                {
                    AddonBenefits.Add($"• {discount.DiscountPercentage}% Off {discount.AddOn.Name}");
                }
            }
        }
    }

    public class MembershipViewModel : ViewModelBase
    {
        private readonly IMembershipService membershipService;
        private readonly INavigationService navigationService;

        private string purchaseResultMessage = string.Empty;

        private bool? purchaseSucceeded;

        public MembershipViewModel(IMembershipService membershipService, INavigationService navigationService)
        {
            this.membershipService = membershipService;
            this.navigationService = navigationService;
            this.Memberships = new ObservableCollection<MembershipDisplayModel>();

            this.PurchaseCommand = new RelayCommand(async parameter => await this.ExecutePurchaseAsync(parameter));

            _ = this.LoadMembershipsAsync();
        }

        public ObservableCollection<MembershipDisplayModel> Memberships { get; set; }

        public string PurchaseResultMessage
        {
            get => this.purchaseResultMessage;
            set
            {
                this.purchaseResultMessage = value;
                this.OnPropertyChanged();
            }
        }

        public bool? PurchaseSucceeded
        {
            get => this.purchaseSucceeded;
            set
            {
                this.purchaseSucceeded = value;
                this.OnPropertyChanged();
            }
        }

        public ICommand PurchaseCommand { get; }

        private async Task LoadMembershipsAsync()
        {
            var memberships = await this.membershipService.GetAllMembershipsAsync();
            foreach (var memebershipEntity in memberships)
            {
                this.Memberships.Add(new MembershipDisplayModel(memebershipEntity));
            }
        }

        private async Task ExecutePurchaseAsync(object? parameter)
        {
            this.PurchaseSucceeded = null;
            this.PurchaseResultMessage = string.Empty;

            if (UserSession.CurrentUser == null)
            {
                this.navigationService.NavigateTo(typeof(View.AuthPage));
                return;
            }

            if (parameter is not int membershipId)
            {
                return;
            }

            var result = await this.membershipService.PurchaseMembershipAsync(UserSession.CurrentUser.Id, membershipId);
            if (result != null)
            {
                this.PurchaseSucceeded = result.Succeeded;
                this.PurchaseResultMessage = result.Message;
            }
        }
    }
}
