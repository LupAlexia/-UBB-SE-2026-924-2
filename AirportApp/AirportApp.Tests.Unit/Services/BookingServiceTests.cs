using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.Src.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service;
using AirportApp.Tests.Unit.Fixtures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System;

namespace AirportApp.Tests.Unit.Services;

[TestClass]
public class BookingServiceTests
{
    private const int DefaultId = 1;
    private const float DefaultBasePrice = 150.0f;
    private const float StandardTicketPrice = 100.0f;
    private const int LargeFlightCapacity = 200;
    private const int HighOccupiedSeats = 195;
    private const int ModerateOccupiedSeats = 100;
    private const int ZeroRequestedPassengers = 0;
    private const int NormalRequestedPassengers = 5;
    private const int ExpectedMaxPassengers = 5;
    private const string ActiveStatus = "Active";
    private const string Seat1A = "1A";
    private const string Seat1C = "1C";
    private const string Seat1D = "1D";
    private const string Seat1F = "1F";
    private const string Seat1B = "1B";
    private const string Seat2B = "2B";
    private const string InvalidParameter = "NotAnArray";
    private const int ExactMultipleCapacity = 180;
    private const int PartialMultipleCapacity = 182;
    private const int MinimumFlightCapacity = 6;
    private const int ExpectedExactMultipleRows = 30;
    private const int ExpectedPartialMultipleRows = 31;
    private const int ExpectedMinimumCapacityRows = 1;
    private const int Column0Index = 0;
    private const int Column2Index = 2;
    private const int Column4Index = 4;
    private const int Column6Index = 6;
    private const int ExpectedExactMultipleLayoutCount = 180;
    private const int ExpectedPartialMultipleLayoutCount = 186;

    private readonly Mock<IFlightTicketRepository> mockTicketRepository;
    private readonly Mock<IAddOnRepository> mockAddOnRepository;
    private readonly BookingService bookingService;

    public BookingServiceTests()
    {
        mockTicketRepository = new Mock<IFlightTicketRepository>();
        mockAddOnRepository = new Mock<IAddOnRepository>();
        bookingService = new BookingService(mockTicketRepository.Object, mockAddOnRepository.Object);
    }

    [TestMethod]
    public void CreateTickets_ValidPassengers_AssignsDataCorrectly()
    {
        var flight = new Flight { Id = DefaultId };
        var user = new Customer { Id = 1, Email = "test@test.com" };
        var passenger = PassengerDataFixture.CreateValidPassengerData(
            firstName: "Ionel",
            lastName: "Gheorghe",
            email: "ionel.ghe@gmail.com");
        var passengers = new List<PassengerData> { passenger };

        var tickets = bookingService.CreateTickets(flight, user, passengers, DefaultBasePrice);

        tickets[0].PassengerFirstName.Should().Be("Ionel");
        tickets[0].PassengerLastName.Should().Be("Gheorghe");
    }

    [TestMethod]
    public void ValidatePassengers_EmptyList_ReturnsError()
    {
        var validationErrorMessage = bookingService.ValidatePassengers(new List<PassengerData>());
        validationErrorMessage.Should().Be("At least one passenger is required.");
    }

    [TestMethod]
    public void ValidatePassengers_MissingFirstName_Fails()
    {
        var passenger = new PassengerData { LastName = "Pop", SelectedSeat = Seat1A };
        var validationErrorMessage = bookingService.ValidatePassengers(new List<PassengerData> { passenger });
        validationErrorMessage.Should().Contain("first name is required");
    }

    [TestMethod]
    public void ValidatePassengers_MissingLastName_Fails()
    {
        var passenger = new PassengerData { FirstName = "Vasile", SelectedSeat = Seat1A };
        var validationErrorMessage = bookingService.ValidatePassengers(new List<PassengerData> { passenger });
        validationErrorMessage.Should().Contain("last name is required");
    }

    [TestMethod]
    public void ValidatePassengers_NoSeatSelected_Fails()
    {
        var passenger = new PassengerData { FirstName = "Vasile", LastName = "Pop", SelectedSeat = string.Empty };
        var validationErrorMessage = bookingService.ValidatePassengers(new List<PassengerData> { passenger });
        validationErrorMessage.Should().Contain("please select a seat");
    }

    [TestMethod]
    public void CalculateMaximumPassengers_ValidRequest_ReturnsRemainingCapacity()
    {
        var maxPassengers = bookingService.CalculateMaxPassengers(LargeFlightCapacity, HighOccupiedSeats, ZeroRequestedPassengers);
        maxPassengers.Should().Be(ExpectedMaxPassengers);
    }

    [TestMethod]
    public void CalculateMaximumPassengers_RequestedExceedsCapacity_CapsAtRequested()
    {
        var maxPassengers = bookingService.CalculateMaxPassengers(LargeFlightCapacity, ModerateOccupiedSeats, NormalRequestedPassengers);
        maxPassengers.Should().Be(ExpectedMaxPassengers);
    }

