using System;
using AirportApp.ClassLibrary.Entity.Domain;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;

namespace AirportApp.Src.ViewModel
{
    public sealed partial class UpperBarViewModel : ObservableObject
    {
        private readonly User user;
        private readonly Employee employee;

        public UpperBarViewModel(User? currentUser = null, Employee? currentEmployee = null)
        {
            // Access to application state is performed inside the ViewModel layer
            // user = ((App)App.Current).User;
            // employee = ((App)App.Current).Employee
            user = currentUser ?? (Application.Current is App app ? app.User : null);
            employee = currentEmployee ?? (Application.Current is App ap ? ap.Employee : null);

            // Determine if the current context is a client (user) or an employee
            isClientView = user != null;
        }

        private bool isClientView;
        public bool IsClientView
        {
            get => isClientView;
            private set
            {
                if (SetProperty(ref isClientView, value))
                {
                    // Notify dependent properties
                    OnPropertyChanged(nameof(UserDisplayLabel));
                }
            }
        }

        public string UserDisplayLabel
        {
            get
            {
                if (IsClientView && user != null)
                {
                    return $"ID: {user.RetrieveUniqueDatabaseIdentifierForBot()}";
                }

                if (!IsClientView && employee != null)
                {
                    return $"ID: {employee.RetrieveUniqueDatabaseIdentifierForBot()}";
                }

                return "Not signed in";
            }
        }

        // Navigation targets are resolved here so the view does not contain branching logic
        public Type LandingPageType => typeof(AirportApp.Src.View.General.LandingPage);
        public Type FAQPageType => typeof(AirportApp.Src.View.Faq.FAQPage);
        public Type ChatPageType => typeof(AirportApp.Src.View.Chat.ChatPage);
        public Type TicketsPageType => IsClientView
            ? typeof(AirportApp.Src.View.Ticket.ComplaintTicketPage)
            : typeof(AirportApp.Src.View.Ticket.TicketEmployeePage);
        public Type ReviewsPageType => IsClientView
            ? typeof(AirportApp.Src.View.Review.ReviewPage)
            : typeof(AirportApp.Src.View.Review.EmployeeSeeReviewsPage);
        public Type ChoosingPageType => typeof(AirportApp.Src.View.General.ChoosingPage);
    }
}
