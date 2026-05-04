using AirportApp;
using System;
using System.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using AirportApp.Src.ViewModel;

namespace AirportApp.Src.View
{
    public sealed partial class DashboardPage : Page
    {
        private readonly DashboardViewModel viewModel;

        public DashboardPage()
        {
            this.InitializeComponent();

            viewModel = new DashboardViewModel(App.DashboardService, App.CancellationService, App.NavigationService);
            this.DataContext = viewModel;

            viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs eventArgs)
        {
            base.OnNavigatedTo(eventArgs);
            await viewModel.OnNavigatedToAsync();
        }

        private async void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs eventArgs)
        {
            if (eventArgs.PropertyName == nameof(viewModel.CancellationSucceeded) &&
                viewModel.CancellationSucceeded == false)
            {
                var errorDialog = new ContentDialog
                {
                    Title = "Cannot cancel",
                    Content = viewModel.CancellationMessage,
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await errorDialog.ShowAsync();
            }

            if (eventArgs.PropertyName == nameof(viewModel.CancellationSucceeded) &&
                viewModel.CancellationSucceeded == true)
            {
                var resultDialog = new ContentDialog
                {
                    Title = "FlightTicket cancelled",
                    Content = viewModel.CancellationMessage,
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await resultDialog.ShowAsync();
            }

            if (eventArgs.PropertyName == nameof(viewModel.PendingCancelTicket) &&
                viewModel.PendingCancelTicket != null)
            {
                var dialog = new ContentDialog
                {
                    Title = "Cancel FlightTicket",
                    Content = $"Are you sure you want to cancel FlightTicket #{viewModel.PendingCancelTicket.Id}?",
                    PrimaryButtonText = "Yes, cancel",
                    CloseButtonText = "No",
                    XamlRoot = this.XamlRoot
                };

                var dialogResult = await dialog.ShowAsync();
                if (dialogResult == ContentDialogResult.Primary)
                {
                    await viewModel.ConfirmCancellationAsync();
                }
                else
                {
                    viewModel.DeclineCancellation();
                }
            }
        }
    }
}





