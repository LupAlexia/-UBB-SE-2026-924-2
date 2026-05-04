using System;
using TicketManager.Service;

namespace TicketManager.View
{
    public class FlightSearchPage
    {
    }
    public class BookingPage
    {
    }
    public class AuthPage
    {
    }
    public class RegisterPage
    {
    }
    public class LoginPage
    {
    }
    public class DashboardPage
    {
    }
    public class MembershipsPage
    {
    }
}

namespace TicketManager.Service
{
    public class NavigationService : INavigationService
    {
        public void NavigateTo(Type pageType, object? parameter = null)
        {
        }
        public void GoBack()
        {
        }
        public bool CanGoBack => false;
    }
}
