using FluentAssertions;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service;
using AirportApp.Tests.Unit.Fixtures;
using AirportApp.ClassLibrary.DataAccess;

namespace AirportApp.Tests.Integration.Services;

[TestClass]
public class BookingServiceIntegrationTests : BaseIntegrationTest
{
    private const int UniqueCodeStartIndex = 0;
    private const int UniqueCodeLength = 4;
    private const string MihaiEmail = "mihai.popescu";
    private const string MihaiUsername = "MihaiPopescu";
    private const string MihaiPassword = "Parolalamos123!";
    private const string MihaiPhone = "0722112233";
    private const float TicketPrice = 100.0f;
    private readonly IBookingService bookingService;
    private readonly IFlightTicketRepository ticketRepository;
    private readonly IFlightRepository flightRepository;
    private readonly AirportDbContext _dbContext; 

    public BookingServiceIntegrationTests()
    {
        _dbContext = CreateDbContext();
        ticketRepository = new FlightTicketRepository(_dbContext);
        flightRepository = new FlightRepository(_dbContext);
        bookingService = new BookingService(ticketRepository, new AddOnRepository(_dbContext));
    }

    [TestMethod]
    public async Task CreateAndSaveTickets_ValidTickets_SucceedsAsync()
    {
        var uniqueCode = Guid.NewGuid().ToString().Substring(UniqueCodeStartIndex, UniqueCodeLength);
        var user = new Customer { Email = $"{MihaiEmail}_{uniqueCode}@gmail.com", Username = $"{MihaiUsername}_{uniqueCode}", Phone = MihaiPhone, PasswordHash = MihaiPassword };

        var flightId = GetFirstAvailableFlightId(_dbContext);
        var flight = await flightRepository.GetFlightByIdAsync(flightId);

        var tickets = new List<FlightTicket>
        {
            new FlightTicket { Flight = flight!, User = user, Seat = "1A", Price = TicketPrice, Status = "Active", PassengerFirstName = "Mihai", PassengerLastName = "Popescu" }
        };

        var saveResult = await bookingService.SaveTicketsAsync(tickets);

        saveResult.Should().BeTrue();
    }

    [TestMethod]
    public void CalculateMaximumPassengers_Integration_CalculatesCorrectly()
    {
        int capacity = 180;
        int occupiedCount = 50;
        int requestedCount = 3;

        int max = bookingService.CalculateMaxPassengers(capacity, occupiedCount, requestedCount);

        max.Should().Be(3);
    }
}
