using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service;
using AirportApp.Tests.Unit.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.Tests.Integration.Workflows;

[TestClass]
public class BookingAndCancellationWorkflowIntegrationTests : BaseIntegrationTest
{
    private const int UniqueCodeStartIndex = 0;
    private const int UniqueCodeLength = 4;
    private const float BasePrice = 100.0f;
    private const int TwoPassengers = 2;
    private const string ReservationEmail = "rezervare.zbor";
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
    private const string CancellationPassword = "ParolaAnulare123!";
    private const string MariusFirstName = "Marius";
    private const string MariusLastName = "Lacatus";
    private const string CancellationSeatSuffix = "_3D";
    private const float CancellationPrice = 150.0f;
    private const string ActiveStatus = "Active";
    private const string CancelledStatus = "Cancelled";
    private const string DomainGmail = "@gmail.com";
    private const string AlreadyCancelledMessage = "already cancelled";
    private readonly ICustomerRepository userRepository;
    private readonly IFlightTicketRepository ticketRepository;
    private readonly IFlightRepository flightRepository;
    private readonly IAddOnRepository addOnRepository;
    private readonly AuthService authentificationService;
    private readonly BookingService bookingService;
    private readonly PricingService pricingService;
    private readonly CancellationService cancellationService;

    private readonly AirportDbContext dbContext;

    public BookingAndCancellationWorkflowIntegrationTests()
    {
        dbContext = CreateDbContext();
        var membershipRepository = new MembershipRepository(dbContext);
        userRepository = new CustomerRepository(dbContext, membershipRepository);
        ticketRepository = new FlightTicketRepository(dbContext);
        flightRepository = new FlightRepository(dbContext);
        addOnRepository = new AddOnRepository(dbContext);
        authentificationService = new AuthService(userRepository);
        bookingService = new BookingService(ticketRepository, addOnRepository);
        pricingService = new PricingService();
        cancellationService = new CancellationService(ticketRepository);
    }

    [TestMethod]
    public async Task CompleteBookingWorkflow_ValidData_Succeeds()
    {
        var uniqueCode = Guid.NewGuid().ToString().Substring(UniqueCodeStartIndex, UniqueCodeLength);
        var email = $"{ReservationEmail}_{uniqueCode}{DomainGmail}";
        var password = ReservationPassword;

        await authentificationService.RegisterAsync(email, ReservationPhone, $"Utilizator_{uniqueCode}", password);
        var user = await authentificationService.LoginAsync(email, password);

        var flightId = GetFirstAvailableFlightId(dbContext);
        var flight = await flightRepository.GetFlightByIdAsync(flightId);
        var passengers = PassengerDataFixture.CreateValidPassengerList(TwoPassengers);

        var validationResult = bookingService.ValidatePassengers(passengers);
        validationResult.Should().BeEmpty();

        var tickets = bookingService.CreateTickets(flight!, user, passengers, BasePrice);
        tickets.Should().HaveCount(TwoPassengers);

        foreach (var ticket in tickets)
        {
            ticket.UserId = user.Id;
            ticket.FlightId = flight!.Id;
        }

        var saveResult = await bookingService.SaveTicketsAsync(tickets);
        saveResult.Should().BeTrue();

        var userTickets = await ticketRepository.GetTicketsByUserIdAsync(user.Id);
        userTickets.Should().HaveCount(TwoPassengers);
    }

    [TestMethod]
    public async Task SaveTickets_DuplicateSeats_ThrowsException()
    {
        var uniqueCode = Guid.NewGuid().ToString().Substring(UniqueCodeStartIndex, UniqueCodeLength);
        var email = $"{DuplicateSeatsEmail}_{uniqueCode}{DomainGmail}";
        await authentificationService.RegisterAsync(email, ReservationPhone, $"{GigelUsername}_{uniqueCode}", GigelPassword);
        var user = await authentificationService.LoginAsync(email, GigelPassword);

        var flightId = GetFirstAvailableFlightId(dbContext);
        var flight = await flightRepository.GetFlightByIdAsync(flightId);
        var ticket1 = new FlightTicket { Flight = flight!, User = user, Seat = Seat1A, Price = BasePrice, Status = ActiveStatus, PassengerFirstName = GigelFirstName, PassengerLastName = GigelLastName };
        var ticket2 = new FlightTicket { Flight = flight!, User = user, Seat = Seat1A, Price = BasePrice, Status = ActiveStatus, PassengerFirstName = VasileFirstName, PassengerLastName = VasileLastName };

        var saveTicketsResult = await bookingService.SaveTicketsAsync(new List<FlightTicket> { ticket1, ticket2 });

        saveTicketsResult.Should().BeFalse();
    }

    [TestMethod]
    public async Task ValidateCancellation_BeforeCancelling_ReturnsTrue()
    {
        var uniqueCode = Guid.NewGuid().ToString().Substring(UniqueCodeStartIndex, UniqueCodeLength);
        var email = $"{CancellationEmail}_{uniqueCode}{DomainGmail}";
        await authentificationService.RegisterAsync(email, ReservationPhone, $"Anulare_{uniqueCode}", CancellationPassword);
        var user = await authentificationService.LoginAsync(email, CancellationPassword);

        var flightId = GetFirstAvailableFlightId(dbContext);
        var flight = await flightRepository.GetFlightByIdAsync(flightId);

        flight!.Date = DateTime.Now.AddDays(10);
        dbContext.Update(flight);
        await dbContext.SaveChangesAsync();

        var ticket = new FlightTicket
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
        await ticketRepository.AddTicketAsync(ticket);

        var userTickets = await ticketRepository.GetTicketsByUserIdAsync(user.Id);
        var createdTicket = userTickets.First();

        var (canCancel, reason) = cancellationService.CanCancelTicket(createdTicket);
        canCancel.Should().BeTrue();
        reason.Should().BeEmpty();

        await cancellationService.CancelTicketAsync(createdTicket.Id);

        dbContext.ChangeTracker.Clear();

        userTickets = await ticketRepository.GetTicketsByUserIdAsync(user.Id);
        var cancelledTicket = userTickets.First();
        cancelledTicket.Status.Should().Be(CancelledStatus);

        var (canCancelAgain, reasonAgain) = cancellationService.CanCancelTicket(cancelledTicket);
        canCancelAgain.Should().BeFalse();
        reasonAgain.Should().Contain(AlreadyCancelledMessage);
    }
}
