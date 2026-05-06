using System;
using Microsoft.UI.Xaml.Controls;
using AirportApp.Src.Service;

namespace AirportApp.Src.Proxy
{
    public class NavigationServiceProxy : INavigationService
    {
        private readonly NavigationService realNavigationService;

        public NavigationServiceProxy(NavigationService realNavigationService)
        {
            this.realNavigationService = realNavigationService ?? throw new ArgumentNullException(nameof(realNavigationService));
        }

        public bool CanGoBack => this.realNavigationService.CanGoBack;

        // Pure logic — UI frame initialization, no DB access
        public void Initialize(Frame frame)
        {
            this.realNavigationService.Initialize(frame);
        }

        // Pure logic — UI navigation, no DB access
        public void NavigateTo(Type pageType, object? parameter = null)
        {
            this.realNavigationService.NavigateTo(pageType, parameter);
        }

        // Pure logic — UI navigation, no DB access
        public void GoBack()
        {
            this.realNavigationService.GoBack();
        }
    }
}
