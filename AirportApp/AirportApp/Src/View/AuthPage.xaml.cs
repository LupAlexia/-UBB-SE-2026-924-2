using System;
using System.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using AirportApp.Src.ViewModel;

namespace AirportApp.Src.View
{
    public sealed partial class AuthPage : Page
    {
        public AuthViewModel ViewModel { get; }

        public AuthPage()
        {
            this.InitializeComponent();

            ViewModel = new AuthViewModel(App.AuthService, App.NavigationService);
            this.DataContext = ViewModel;

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private async void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs eventArgs)
        {
            if (eventArgs.PropertyName == nameof(ViewModel.ErrorMessage) &&
                !string.IsNullOrWhiteSpace(ViewModel.ErrorMessage))
            {
                var dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = ViewModel.ErrorMessage,
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await dialog.ShowAsync();
            }
            else if (eventArgs.PropertyName == nameof(ViewModel.SuccessMessage) &&
                     !string.IsNullOrWhiteSpace(ViewModel.SuccessMessage) &&
                     !ViewModel.IsAuthenticated)
            {
                var dialog = new ContentDialog
                {
                    Title = "Success",
                    Content = ViewModel.SuccessMessage,
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await dialog.ShowAsync();
            }
        }

        private void Password_PasswordChanged(object? sender, Microsoft.UI.Xaml.RoutedEventArgs eventArgs)
        {
            ViewModel.PasswordText = passwordInput.Password;
        }
    }
}




