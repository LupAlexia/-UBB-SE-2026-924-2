using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Proxy.ServiceProxies;
using AirportApp.ClassLibrary.Service.Interfaces;
using AirportApp.Src.Service;
using AirportApp.Src.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AirportApp
{
    public partial class App : Application
    {
        public IServiceProvider Services { get; }
        public Window? Window;
        public User? User { get; private set; }
        public Employee? Employee { get; private set; }
        public bool IsEmployee = false;

        public static IAuthService AuthService { get; private set; } = null!;
        public static IFlightSearchService FlightSearchService { get; private set; } = null!;
        public static IBookingService BookingService { get; private set; } = null!;
        public static IPricingService PricingService { get; private set; } = null!;
        public static IDashboardService DashboardService { get; private set; } = null!;
        public static ICancellationService CancellationService { get; private set; } = null!;
        public static IMembershipService MembershipService { get; private set; } = null!;
        public static NavigationService NavigationService { get; private set; } = null!;

        public App()
        {
            try
            {
                Services = ConfigureServices();
            }
            catch (Exception exception)
            {
                System.IO.File.WriteAllText(
                System.IO.Path.Combine(AppContext.BaseDirectory, "crash.txt"),
                exception.ToString());
                throw;
            }

            InitializeComponent();
        }

        public async Task<bool> SetUserAsync(int userId)
        {
            User = null;
            Employee = null;
            App.AuthService.Logout();

            try
            {
                if (IsEmployee)
                {
                    Employee = await Services.GetService<IEmployeeService>()!.GetEmployeeByIdAsync(userId);
                    return Employee != null;
                }
                else
                {
                    var userService = Services.GetService<IUserService>();
                    User = await userService!.GetByIdAsync(userId);

                    if (User == null && userId > 0 && userId < 100)
                    {
                        User = await userService.GetByIdAsync(userId + 100);
                    }

                    return User != null;
                }
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddLogging();

            services.AddSingleton<HttpClient>(serviceProvider => new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5253/")
            });

            services.AddAutoMapper(mapperConfiguration =>
            {
                mapperConfiguration.AddProfile<UserMappingProfile>();
                mapperConfiguration.AddProfile<EmployeeMappingProfile>();
                mapperConfiguration.AddProfile<MessageMappingProfile>();
                mapperConfiguration.AddProfile<FAQEntryMappingProfile>();
                mapperConfiguration.AddProfile<ReviewMappingProfile>();
                mapperConfiguration.AddProfile<TicketMappingProfile>();
            });

            services.AddSingleton<IAuthService, AuthServiceProxy>();
            services.AddSingleton<IBookingService, BookingServiceProxy>();
            services.AddSingleton<ICancellationService, CancellationServiceProxy>();
            services.AddSingleton<IChatService, ChatServiceProxy>();
            services.AddSingleton<IComplaintTicketService, ComplaintTicketServiceProxy>();
            services.AddSingleton<IComplaintTicketCategoryService, ComplaintTicketCategoryServiceProxy>();
            services.AddSingleton<IComplaintTicketSubcategoryService, ComplaintTicketSubcategoryServiceProxy>();
            services.AddSingleton<IDashboardService, DashboardServiceProxy>();
            services.AddSingleton<IDecisionTreeService, DecisionTreeServiceProxy>();
            services.AddSingleton<IEmployeeService, EmployeeServiceProxy>();
            services.AddSingleton<IFAQService, FAQServiceProxy>();
            services.AddSingleton<IFlightSearchService, FlightSearchServiceProxy>();
            services.AddSingleton<IMembershipService, MembershipServiceProxy>();
            services.AddSingleton<IMessageService, MessageServiceProxy>();
            services.AddSingleton<IPricingService, PricingServiceProxy>();
            services.AddSingleton<IReviewService, ReviewServiceProxy>();
            services.AddSingleton<IUserService, UserServiceProxy>();

            services.AddSingleton<NavigationService>();

            services.AddTransient<LandingViewModel>();
            services.AddTransient<AllReviewsViewModel>();
            services.AddTransient<AddReviewViewModel>();
            services.AddTransient<ChatViewModel>();
            services.AddTransient<UpperBarViewModel>();
            services.AddTransient<TicketsViewModel>();
            services.AddTransient<FAQViewModel>();

            var provider = services.BuildServiceProvider();

            AuthService = provider.GetRequiredService<IAuthService>();
            FlightSearchService = provider.GetRequiredService<IFlightSearchService>();
            BookingService = provider.GetRequiredService<IBookingService>();
            PricingService = provider.GetRequiredService<IPricingService>();
            DashboardService = provider.GetRequiredService<IDashboardService>();
            CancellationService = provider.GetRequiredService<ICancellationService>();
            MembershipService = provider.GetRequiredService<IMembershipService>();
            NavigationService = provider.GetRequiredService<NavigationService>();

            return provider;
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs launchActivatedEventArgs)
        {
            Window = new MainWindow();
            var frame = new Frame();
            frame.Navigate(typeof(AirportApp.Src.View.General.ChoosingPage));
            Window.Content = frame;
            Window.Activate();
        }
    }
}