    [TestMethod]
    public async Task SaveTicketsAsynchronous_NullTicketList_ReturnsFalse()
    {
        var saveTicketsSucceeded = await bookingService.SaveTicketsAsync(null!);
        saveTicketsSucceeded.Should().BeFalse();
    }

    [TestMethod]
    public async Task SaveTicketsAsynchronous_EmptyTicketList_ReturnsFalse()
    {
        var saveTicketsSucceeded = await bookingService.SaveTicketsAsync(new List<FlightTicket>());
        saveTicketsSucceeded.Should().BeFalse();
    }

    [TestMethod]
    public async Task SaveTicketsAsynchronous_DuplicateSeats_ReturnsFalse()
    {
        var ticket1 = new FlightTicket { Seat = Seat1A, Price = StandardTicketPrice, Status = ActiveStatus };
        var ticket2 = new FlightTicket { Seat = Seat1A, Price = StandardTicketPrice, Status = ActiveStatus };
        var tickets = new List<FlightTicket> { ticket1, ticket2 };

        var saveTicketsSucceeded = await bookingService.SaveTicketsAsync(tickets);

        saveTicketsSucceeded.Should().BeFalse();
    }

    [TestMethod]
    public async Task SaveTicketsAsynchronous_ValidTickets_ReturnsTrue()
    {
        mockTicketRepository.Setup(repo => repo.IsSeatAvailableAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(true);
        mockTicketRepository.Setup(repo => repo.SaveTicketsWithAddOnsAsync(It.IsAny<List<FlightTicket>>(), It.IsAny<List<List<int>>>())).ReturnsAsync(true);


        var ticket1 = new FlightTicket { Seat = Seat1A, Price = StandardTicketPrice, Status = ActiveStatus };
        var ticket2 = new FlightTicket { Seat = Seat1B, Price = StandardTicketPrice, Status = ActiveStatus };
        var tickets = new List<FlightTicket> { ticket1, ticket2 };

        var saveTicketsSucceeded = await bookingService.SaveTicketsAsync(tickets);

        saveTicketsSucceeded.Should().BeTrue();
        mockTicketRepository.Verify(repo => repo.SaveTicketsWithAddOnsAsync(It.IsAny<List<FlightTicket>>(), It.IsAny<List<List<int>>>()), Times.Once);
    }

    [TestMethod]
    public void ValidatePassengers_ValidEmailAddress_Accepts()
    {
        var passenger = PassengerDataFixture.CreateValidPassengerData(email: "test@gmail.com");
        var validationErrorMessage = bookingService.ValidatePassengers(new List<PassengerData> { passenger });
        validationErrorMessage.Should().BeEmpty();
    }

    [TestMethod]
    public void ValidatePassengers_InvalidEmailAddress_Rejects()
    {
        var passenger = new PassengerData { FirstName = "Ion", LastName = "Pop", SelectedSeat = Seat1A, Email = "not-an-email" };
        var validationErrorMessage = bookingService.ValidatePassengers(new List<PassengerData> { passenger });
        validationErrorMessage.Should().Contain("email format is invalid");
    }

    [TestMethod]
    public void ParseBookingParameters_WithArguments_ReturnsParsedResult()
    {
        var defaultFlight = new Flight { Id = DefaultId };
        var defaultTestUser = new Customer { Id = 1, Email = "test@test.com" };
        object[] bookingArguments = { defaultFlight, defaultTestUser, NormalRequestedPassengers };

        var parsedBookingParameters = bookingService.ParseBookingParameters(bookingArguments);

        parsedBookingParameters.Flight.Should().Be(defaultFlight);
        parsedBookingParameters.User.Should().Be(defaultTestUser);
        parsedBookingParameters.RequestedPassengers.Should().Be(NormalRequestedPassengers);
    }

    [TestMethod]
    public void StorePendingBooking_ValidBooking_StoresBooking()
    {
        var defaultFlight = new Flight { Id = DefaultId };

        bookingService.StorePendingBooking(defaultFlight, NormalRequestedPassengers);

        UserSession.PendingBookingParameters.Should().NotBeNull();
        var pendingParams = UserSession.PendingBookingParameters as object[];
        pendingParams.Should().NotBeNull();
        pendingParams.Should().HaveCount(2);
        pendingParams[0].Should().Be(defaultFlight);
        pendingParams[1].Should().Be(NormalRequestedPassengers);
    }

    [TestMethod]
    public void ApplySeatSelection_SeatAlreadyAssigned_RemovesSeat()
    {
        var seats = new List<string> { Seat1A, Seat1B };

        var updated = bookingService.ApplySeatSelection(seats, 0, Seat1A);

        updated[0].Should().BeEmpty();
        updated[1].Should().Be(Seat1B);
    }

    [TestMethod]
    public void ApplySeatSelection_NewSeat_AssignsSeatAndClearsDuplicate()
    {
        var seats = new List<string> { Seat1A, Seat2B };

        var updated = bookingService.ApplySeatSelection(seats, 1, Seat1A);

        updated[0].Should().BeEmpty();
        updated[1].Should().Be(Seat1A);
    }

    [TestMethod]
    public void ApplyAddOn_Updates_AdjustsListCorrectly()
    {
        var priorityBoarding = new AddOn { Id = 1, Name = "Priority Boarding" };
        var extraLuggage = new AddOn { Id = 2, Name = "Extra Luggage" };

        var currentAddOns = new List<AddOn> { priorityBoarding };
        var toAdd = new List<AddOn> { extraLuggage };
        var toRemove = new List<AddOn> { priorityBoarding };

        bookingService.ApplyAddOnUpdates(currentAddOns, toAdd, toRemove);

        currentAddOns.Should().Contain(extraLuggage);
        currentAddOns.Should().NotContain(priorityBoarding);
    }

    [TestMethod]
    public void GetInitialPassengerCount_Default_ReturnsMinimumValue()
    {
        var initialPassengerCountRequested = bookingService.GetInitialPassengerCount(ExpectedMaxPassengers, NormalRequestedPassengers);
        initialPassengerCountRequested.Should().Be(NormalRequestedPassengers);

        var limitedCount = bookingService.GetInitialPassengerCount(2, 5);
        limitedCount.Should().Be(2);

        var fallbackCount = bookingService.GetInitialPassengerCount(5, 0);
        fallbackCount.Should().Be(1);
    }

    [TestMethod]
    public void ParseBookingParameters_NullOrInvalidObject_HandlesCorrectly()
    {
        var parsedBookingParameters = bookingService.ParseBookingParameters(InvalidParameter);

        parsedBookingParameters.Flight.Should().BeNull();
        parsedBookingParameters.RequestedPassengers.Should().Be(ZeroRequestedPassengers);
    }

    [TestMethod]
    public void ParseBookingParameters_OnlyFlightArgument_HandlesCorrectly()
    {
        var defaultFlight = new Flight { Id = DefaultId };
        object[] bookingArguments = { defaultFlight };

        var parsedBookingParameters = bookingService.ParseBookingParameters(bookingArguments);

        parsedBookingParameters.Flight.Should().Be(defaultFlight);
        parsedBookingParameters.RequestedPassengers.Should().Be(ZeroRequestedPassengers);
    }

    [TestMethod]
    public void ParseBookingParameters_FlightAndPassengerCount_HandlesCorrectly()
    {
        var defaultFlight = new Flight { Id = DefaultId };
        object[] bookingArguments = { defaultFlight, NormalRequestedPassengers };

        var parsedBookingParameters = bookingService.ParseBookingParameters(bookingArguments);

        parsedBookingParameters.Flight.Should().Be(defaultFlight);
        parsedBookingParameters.RequestedPassengers.Should().Be(NormalRequestedPassengers);
    }

    [TestMethod]
    public void ParseBookingParameters_NoArguments_FallsBackToSession()
    {
        var defaultSessionUser = new Customer { Id = 1, Email = "test@test.com" };
        UserSession.CurrentUser = defaultSessionUser;
        var defaultFlight = new Flight { Id = DefaultId };
        object[] bookingArguments = { defaultFlight };

        var parsedBookingParameters = bookingService.ParseBookingParameters(bookingArguments);

        parsedBookingParameters.User.Should().Be(defaultSessionUser);

        UserSession.CurrentUser = null;
    }

    [TestMethod]
    public void BuildSeatMapLayout_ExactMultiple_CalculatesRowsCorrectly()
    {
        var (layout, rowCount) = bookingService.BuildSeatMapLayout(ExactMultipleCapacity);

        rowCount.Should().Be(ExpectedExactMultipleRows);
        layout.Should().HaveCount(ExpectedExactMultipleLayoutCount);
    }

    [TestMethod]
    public void BuildSeatMapLayout_NonMultiple_CalculatesRowsCorrectly()
    {
        var (layout, rowCount) = bookingService.BuildSeatMapLayout(PartialMultipleCapacity);

        rowCount.Should().Be(ExpectedPartialMultipleRows);
        layout.Should().HaveCount(ExpectedPartialMultipleLayoutCount);
    }

    [TestMethod]
    public void BuildSeatMapLayout_ValidData_AssignsCorrectLabelsAndColumns()
    {
        var (layout, rowCount) = bookingService.BuildSeatMapLayout(MinimumFlightCapacity);

        rowCount.Should().Be(ExpectedMinimumCapacityRows);

        layout[0].Label.Should().Be(Seat1A);
        layout[2].Label.Should().Be(Seat1C);
        layout[3].Label.Should().Be(Seat1D);
        layout[5].Label.Should().Be(Seat1F);

        layout[0].Column.Should().Be(Column0Index);
        layout[2].Column.Should().Be(Column2Index);
        layout[3].Column.Should().Be(Column4Index);
        layout[5].Column.Should().Be(Column6Index);
    }
}