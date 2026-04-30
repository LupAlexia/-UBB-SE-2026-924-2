using AirportApp;
using Microsoft.UI.Xaml;
using AirportApp.Src.Repository;
using AirportApp.Src.Service;

namespace AirportApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// Also serves as the Composition Root: the single place where all concrete
    /// dependencies are wired together. Views retrieve what they need from here
    /// instead of constructing dependency chains themselves.
    /// </summary>
    public partial class App2 : Application
    {
        private Window window = null!;

        private static IDatabaseConnectionFactory databaseConnectionFactory = null!;

        private static IFlightRepository flightRepository = null!;
        private static IFlightTicketRepository ticketRepository = null!;
        private static IAddOnRepository addOnRepository = null!;
        private static IMembershipRepository membershipRepository = null!;
        private static IUser2Repository userRepository = null!;

        public static IAuthService AuthService { get; private set; } = null!;
        public static IFlightSearchService FlightSearchService { get; private set; } = null!;
        public static IBookingService BookingService { get; private set; } = null!;
        public static IPricingService PricingService { get; private set; } = null!;
        public static IDashboardService DashboardService { get; private set; } = null!;
        public static ICancellationService CancellationService { get; private set; } = null!;
        public static IMembershipService MembershipService { get; private set; } = null!;
        public static NavigationService NavigationService { get; private set; } = null!;

        public App2()
        {
            InitializeComponent();
            ConfigureServices();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs eventArgs)
        {
            window = new MainWindow2();
            window.Activate();
        }

        /// <summary>
        /// Wires up every concrete dependency once. Views and ViewModels never
        /// need to know about DatabaseConnectionFactory or concrete repository types.
        /// </summary>
        private static void ConfigureServices()
        {
            databaseConnectionFactory = new DatabaseConnectionFactory();

            flightRepository = new FlightRepository(databaseConnectionFactory);
            ticketRepository = new FlightTicketRepository(databaseConnectionFactory);
            addOnRepository = new AddOnRepository(databaseConnectionFactory);
            membershipRepository = new MembershipRepository(databaseConnectionFactory);
            userRepository = new User2Repository(databaseConnectionFactory, membershipRepository);

            AuthService = new AuthService(userRepository);
            FlightSearchService = new FlightSearchService(flightRepository);
            BookingService = new BookingService(ticketRepository, addOnRepository);
            PricingService = new PricingService();
            DashboardService = new DashboardService(ticketRepository);
            CancellationService = new CancellationService(ticketRepository);
            MembershipService = new MembershipService(userRepository, membershipRepository);
            NavigationService = new NavigationService();
        }
    }
}
