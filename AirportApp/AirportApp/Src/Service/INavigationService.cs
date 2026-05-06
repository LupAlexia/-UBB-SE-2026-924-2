using System;

namespace AirportApp.Src.Service
{
    /// <summary>
    /// Allows ViewModels to request page navigation without depending on WinUI Frame directly.
    /// This keeps ViewModels testable and free of UI framework references.
    /// </summary>
    public interface INavigationService
    {
        bool CanGoBack { get; }
        
        void Initialize(Microsoft.UI.Xaml.Controls.Frame frame);

        void NavigateTo(Type pageType, object? parameter = null);

        void GoBack();
    }
}
