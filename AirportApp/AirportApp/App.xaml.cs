using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using AutoMapper;
using AirportApp.Src;
using AirportApp.Src.Dto;
using AirportApp.Src.Dto.MappingProfiles;
using AirportApp.Src.Model;
using AirportApp.Src.Model.Chats;
using AirportApp.Src.Model.Review;
using AirportApp.Src.Model.Employee;
using AirportApp.Src.Repository;
using AirportApp.Src.Repository.Implementation;
using AirportApp.Src.Repository.Interfaces;
using AirportApp.Src.Service;
using AirportApp.Src.Service.Bot;
using AirportApp.Src.Service.Bot.Strategy;
using AirportApp.Src.Service.Implementation;
using AirportApp.Src.Service.Interfaces;
using AirportApp.Src.ViewModel;
using AirportApp.Src.ViewModel;
using AirportApp.Src.ViewModel.Chats;
using AirportApp.Src.ViewModel.Faq;
using AirportApp.Src.ViewModel.General;
using AirportApp.Src.ViewModel.Review;
using AirportApp.Src.Repository.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using AirportApp.Src.ViewModel.General;
using AirportApp.Src.ViewModel;
using AirportApp.Src.Repository.Interfaces;
using AirportApp.Src.Service.Interfaces;
using AirportApp.Src.Model.Review;
using AirportApp.Src.Model.Faq.Bot;
using AirportApp.Src.Model.Message;

namespace AirportApp
{
    public partial class App : Application
    {
        public IServiceProvider Services { get; }
        private Window? window;
        public User User { get; private set; }
        public Employee Employee { get; private set; }
        public bool IsEmployee = false;

        public App()
        {
            Services = ConfigureServices();
            InitializeComponent();
        }

        /// <summary>
        /// Attempts to find and set the active user or employee.
        /// Returns true if the ID was found; otherwise, false.
        /// </summary>
        // Updated SetUser in App.xaml.cs
        public bool SetUser(int userId)
        {
            User = null;
            Employee = null;

            try
            {
                if (IsEmployee)
                {
                    Employee = Services.GetService<IEmployeeService>().GetEmployeeById(userId);
                    return Employee != null;
                }
                else
                {
                    User = Services.GetService<IUserService>().GetById(userId);
                    return User != null;
                }
            }
            catch (KeyNotFoundException)
            {
                return false; // Safely return false so the UI shows the error
            }
        }

        private static IServiceProvider ConfigureServices()
        {
            DotNetEnv.Env.Load(System.IO.Path.Combine(AppContext.BaseDirectory, ".env"));

            var services = new ServiceCollection();
            services.AddAutoMapper(
                typeof(UserMappingProfile).Assembly,
                typeof(EmployeeMappingProfile).Assembly,
                typeof(MessageMappingProfile).Assembly,
                typeof(FAQEntryMappingProfile).Assembly,
                typeof(ReviewMappingProfile).Assembly,
                typeof(TicketMappingProfile).Assembly);

            services.AddSingleton<DecisionTreeRepository>();
            services.AddSingleton<IRepository<int, FAQNode>>(provider => provider.GetRequiredService<DecisionTreeRepository>());
            services.AddTransient<IBotStrategy, DecisionTreeStrategy>(); // I am not sure this is the way to do it :(
            services.AddTransient<BotEngine>();

            services.AddSingleton<MessageDatabaseRepository>();
            services.AddSingleton<IRepository<int, Message>>(provider => provider.GetRequiredService<MessageDatabaseRepository>());
            services.AddSingleton<MessageService>();

            services.AddSingleton<ChatDatabaseRepository>();
            services.AddSingleton<IRepository<int, Chat>>(provider => provider.GetRequiredService<ChatDatabaseRepository>());
            services.AddSingleton<ChatService>();

            services.AddSingleton<ReviewRepository>();
            services.AddSingleton<IRepository<int, Review>>(provider => provider.GetRequiredService<ReviewRepository>());
            services.AddSingleton<ReviewService>();

            services.AddSingleton<IEmployeeRepository, EmployeeRepository>();
            services.AddSingleton<IEmployeeService, EmployeeService>();

            services.AddSingleton<UserRepository>();
            services.AddSingleton<IUserRepository>(provider => provider.GetRequiredService<UserRepository>());
            services.AddSingleton<IRepository<int, User>>(provider => provider.GetRequiredService<UserRepository>());

            services.AddSingleton<IUserService, UserService>();

            services.AddTransient<LandingViewModel>();
            services.AddTransient<AllReviewsViewModel>();
            services.AddTransient<AddReviewViewModel>();
            services.AddTransient<ChatViewModel>();

            // Register the ViewModel
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

            return services.BuildServiceProvider();
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
