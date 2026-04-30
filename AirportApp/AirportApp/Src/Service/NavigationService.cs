using System;
using Microsoft.UI.Xaml.Controls;

namespace AirportApp.Src.Service
{
    /// <summary>
    /// Concrete navigation service that wraps the WinUI Frame.
    /// Lives in the Service layer but depends on WinUI — this is acceptable because
    /// it's the single place where the UI framework is referenced for navigation.
    /// ViewModels only see the INavigationService interface.
    /// </summary>
    public class NavigationService : INavigationService
    {
        private Frame? frame;

        public bool CanGoBack => this.frame?.CanGoBack ?? false;

        public void Initialize(Frame frame)
        {
            this.frame = frame ?? throw new ArgumentNullException(nameof(frame));
        }

        public void NavigateTo(Type pageType, object? parameter = null)
        {
            if (this.frame == null)
            {
                throw new InvalidOperationException("NavigationService has not been initialized with a Frame.");
            }

            this.frame.Navigate(pageType, parameter);
        }

        public void GoBack()
        {
            if (this.frame != null && this.frame.CanGoBack)
            {
                this.frame.GoBack();
            }
        }
    }
}
