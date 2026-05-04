using FluentAssertions;
using TicketManager.Domain;
using TicketManager.Repository;
using TicketManager.Service;

namespace TicketManager.Tests.Integration.Services;

public class BookingServiceIntegrationTests : BaseIntegrationTest
{
    private const float DefaultBasePrice = 150.0f;
    private const int DefaultFlightCapacity = 180;
    private const int DefaultOccupiedSeats = 50;
    private const int DefaultRequestedPassengers = 10;
    private const int UniqueCodeStartIndex = 0;
    private const int UniqueCodeLength = 4;
    private const int ExpectedTicketCount = 1;
    private const string MirceaEmail = "mrc.popa";
    private const string MirceaUsername = "MirceaP";
    private const string MirceaPassword = "Mircea123!";
    private const string MirceaPhone = "0722334455";
    private const string MirceaFirstName = "Mircea";
    private const string MirceaLastName = "Popa";
    private const string SeatIdentifier = "_1A";

    private readonly ITicketRepository ticketRepository;
    private readonly IAddOnRepository addOnRepository;
    private readonly IUserRepository userRepository;
    private readonly BookingService bookingService;

    public BookingServiceIntegrationTests()
    {
        var databaseConnectionFactory = new DatabaseConnectionFactory(GetTestConnectionString());
        ticketRepository = new TicketRepository(databaseConnectionFactory);
        addOnRepository = new AddOnRepository(databaseConnectionFactory);
        var membershipRepository = new MembershipRepository(databaseConnectionFactory);
        userRepository = new UserRepository(databaseConnectionFactory, membershipRepository);
        bookingService = new BookingService(ticketRepository, addOnRepository);
    }

    [Fact]
    public async Task CreateAndSaveTickets_ValidTickets_Succeeds()
    {
        var flightId = GetFirstAvailableFlightId();
        var flight = new Flight { FlightId = flightId };
        var uniqueCode = Guid.NewGuid().ToString().Substring(UniqueCodeStartIndex, UniqueCodeLength);
        var user = new User { Email = $"{MirceaEmail}_{uniqueCode}@gmail.com", Username = $"{MirceaUsername}_{uniqueCode}", PasswordHash = MirceaPassword };
        userRepository.AddUser(user);
        var databaseUser = userRepository.GetByEmail(user.Email);

        var passengers = new List<PassengerData>
        {
            new PassengerData { FirstName = MirceaFirstName, LastName = MirceaLastName, Email = user.Email, Phone = MirceaPhone, SelectedSeat = $"{uniqueCode}{SeatIdentifier}" }
        };

        var tickets = bookingService.CreateTickets(flight, databaseUser!, passengers, DefaultBasePrice);
        var saveResult = await bookingService.SaveTicketsAsync(tickets);

        saveResult.Should().BeTrue();
        tickets.Should().HaveCount(ExpectedTicketCount);
    }

    [Fact]
    public void CalculateMaximumPassengers_Integration_CalculatesCorrectly()
    {
        var maxPassengers = bookingService.CalculateMaxPassengers(DefaultFlightCapacity, DefaultOccupiedSeats, DefaultRequestedPassengers);
        maxPassengers.Should().Be(DefaultRequestedPassengers);
    }
}


