using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Repository.Database;
using AirportApp.ClassLibrary.Repository;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service;
using AirportApp.Src.Service.Bot.Strategy;
using AirportApp.Src.Service.Implementation;
using AirportApp.Src.Service.Interfaces;
using AirportApp.Src.ViewModel;
using Microsoft.EntityFrameworkCore;
using AirportApp.Src.ViewModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.Src.Proxy;
using AirportApp.ClassLibrary.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace AirportApp
{
    public partial class App : Application
    {
        public IServiceProvider Services { get; }
        public Window? Window;
        public User? User { get; private set; }
        public Employee? Employee { get; private set; }
        public bool IsEmployee = false;

        // Static services din App2 (CloudSpritzers) — accesibile global
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
            try
            {
                if (IsEmployee)
                {
                    Employee = await Services.GetService<IEmployeeService>() !.GetEmployeeByIdAsync(userId);
                    return Employee != null;
                }
                else
                {
                    var userService = Services.GetService<IUserService>();
                    User = await userService !.GetByIdAsync(userId);

                    // Temporary compatibility: seeded user IDs may be offset to avoid TPH key collisions.
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
            var configuration = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .AddJsonFile(System.IO.Path.Combine(AppContext.BaseDirectory, "appsettings.json"), optional: false, reloadOnChange: true)
                .Build();

            var services = new ServiceCollection();
            services.AddLogging();

            services.AddSingleton<HttpClient>(sp => new HttpClient
            {
                BaseAddress = new Uri("http://172.30.246.135")
                // BaseAddress = new Uri("http://localhost:5253/")
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

            // --- Servicii Mystery Inc (Customer Support) ---
            // Register EF DbContext using connection string from appsettings.json
            services.AddDbContext<AirportDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(connectionString);
            }, contextLifetime: Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient, optionsLifetime: Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient);

            services.AddTransient<IBotStrategy, DecisionTreeStrategy>();
            services.AddTransient<BotEngineIdentity>();

            services.AddSingleton<IRepository<int, FAQNode>, DecisionTreeRepositoryProxy>();
            services.AddSingleton<IRepository<int, Chat>, ChatRepositoryProxy>();
            services.AddSingleton<IMessageRepository, MessageRepositoryProxy>();
            services.AddSingleton<IRepository<int, Message>>(serviceProvider => serviceProvider.GetRequiredService<IMessageRepository>());
            services.AddSingleton<ITicketCategoryRepository, ComplaintTicketCategoryRepositoryProxy>();
            services.AddSingleton<ITicketSubcategoryRepository, ComplaintTicketSubcategoryRepositoryProxy>();
            services.AddSingleton<ITicketRepository, ComplaintTicketRepositoryProxy>();

            services.AddSingleton<UserRepositoryProxy>();
            services.AddSingleton<IUserRepository>(serviceProvider => serviceProvider.GetRequiredService<UserRepositoryProxy>());
            services.AddSingleton<IRepository<int, User>>(serviceProvider => serviceProvider.GetRequiredService<UserRepositoryProxy>());

            services.AddSingleton<IEmployeeRepository, EmployeeRepositoryProxy>();
            services.AddSingleton<IFAQRepository, FAQRepositoryProxy>();
            services.AddSingleton<IRepository<int, Review>, ReviewRepositoryProxy>();

            services.AddSingleton<IEmployeeService, EmployeeService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IMessageService, MessageService>();
            services.AddSingleton<IComplaintTicketService, ComplaintTicketService>();
            services.AddSingleton<IComplaintTicketCategoryService, ComplaintTicketCategoryService>();
            services.AddSingleton<IComplaintTicketSubcategoryService, ComplaintTicketSubcategoryService>();
            services.AddSingleton<IFAQService, FAQService>();
            services.AddSingleton<IReviewService, ReviewService>();
            services.AddSingleton<IChatService, ChatService>();

            services.AddTransient<LandingViewModel>();
            services.AddTransient<AllReviewsViewModel>();
            services.AddTransient<AddReviewViewModel>();
            services.AddTransient<ChatViewModel>();
            services.AddTransient<UpperBarViewModel>();
            services.AddTransient<TicketsViewModel>();
            services.AddTransient<FAQViewModel>();

            // --- Servicii CloudSpritzers (Flight Tickets) ---
            services.AddSingleton<IFlightRepository, FlightRepositoryProxy>();
            services.AddSingleton<IFlightTicketRepository, FlightTicketRepositoryProxy>();
            services.AddSingleton<IAddOnRepository, AddOnRepositoryProxy>();
            services.AddSingleton<IMembershipRepository, MembershipRepositoryProxy>();
            services.AddSingleton<ICustomerRepository, CustomerRepositoryProxy>();
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IFlightSearchService, FlightSearchService>();
            services.AddSingleton<IBookingService, BookingService>();
            services.AddSingleton<IPricingService, PricingService>();
            services.AddSingleton<IDashboardService, DashboardService>();
            services.AddSingleton<ICancellationService, CancellationService>();
            services.AddSingleton<IMembershipService, MembershipService>();
            services.AddSingleton<NavigationService>();

            var provider = services.BuildServiceProvider();

            // Initializare servicii statice (compatibilitate cu codul CloudSpritzers existent)
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