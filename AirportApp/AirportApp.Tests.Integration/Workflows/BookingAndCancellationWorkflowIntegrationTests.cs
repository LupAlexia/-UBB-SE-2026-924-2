using FluentAssertions;
using TicketManager.Domain;
using TicketManager.Repository;
using TicketManager.Service;
using TicketManager.Tests.Unit.Fixtures;

namespace TicketManager.Tests.Integration.Workflows;

public class BookingAndCancellationWorkflowIntegrationTests : BaseIntegrationTest
{
    private const int UniqueCodeStartIndex = 0;
    private const int UniqueCodeLength = 4;
    private const float BasePrice = 100.0f;
    private const int TwoPassengers = 2;
    private const string ReservationEmail = "rezervare.zbor";
    private const string ReservationUsername = "Utilizator";
    private const string ReservationPassword = "ParolaRezervare123!";
    private const string ReservationPhone = "0722112233";
    private const string DuplicateSeatsEmail = "locuri.duplicate";
    private const string GigelUsername = "Gigel";
    private const string GigelPassword = "ParolaGigel123!";
    private const string GigelFirstName = "Gigel";
    private const string GigelLastName = "Frone";
    private const string VasileFirstName = "Vasile";
    private const string VasileLastName = "Traian";
    private const string Seat1A = "1A";
    private const string CancellationEmail = "anulare.zbor";
    private const string CancellationUsername = "Anulare";
    private const string CancellationPassword = "ParolaAnulare123!";
    private const string MariusFirstName = "Marius";
    private const string MariusLastName = "Lacatus";
    private const string CancellationSeatSuffix = "_3D";
    private const float CancellationPrice = 150.0f;
    private const string ActiveStatus = "Active";
    private const string CancelledStatus = "Cancelled";
    private const string DomainGmail = "@gmail.com";
    private const string AlreadyCancelledMessage = "already cancelled";
    private readonly IUserRepository userRepository;
    private readonly ITicketRepository ticketRepository;
    private readonly IAddOnRepository addOnRepository;
    private readonly AuthService authentificationService;
    private readonly BookingService bookingService;
    private readonly PricingService pricingService;
    private readonly CancellationService cancellationService;

    public BookingAndCancellationWorkflowIntegrationTests()
    {
        var databaseConnectionFactory = new DatabaseConnectionFactory(GetTestConnectionString());
        var membershipRepository = new MembershipRepository(databaseConnectionFactory);
        userRepository = new UserRepository(databaseConnectionFactory, membershipRepository);
        ticketRepository = new TicketRepository(databaseConnectionFactory);
        addOnRepository = new AddOnRepository(databaseConnectionFactory);
        authentificationService = new AuthService(userRepository);
        bookingService = new BookingService(ticketRepository, addOnRepository);
        pricingService = new PricingService();
        cancellationService = new CancellationService(ticketRepository);
    }

    [Fact]
    public async Task CompleteBookingWorkflow_ValidData_Succeeds()
    {
        var uniqueCode = Guid.NewGuid().ToString().Substring(UniqueCodeStartIndex, UniqueCodeLength);
        var email = $"{ReservationEmail}_{uniqueCode}{DomainGmail}";
        var password = ReservationPassword;

        authentificationService.Register(email, ReservationPhone, $"{ReservationUsername}_{uniqueCode}", password);
        var user = authentificationService.Login(email, password);

        var flightId = GetFirstAvailableFlightId();
        var flight = FlightFixture.CreateValidTestFlight(flightId: flightId);
        var passengers = PassengerDataFixture.CreateValidPassengerList(TwoPassengers);

        var validationResult = bookingService.ValidatePassengers(passengers);
        validationResult.Should().BeEmpty();

        var tickets = bookingService.CreateTickets(flight, user, passengers, BasePrice);
        tickets.Should().HaveCount(TwoPassengers);

        var saveResult = await bookingService.SaveTicketsAsync(tickets);
        saveResult.Should().BeTrue();

        var userTickets = ticketRepository.GetTicketsByUserId(user.UserId);
        userTickets.Should().HaveCount(TwoPassengers);
    }

    [Fact]
    public async Task SaveTickets_DuplicateSeats_ThrowsException()
    {
        var uniqueCode = Guid.NewGuid().ToString().Substring(UniqueCodeStartIndex, UniqueCodeLength);
        var email = $"{DuplicateSeatsEmail}_{uniqueCode}{DomainGmail}";
        authentificationService.Register(email, ReservationPhone, $"{GigelUsername}_{uniqueCode}", GigelPassword);
        var user = authentificationService.Login(email, GigelPassword);

        var flightId = GetFirstAvailableFlightId();
        var flight = FlightFixture.CreateValidTestFlight(flightId: flightId);
        var ticket1 = new Ticket { Flight = flight, User = user, Seat = Seat1A, Price = BasePrice, Status = ActiveStatus, PassengerFirstName = GigelFirstName, PassengerLastName = GigelLastName };
        var ticket2 = new Ticket { Flight = flight, User = user, Seat = Seat1A, Price = BasePrice, Status = ActiveStatus, PassengerFirstName = VasileFirstName, PassengerLastName = VasileLastName };

        var saveTicketsResult = await bookingService.SaveTicketsAsync(new List<Ticket> { ticket1, ticket2 });

        saveTicketsResult.Should().BeFalse();
    }

    [Fact]
    public void ValidateCancellation_BeforeCancelling_ReturnsTrue()
    {
        var uniqueCode = Guid.NewGuid().ToString().Substring(UniqueCodeStartIndex, UniqueCodeLength);
        var email = $"{CancellationEmail}_{uniqueCode}{DomainGmail}";
        authentificationService.Register(email, ReservationPhone, $"{CancellationUsername}_{uniqueCode}", CancellationPassword);
        var user = authentificationService.Login(email, CancellationPassword);

        var flightId = GetFirstAvailableFlightId();
        var flight = FlightFixture.CreateValidTestFlight(flightId: flightId);
        var ticket = new Ticket
        {
            Flight = flight,
            User = user,
            Seat = $"{uniqueCode}{CancellationSeatSuffix}",
            Price = CancellationPrice,
            Status = ActiveStatus,
            PassengerFirstName = MariusFirstName,
            PassengerLastName = MariusLastName,
            PassengerEmail = email
        };
        ticketRepository.AddTicket(ticket);

        var createdTicket = ticketRepository.GetTicketsByUserId(user.UserId).First();

        var (canCancel, reason) = cancellationService.CanCancelTicket(createdTicket);
        canCancel.Should().BeTrue();
        reason.Should().BeEmpty();

        cancellationService.CancelTicket(createdTicket.TicketId);

        var cancelledTicket = ticketRepository.GetTicketsByUserId(user.UserId).First();
        cancelledTicket.Status.Should().Be(CancelledStatus);

        var (canCancelAgain, reasonAgain) = cancellationService.CanCancelTicket(cancelledTicket);
        canCancelAgain.Should().BeFalse();
        reasonAgain.Should().Contain(AlreadyCancelledMessage);
    }
}
