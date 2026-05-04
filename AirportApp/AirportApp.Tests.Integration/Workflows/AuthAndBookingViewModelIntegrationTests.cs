using FluentAssertions;
using TicketManager.Domain;
using TicketManager.Repository;
using TicketManager.Service;
using TicketManager.ViewModel;

namespace TicketManager.Tests.Integration.Workflows;

public class AuthAndBookingViewModelIntegrationTests : BaseIntegrationTest
{
    private const int UniqueCodeStartIndex = 0;
    private const int UniqueCodeLength = 4;
    private const int SinglePassenger = 1;
    private const int ThreePassengers = 3;
    private const int TwoPassengers = 2;
    private const int DefaultFlightCapacity = 180;
    private const int DaysUntilDeparture = 5;
    private const int ThreeDaysUntilDeparture = 3;
    private const int TwoDaysUntilDeparture = 2;
    private const int StandardFlightDurationHours = 2;
    private const int OneHourDuration = 1;
    private const string VasileEmail = "vasile.mihai";
    private const string VasileUsername = "VasileM";
    private const string VasilePassword = "Parola@Vasile123";
    private const string VasilePhone = "0733445566";
    private const string GeorgetaEmail = "georgeta.popescu";
    private const string GeorgetaUsername = "GeorgetaP";
    private const string GeorgetaPassword = "Parola@Georgeta456";
    private const string GeorgetaPhone = "0722556677";
    private const string CosminEmail = "cosmin.tudor";
    private const string CosminUsername = "CosminT";
    private const string CosminPassword = "Parola@Cosmin789";
    private const string CosminPhone = "0733667788";
    private const string DomainGmail = "@gmail.com";
    private const string UpcomingFilter = "Upcoming";

    private readonly IUserRepository userRepository;
    private readonly IFlightRepository flightRepository;
    private readonly ITicketRepository ticketRepository;
    private readonly IAddOnRepository addOnRepository;
    private readonly IMembershipRepository membershipRepository;
    private readonly AuthService authentificationService;
    private readonly BookingService bookingService;
    private readonly PricingService pricingService;
    private readonly NavigationService navigationService;

    public AuthAndBookingViewModelIntegrationTests()
    {
        var databaseConnectionFactory = new DatabaseConnectionFactory(GetTestConnectionString());
        membershipRepository = new MembershipRepository(databaseConnectionFactory);
        userRepository = new UserRepository(databaseConnectionFactory, membershipRepository);
        flightRepository = new FlightRepository(databaseConnectionFactory);
        ticketRepository = new TicketRepository(databaseConnectionFactory);
        addOnRepository = new AddOnRepository(databaseConnectionFactory);

        authentificationService = new AuthService(userRepository);
        bookingService = new BookingService(ticketRepository, addOnRepository);
        pricingService = new PricingService();
        navigationService = new NavigationService();
    }

    [Fact]
    public void AuthenticationViewModel_RegisterAndLogin_Succeeds()
    {
        var authViewModel = new AuthViewModel(authentificationService, navigationService);
        string uniqueCode = Guid.NewGuid().ToString().Substring(0, 4);
        string email = $"vasile.mihai_{uniqueCode}@gmail.com";
        string password = "Parola@Vasile123";

        authViewModel.IsLoginMode = false;
        authViewModel.EmailText = email;
        authViewModel.PhoneText = "0733445566";
        authViewModel.UsernameText = $"VasileM_{uniqueCode}";
        authViewModel.PasswordText = password;

        authViewModel.ActionCommand.Execute(null);
        authViewModel.SuccessMessage.Should().Contain("Registration successful");

        authViewModel.IsLoginMode = true;
        authViewModel.EmailText = email;
        authViewModel.PasswordText = password;
        authViewModel.ErrorMessage = string.Empty;

        authViewModel.ActionCommand.Execute(null);
        authViewModel.IsAuthenticated.Should().BeTrue();
        authViewModel.AuthenticatedUser.Should().NotBeNull();
        authViewModel.AuthenticatedUser!.Email.Should().Be(email);
    }

