using System;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.Src.Service;

namespace AirportApp.Src.View
{
    public sealed partial class TravelPage : Page
    {
        private const string AccountNavTag = "Account";
        private const string HomeNavTag = "Home";
        private readonly IAuthService authService;

        public TravelPage()
        {
            InitializeComponent();
            authService = App.AuthService;
            App.NavigationService.Initialize(ContentFrame);
            ContentFrame.Navigated += ContentFrame_Navigated;

            // Enforce authentication when entering the Flights section
            if (UserSession.CurrentUser == null)
            {
                NavigateTo(typeof(AuthPage));
            }
            else
            {
                NavigateToSearch();
            }

            UpdateNavigationAvailability();
            TopNav.SelectedItem = null;
        }

        private void UpdateNavigationAvailability()
        {
            bool isAuthenticated = UserSession.CurrentUser != null;
            foreach (var item in TopNav.MenuItems.OfType<NavigationViewItem>())
            {
                string tag = item.Tag?.ToString() ?? string.Empty;
                bool isSearchItem = tag.EndsWith("FlightSearchPage", StringComparison.OrdinalIgnoreCase);
                bool isHomeItem = tag == HomeNavTag;
                
                // Allow home or search items, and only enable other options if authenticated
                item.IsEnabled = isAuthenticated || isHomeItem;
            }
            if (AccountMenuItem != null)
                AccountMenuItem.IsEnabled = true;
        }

        private void NavigateTo(Type pageType)
        {
            if (ContentFrame.CurrentSourcePageType != pageType)
                ContentFrame.Navigate(pageType);
        }

        private void NavigateToSearch() => NavigateTo(typeof(FlightSearchPage));

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (TopNav?.MenuItems == null) return;
            UpdateNavigationAvailability();

            bool itemFound = false;
            string pageName = e.SourcePageType.Name;
            foreach (NavigationViewItem item in TopNav.MenuItems.OfType<NavigationViewItem>())
            {
                if (item.Tag?.ToString()?.EndsWith(pageName, StringComparison.OrdinalIgnoreCase) == true)
                {
                    TopNav.SelectedItem = item;
                    itemFound = true;
                    break;
                }
            }
            // If they are on AuthPage, don't select any menu items
            if (!itemFound || pageName == "AuthPage")
                TopNav.SelectedItem = null;
        }

        private async void TopNav_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs e)
        {
            UpdateNavigationAvailability();
            var tag = e.InvokedItemContainer?.Tag?.ToString();

            // Home Button -> back to UserHomePage hub
            if (tag == HomeNavTag)
            {
                var rootFrame = (Frame)((MainWindow)((App)Application.Current).Window).Content;
                rootFrame.Navigate(typeof(General.UserHomePage));
                return;
            }

            if (tag == AccountNavTag)
            {
                var currentUser = UserSession.CurrentUser;
                if (currentUser == null)
                {
                    NavigateTo(typeof(AuthPage));
                    return;
                }

                string membershipTier = string.IsNullOrWhiteSpace(currentUser.Membership?.Name)
                    ? "None"
                    : currentUser.Membership.Name;

                var dialog = new ContentDialog
                {
                    Title = "Account",
                    Content = $"Email: {currentUser.Email}\nUsername: {currentUser.Username}\nMembership tier: {membershipTier}",
                    PrimaryButtonText = "Sign out",
                    CloseButtonText = "Close",
                    XamlRoot = ContentFrame.XamlRoot
                };

                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    authService.Logout();
                    UpdateNavigationAvailability();
                    NavigateTo(typeof(AuthPage));
                }
                return;
            }

            if (!string.IsNullOrEmpty(tag))
            {
                Type? pageType = Type.GetType(tag);
                if (pageType != null)
                {
                    if (UserSession.CurrentUser == null && pageType != typeof(AuthPage))
                    {
                        NavigateTo(typeof(AuthPage));
                        return;
                    }
                    NavigateTo(pageType);
                }
            }
        }

        private void TopNav_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs e)
        {
            if (ContentFrame.CanGoBack)
                ContentFrame.GoBack();
        }
    }
}