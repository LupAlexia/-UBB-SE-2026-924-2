using System;
using System.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using AirportApp.Src.ViewModel;

namespace AirportApp.Src.View
{
    public sealed partial class MembershipsPage : Page
    {
        public MembershipViewModel ViewModel { get; }

        public MembershipsPage()
        {
            this.InitializeComponent();

            ViewModel = new MembershipViewModel(App.MembershipService, App.NavigationService);
            this.DataContext = ViewModel;

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private async void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs eventArgs)
        {
            if (eventArgs.PropertyName != nameof(ViewModel.PurchaseSucceeded) || ViewModel.PurchaseSucceeded == null)
            {
                return;
            }

            var dialog = new ContentDialog
            {
                Title = ViewModel.PurchaseSucceeded == true ? "Membership updated" : "Purchase failed",
                Content = ViewModel.PurchaseResultMessage,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };

            await dialog.ShowAsync();
        }
    }
}