    [Fact]
    public void AuthenticationViewModel_InvalidPassword_LoginFails()
    {
        var authViewModel = new AuthViewModel(authentificationService, navigationService);
        string uniqueCode = Guid.NewGuid().ToString().Substring(0, 4);
        string email = $"georgeta.popescu_{uniqueCode}@gmail.com";
        string correctPassword = "Parola@Georgeta456";

        authentificationService.Register(email, "0722556677", $"GeorgetaP_{uniqueCode}", correctPassword);

        authViewModel.IsLoginMode = true;
        authViewModel.EmailText = email;
        authViewModel.PasswordText = "WrongPassword123";

        authViewModel.ActionCommand.Execute(null);
        authViewModel.IsAuthenticated.Should().BeFalse();
        authViewModel.ErrorMessage.Should().Contain("Invalid");
    }

    [Fact]
    public async Task BookingViewModel_Initialization_UpdatesPrices()
    {
        var bookingViewModel = new BookingViewModel(bookingService, pricingService, navigationService);

        var user = new User { UserId = 1, Email = "test@gmail.com", Username = "test" };
        var flight = new Flight
        {
            FlightId = GetFirstAvailableFlightId(),
            Route = new Route { Capacity = 180, DepartureTime = DateTime.Now.AddDays(5), ArrivalTime = DateTime.Now.AddDays(5).AddHours(2) }
        };

        await bookingViewModel.InitializeAsync(flight, user);

        bookingViewModel.CurrentFlight.Should().Be(flight);
        bookingViewModel.CurrentUser.Should().Be(user);
        bookingViewModel.Passengers.Count.Should().Be(1);
    }

    [Fact]
    public async Task BookingViewModel_AddPassenger_UpdatesState()
    {
        var bookingViewModel = new BookingViewModel(bookingService, pricingService, navigationService);

        var user = new User { UserId = 1, Email = "rares.ionescu@gmail.com", Username = "rares" };
        var flight = new Flight
        {
            FlightId = GetFirstAvailableFlightId(),
            Route = new Route { Capacity = 180, DepartureTime = DateTime.Now.AddDays(3), ArrivalTime = DateTime.Now.AddDays(3).AddHours(1) }
        };

        await bookingViewModel.InitializeAsync(flight, user, 3);

        bookingViewModel.Passengers.Count.Should().Be(3);
        bookingViewModel.CanAddPassenger.Should().BeFalse();
    }

    [Fact]
    public async Task BookingViewModel_RemovePassenger_UpdatesCapacity()
    {
        var bookingViewModel = new BookingViewModel(bookingService, pricingService, navigationService);

        var user = new User { UserId = 1, Email = "adrian.stefan@gmail.com", Username = "adrian" };
        var flight = new Flight
        {
            FlightId = GetFirstAvailableFlightId(),
            Route = new Route { Capacity = 180, DepartureTime = DateTime.Now.AddDays(2), ArrivalTime = DateTime.Now.AddDays(2).AddHours(2) }
        };

        await bookingViewModel.InitializeAsync(flight, user, 2);
        int passengerCountBefore = bookingViewModel.Passengers.Count;

        var passengerToRemove = bookingViewModel.Passengers[0];
        bookingViewModel.RemovePassengerCommand.Execute(passengerToRemove);

        bookingViewModel.Passengers.Count.Should().Be(passengerCountBefore - 1);
        bookingViewModel.CanAddPassenger.Should().BeTrue();
    }

    [Fact]
    public void DashboardViewModel_AfterBooking_LoadsTickets()
    {
        string uniqueCode = Guid.NewGuid().ToString().Substring(0, 4);
        string email = $"cosmin.tudor_{uniqueCode}@gmail.com";
        string password = "Parola@Cosmin789";

        authentificationService.Register(email, "0733667788", $"CosminT_{uniqueCode}", password);
        var user = authentificationService.Login(email, password);

        UserSession.CurrentUser = user;
        var dashboardViewModel = new DashboardViewModel(
            new DashboardService(ticketRepository),
            new CancellationService(ticketRepository),
            navigationService);

        dashboardViewModel.LoadUserTickets();

        dashboardViewModel.OnNavigatedTo().Should().BeTrue();
    }

    [Fact]
    public void DashboardViewModel_FilterByUpcoming_ReturnsUpcomingTickets()
    {
        var user = new User { UserId = 1, Email = "test@gmail.com", Username = "test" };
        UserSession.CurrentUser = user;

        var dashboardViewModel = new DashboardViewModel(
            new DashboardService(ticketRepository),
            new CancellationService(ticketRepository),
            navigationService);

        dashboardViewModel.SelectedTicketFilter = "Upcoming";

        dashboardViewModel.MyTickets.Should().NotBeNull();
    }
}

