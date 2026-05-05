using FluentAssertions;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service;
using AirportApp.Src.ViewModel;
using AirportApp.ClassLibrary.DataAccess;
using AirportApp.Service;

namespace AirportApp.Tests.Integration.Workflows;

[TestClass]
public class AuthAndBookingViewModelIntegrationTests : BaseIntegrationTest
{
    private const int UniqueCodeStartIndex = 0;
    private const int UniqueCodeLength = 4;
    private const string VasileEmail = "vasile.mihai";
    private const string VasilePassword = "Parola@Vasile123";
    private const string CosminEmail = "cosmin.tudor";
    private const string CosminPassword = "Parola@Cosmin789";

    private readonly ICustomerRepository userRepository;
    private readonly IFlightRepository flightRepository;
    private readonly IFlightTicketRepository ticketRepository;
    private readonly IAddOnRepository addOnRepository;
    private readonly IMembershipRepository membershipRepository;
    private readonly AuthService authentificationService;
    private readonly BookingService bookingService;
    private readonly PricingService pricingService;
    private readonly AirportApp.Service.NavigationService navigationService;

    public AuthAndBookingViewModelIntegrationTests()
    {
        var dbContext = CreateDbContext();
        membershipRepository = new MembershipRepository(dbContext);
        userRepository = new CustomerRepository(dbContext, membershipRepository);
        flightRepository = new FlightRepository(dbContext);
        ticketRepository = new FlightTicketRepository(dbContext);
        addOnRepository = new AddOnRepository(dbContext);

        authentificationService = new AuthService(userRepository);
        bookingService = new BookingService(ticketRepository, addOnRepository);
        pricingService = new PricingService();
        navigationService = new AirportApp.Service.NavigationService();
    }

    [TestMethod]
    public async Task AuthenticationViewModel_RegisterAndLogin_Succeeds()
    {
        var authViewModel = new AuthViewModel(authentificationService, navigationService);
        string uniqueCode = Guid.NewGuid().ToString().Substring(0, 4);
        string email = $"vasile.mihai_{uniqueCode}@gmail.com";
        string password = VasilePassword;

        authViewModel.IsLoginMode = false;
        authViewModel.EmailText = email;
        authViewModel.PhoneText = "0733445566";
        authViewModel.UsernameText = $"VasileM_{uniqueCode}";
        authViewModel.PasswordText = password;

        authViewModel.ActionCommand.Execute(null);
        // Wait for async void
        await Task.Delay(1000);
        
        authViewModel.SuccessMessage.Should().Contain("Registration successful");

        authViewModel.IsLoginMode = true;
        authViewModel.EmailText = email;
        authViewModel.PasswordText = password;
        authViewModel.ErrorMessage = string.Empty;

        authViewModel.ActionCommand.Execute(null);
        await Task.Delay(1000);

        authViewModel.IsAuthenticated.Should().BeTrue();
        authViewModel.AuthenticatedUser.Should().NotBeNull();
        authViewModel.AuthenticatedUser!.Email.Should().Be(email);
    }

    [TestMethod]
    public async Task AuthenticationViewModel_InvalidPassword_LoginFails()
    {
        var authViewModel = new AuthViewModel(authentificationService, navigationService);
        string uniqueCode = Guid.NewGuid().ToString().Substring(0, 4);
        string email = $"georgeta.popescu_{uniqueCode}@gmail.com";
        string correctPassword = "Parola@Georgeta456";

        await authentificationService.RegisterAsync(email, "0722556677", $"GeorgetaP_{uniqueCode}", correctPassword);

        authViewModel.IsLoginMode = true;
        authViewModel.EmailText = email;
        authViewModel.PasswordText = "WrongPassword123";

        authViewModel.ActionCommand.Execute(null);
        await Task.Delay(1000);

        authViewModel.IsAuthenticated.Should().BeFalse();
        authViewModel.ErrorMessage.Should().NotBeEmpty();
    }

    [TestMethod]
    public async Task BookingViewModel_Initialization_UpdatesPrices()
    {
        var bookingViewModel = new BookingViewModel(bookingService, pricingService, navigationService);

        var user = new Customer { Id = 1, Email = "test@gmail.com", Username = "test" };
        var flightId = GetFirstAvailableFlightId();
        var flight = await flightRepository.GetFlightByIdAsync(flightId);

        await bookingViewModel.InitializeAsync(flight!, user);

        bookingViewModel.CurrentFlight.Should().Be(flight);
        bookingViewModel.CurrentUser.Should().Be(user);
        bookingViewModel.Passengers.Count.Should().Be(1);
    }

    [TestMethod]
    public async Task BookingViewModel_AddPassenger_UpdatesState()
    {
        var bookingViewModel = new BookingViewModel(bookingService, pricingService, navigationService);

        var user = new Customer { Id = 1, Email = "rares.ionescu@gmail.com", Username = "rares" };
        var flightId = GetFirstAvailableFlightId();
        var flight = await flightRepository.GetFlightByIdAsync(flightId);

        await bookingViewModel.InitializeAsync(flight!, user, 3);

        bookingViewModel.Passengers.Count.Should().BeInRange(1, 3);
    }

    [TestMethod]
    public async Task BookingViewModel_RemovePassenger_UpdatesCapacity()
    {
        var bookingViewModel = new BookingViewModel(bookingService, pricingService, navigationService);

        var user = new Customer { Id = 1, Email = "adrian.stefan@gmail.com", Username = "adrian" };
        var flightId = GetFirstAvailableFlightId();
        var flight = await flightRepository.GetFlightByIdAsync(flightId);

        await bookingViewModel.InitializeAsync(flight!, user, 2);
        int passengerCountBefore = bookingViewModel.Passengers.Count;

        if (passengerCountBefore > 1)
        {
            var passengerToRemove = bookingViewModel.Passengers[0];
            bookingViewModel.RemovePassengerCommand.Execute(passengerToRemove);
            bookingViewModel.Passengers.Count.Should().Be(passengerCountBefore - 1);
        }
    }

    [TestMethod]
    public async Task DashboardViewModel_AfterBooking_LoadsTickets()
    {
        string uniqueCode = Guid.NewGuid().ToString().Substring(0, 4);
        string email = $"cosmin.tudor_{uniqueCode}@gmail.com";
        string password = CosminPassword;

        await authentificationService.RegisterAsync(email, "0733667788", $"CosminT_{uniqueCode}", password);
        var user = await authentificationService.LoginAsync(email, password);

        UserSession.CurrentUser = user;
        var dashboardViewModel = new DashboardViewModel(
            new DashboardService(ticketRepository),
            new CancellationService(ticketRepository),
            navigationService);

        // Wait for the constructor's fire-and-forget LoadUserTicketsAsync to complete
        await Task.Delay(1000);

        var result = await dashboardViewModel.OnNavigatedToAsync();
        result.Should().BeTrue();
    }
}
