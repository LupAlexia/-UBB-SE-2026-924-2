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
public class CompleteBookingFlowIntegrationTests : BaseIntegrationTest
{
    private const int UniqueCodeStartIndex = 0;
    private const int UniqueCodeLength = 4;
    private const string DanEmail = "dan.ionescu";
    private const string DanUsername = "DanI";
    private const string DanPassword = "ParolaDan2024!";
    private const string DefaultPhone = "0722112233";
    private const float TargetPrice = 150.0f;
    private const int MembershipId = 1;
    private const string MembershipName = "Premium";
    private const int MembershipDiscount = 10;
    private const float TicketPrice = 100.0f;
    private const int SinglePassenger = 1;
    private const string DomainGmail = "@gmail.com";
    private readonly ICustomerRepository userRepository;
    private readonly IFlightTicketRepository ticketRepository;
    private readonly IFlightRepository flightRepository;
    private readonly AuthService authentificationService;
    private readonly BookingService bookingService;
    private readonly PricingService pricingService;
    private readonly AirportDbContext dbContext;

    public CompleteBookingFlowIntegrationTests()
    {
        dbContext = CreateDbContext();
        var membershipRepository = new MembershipRepository(dbContext);
        userRepository = new CustomerRepository(dbContext, membershipRepository);
        ticketRepository = new FlightTicketRepository(dbContext);
        flightRepository = new FlightRepository(dbContext);
        authentificationService = new AuthService(userRepository);
        bookingService = new BookingService(ticketRepository, new AddOnRepository(dbContext));
        pricingService = new PricingService();
    }

    private async Task<Flight> GetTestFlightAsync()
    {
        var flightId = GetFirstAvailableFlightId(dbContext);
        return (await flightRepository.GetFlightByIdAsync(flightId)) !;
    }

    [TestMethod]
    public async Task CompleteBookingFlow_ValidData_Succeeds()
    {
        string uniqueCode = Guid.NewGuid().ToString().Substring(UniqueCodeStartIndex, UniqueCodeLength);
        string email = $"{DanEmail}_{uniqueCode}{DomainGmail}";
        string password = DanPassword;
        await authentificationService.RegisterAsync(email, DefaultPhone, $"{DanUsername}_{uniqueCode}", password);

        var user = await authentificationService.LoginAsync(email, password);
        var flight = await GetTestFlightAsync();
        var passengers = PassengerDataFixture.CreateValidPassengerList(SinglePassenger);

        var tickets = bookingService.CreateTickets(flight, user, passengers, TargetPrice);
        var saveResult = await bookingService.SaveTicketsAsync(tickets);

        saveResult.Should().BeTrue();
    }

    [TestMethod]
    public async Task CompleteBookingFlow_PremiumUser_GetsMembershipDiscount()
    {
        var membership = new Membership { Id = MembershipId, Name = MembershipName, FlightDiscountPercentage = MembershipDiscount };
        var user = UserFixture.CreateValidTestUser(membership: membership);
        user.Membership = membership;
        user.MembershipId = membership.Id;

        var flight = await GetTestFlightAsync();
        var tickets = new List<FlightTicket> { new FlightTicket { Price = TicketPrice } };

        var priceBreakdown = pricingService.CalculatePriceBreakdown(flight, user, tickets);

        priceBreakdown.MembershipSavings.Should().BeGreaterThan(0);
        priceBreakdown.FinalTotal.Should().BeLessThan(TicketPrice);
    }
}
