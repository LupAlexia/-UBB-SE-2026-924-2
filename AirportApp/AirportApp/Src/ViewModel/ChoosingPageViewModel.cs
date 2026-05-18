using CommunityToolkit.Mvvm.ComponentModel;

namespace AirportApp.Src.ViewModel
{
    public class ChoosingPageViewModel
    {
        public void SetUserRole(string roleTag)
        {
            bool isEmployee = roleTag == "Employee";

            var application = (App)Microsoft.UI.Xaml.Application.Current;
            application.IsEmployee = isEmployee;
        }
    }
}
