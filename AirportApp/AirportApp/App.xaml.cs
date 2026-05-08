using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Domain.Chats;
using AirportApp.ClassLibrary.Entity.Domain.Employee;
using AirportApp.ClassLibrary.Entity.Domain.Faq.Bot;
using AirportApp.ClassLibrary.Entity.Domain.Message;
using AirportApp.ClassLibrary.Entity.Domain.Review;
using AirportApp.ClassLibrary.Entity.Dto.MappingProfiles;
using AirportApp.ClassLibrary.Entity.Repository.Database;
using AirportApp.ClassLibrary.Repository;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service;
using AirportApp.Src.Service.Bot.Strategy;
using AirportApp.Src.Service.Implementation;
using AirportApp.Src.Service.Interfaces;
using AirportApp.Src.ViewModel;
using AirportApp.Src.ViewModel.Chats;
using AirportApp.Src.ViewModel.Faq;
using AirportApp.Src.ViewModel.General;
using AirportApp.Src.ViewModel.Review;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(
                System.IO.Path.Combine(AppContext.BaseDirectory, "crash.txt"),
                ex.ToString());
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
                    User = await Services.GetService<IUserService>() !.GetByIdAsync(userId);
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
                BaseAddress = new Uri("http://localhost:5253/")
            });

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
            // Register EF DbContext using connection string from appsettings.json
            services.AddDbContext<AirportDbContext>(options =>
            {
                var conn = configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(conn);
            }, contextLifetime: Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient, optionsLifetime: Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient);



            services.AddTransient<IBotStrategy, DecisionTreeStrategy>();
            services.AddTransient<BotEngineIdentity>();


            services.AddSingleton<IRepository<int, FAQNode>, DecisionTreeRepositoryProxy>();
            services.AddSingleton<IRepository<int, Chat>, ChatRepositoryProxy>();
            services.AddSingleton<IMessageRepository, MessageRepositoryProxy>();
            services.AddSingleton<IRepository<int, Message>>(p => p.GetRequiredService<IMessageRepository>());
            services.AddSingleton<ITicketCategoryRepository, ComplaintTicketCategoryRepositoryProxy>();
            services.AddSingleton<ITicketSubcategoryRepository, ComplaintTicketSubcategoryRepositoryProxy>();
            services.AddSingleton<ITicketRepository, ComplaintTicketRepositoryProxy>();
            services.AddSingleton<IUserRepository, UserRepositoryProxy>();
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
            services.AddSingleton(new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5253/")
            });
            services.AddSingleton<IFlightRepository, FlightRepository>();
            services.AddSingleton<IFlightTicketRepository, FlightTicketRepository>();
            services.AddSingleton<IAddOnRepository, AddOnRepository>();
            services.AddSingleton<IMembershipRepository, MembershipRepository>();
            services.AddSingleton<ICustomerRepository, CustomerRepository>();
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
            Window = new MainWindow();
            var frame = new Frame();
            frame.Navigate(typeof(AirportApp.Src.View.General.ChoosingPage));
            Window.Content = frame;
            Window.Activate();
        }
    }
}