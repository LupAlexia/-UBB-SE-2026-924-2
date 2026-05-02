using System;
using System.Collections.Generic;
using AutoMapper;
using AirportApp.Src;
using AirportApp.Src.Dto.MappingProfiles;
using AirportApp.Src.Model;
using AirportApp.Src.Model.Chats;
using AirportApp.Src.Model.Review;
using AirportApp.Src.Model.Employee;
using AirportApp.Src.Repository;
using AirportApp.Src.Repository.Implementation;
using AirportApp.Src.Repository.Interfaces;
using AirportApp.Src.Repository.Database;
using AirportApp.Src.Service;
using AirportApp.Src.Service.Bot;
using AirportApp.Src.Service.Bot.Strategy;
using AirportApp.Src.Service.Implementation;
using AirportApp.Src.Service.Interfaces;
using AirportApp.Src.ViewModel;
using AirportApp.Src.ViewModel.Chats;
using AirportApp.Src.ViewModel.Faq;
using AirportApp.Src.ViewModel.General;
using AirportApp.Src.ViewModel.Review;
using AirportApp.Src.Model.Faq.Bot;
using AirportApp.Src.Model.Message;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AirportApp
{
    public partial class App : Application
    {
        public IServiceProvider Services { get; }
        public Window? window;
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
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(
                System.IO.Path.Combine(AppContext.BaseDirectory, "crash.txt"),
                ex.ToString()
            );
                throw;
            }

            InitializeComponent();
        }
        

        public bool SetUser(int userId)
        {
            User = null;
            Employee = null;
            try
            {
                if (IsEmployee)
                {
                    Employee = Services.GetService<IEmployeeService>()!.GetEmployeeById(userId);
                    return Employee != null;
                }
                else
                {
                    User = Services.GetService<IUserService>()!.GetById(userId);
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
            DotNetEnv.Env.Load(System.IO.Path.Combine(AppContext.BaseDirectory, ".env"));

            var services = new ServiceCollection();
            services.AddLogging();

            // Înlocuiește cu asta:
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<UserMappingProfile>();
                cfg.AddProfile<EmployeeMappingProfile>();
                cfg.AddProfile<MessageMappingProfile>();
                cfg.AddProfile<FAQEntryMappingProfile>();
                cfg.AddProfile<ReviewMappingProfile>();
                cfg.AddProfile<TicketMappingProfile>();
            });

            // --- Servicii Mystery Inc (Customer Support) ---
            services.AddSingleton<DecisionTreeRepository>();
            services.AddSingleton<IRepository<int, FAQNode>>(p => p.GetRequiredService<DecisionTreeRepository>());
            services.AddTransient<IBotStrategy, DecisionTreeStrategy>();
            services.AddTransient<BotEngine>();

            services.AddSingleton<MessageDatabaseRepository>();
            services.AddSingleton<IRepository<int, Message>>(p => p.GetRequiredService<MessageDatabaseRepository>());
            services.AddSingleton<MessageService>();

            services.AddSingleton<ChatDatabaseRepository>();
            services.AddSingleton<IRepository<int, Chat>>(p => p.GetRequiredService<ChatDatabaseRepository>());
            services.AddSingleton<ChatService>();

            services.AddSingleton<ReviewRepository>();
            services.AddSingleton<IRepository<int, Review>>(p => p.GetRequiredService<ReviewRepository>());
            services.AddSingleton<ReviewService>();

            services.AddSingleton<IEmployeeRepository, EmployeeRepository>();
            services.AddSingleton<IEmployeeService, EmployeeService>();

            services.AddSingleton<UserRepository>();
            services.AddSingleton<IUserRepository>(p => p.GetRequiredService<UserRepository>());
            services.AddSingleton<IRepository<int, User>>(p => p.GetRequiredService<UserRepository>());
            services.AddSingleton<IUserService, UserService>();

            services.AddTransient<LandingViewModel>();
            services.AddTransient<AllReviewsViewModel>();
            services.AddTransient<AddReviewViewModel>();
            services.AddTransient<ChatViewModel>();
            services.AddTransient<UpperBarViewModel>();

            services.AddSingleton<ITicketRepository, TicketRepository>();
            services.AddSingleton<ITicketCategoryRepository, TicketCategoryRepository>();
            services.AddSingleton<ITicketSubcategoryRepository, TicketSubcategoryRepository>();
            services.AddSingleton<ITicketService, TicketService>();
            services.AddSingleton<ITicketCategoryService, TicketCategoryService>();
            services.AddSingleton<ITicketSubcategoryService, TicketSubcategoryService>();
            services.AddTransient<TicketsViewModel>();

            services.AddSingleton<IFAQRepository, FAQRepository>();
            services.AddSingleton<IFAQService, FAQService>();
            services.AddTransient<FAQViewModel>();

            // --- Servicii CloudSpritzers (Flight Tickets) ---
            services.AddSingleton<IDatabaseConnectionFactory, DatabaseConnectionFactory>();
            services.AddSingleton<IFlightRepository, FlightRepository>();
            services.AddSingleton<IFlightTicketRepository, FlightTicketRepository>();
            services.AddSingleton<IAddOnRepository, AddOnRepository>();
            services.AddSingleton<IMembershipRepository, MembershipRepository>();
            services.AddSingleton<IUser2Repository, User2Repository>();
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

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            window = new MainWindow();
            var frame = new Frame();
            frame.Navigate(typeof(AirportApp.Src.View.General.ChoosingPage));
            window.Content = frame;
            window.Activate();
        }
    }
}