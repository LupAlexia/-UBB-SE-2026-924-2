using System.Collections.ObjectModel;
using AirportApp.ClassLibrary.Entity.Domain;
using FluentAssertions;
using Moq;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.Src.Service;
using AirportApp.Src.ViewModel;

namespace AirportApp.Tests.Unit.ViewModel;

public class BookingViewModelTests
{
    private const int MockedMaxPassengers = 5;
    private const int LimitedMaxPassengers = 2;
    private const int DefaultFlightCapacity = 180;
    private const int TestFlightId = 1;
    private const int TestUserId = 1;
    private const float BaseTicketPrice = 100.0f;
    private const int DefaultRequestedPassengers = 1;
    private const int EventWaitDelayMs = 50;
    private const int MaxEventWaitRetries = 10;
    private const string TestEmail = "andrei.tudor@gmail.com";
    private const string TestAlternateEmail = "andrei@gmail.com";

    private readonly Mock<IBookingService> mockBookingService;
    private readonly Mock<IPricingService> mockPricingService;
    private readonly Mock<INavigationService> mockNavigationService;
    private readonly BookingViewModel viewModel;

    public BookingViewModelTests()
    {
        UserSession.CurrentUser = null;
        UserSession.PendingBookingParameters = null;
        mockBookingService = new Mock<IBookingService>();
        mockPricingService = new Mock<IPricingService>();
        mockNavigationService = new Mock<INavigationService>();
        mockBookingService.Setup(serviceReturningMockedCapacity => serviceReturningMockedCapacity.CalculateMaxPassengers(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(MockedMaxPassengers);
        viewModel = new BookingViewModel(mockBookingService.Object, mockPricingService.Object, mockNavigationService.Object);
    }

    [TestMethod]
    public void AddPassengerCommand_ValidCapacity_AddsPassenger()
    {
        viewModel.MaximumPassengers = LimitedMaxPassengers;
        viewModel.Passengers.Clear();

        viewModel.AddPassengerCommand.Execute(null);
        viewModel.Passengers.Count.Should().Be(1);

        viewModel.AddPassengerCommand.Execute(null);
        viewModel.Passengers.Count.Should().Be(2);

        viewModel.AddPassengerCommand.Execute(null);
        viewModel.Passengers.Count.Should().Be(2);
    }

    [TestMethod]
    public void RemovePassengerCommand_MultipleExist_RemovesPassenger()
    {
        var passenger1 = new PassengerFormViewModel();
        var passenger2 = new PassengerFormViewModel();
        viewModel.Passengers.Clear();
        viewModel.Passengers.Add(passenger1);
        viewModel.Passengers.Add(passenger2);

        viewModel.RemovePassengerCommand.Execute(passenger1);

        viewModel.Passengers.Count.Should().Be(1);
        viewModel.Passengers[0].Should().Be(passenger2);
    }

    [TestMethod]
    public void RemovePassengerCommand_OnlyOnePassenger_DoesNotRemove()
    {
        var passenger = new PassengerFormViewModel();
        viewModel.Passengers.Clear();
        viewModel.Passengers.Add(passenger);

        viewModel.RemovePassengerCommand.Execute(passenger);

        viewModel.Passengers.Count.Should().Be(1);
    }

    [TestMethod]
    public async Task ConfirmBookingCommand_Invoked_CallsServiceAndRaisesEventAsync()
    {
        var flight = new Flight { Id = TestFlightId, Route = new Route { Capacity = DefaultFlightCapacity } };
        var customer = new Customer { Id = TestUserId, Email = TestEmail };

        mockBookingService.Setup(bookingServiceReturningEmptyAddOns => bookingServiceReturningEmptyAddOns.GetAvailableAddOnsAsync()).ReturnsAsync(new List<AddOn>());
        mockBookingService.Setup(bookingServiceReturningEmptyOccupiedSeats => bookingServiceReturningEmptyOccupiedSeats.GetOccupiedSeatsAsync(It.IsAny<int>())).ReturnsAsync(new List<string>());
        mockBookingService.Setup(bookingServiceReturningMaxPassengers => bookingServiceReturningMaxPassengers.CalculateMaxPassengers(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(MockedMaxPassengers);
        mockBookingService.Setup(bookingServiceReturningCreatedTickets => bookingServiceReturningCreatedTickets.CreateTickets(It.IsAny<Flight>(), It.IsAny<Customer>(), It.IsAny<List<PassengerData>>(), It.IsAny<float>()))
            .Returns(new List<FlightTicket> { new FlightTicket() });
        mockBookingService.Setup(bookingServiceReturningSuccessfulSave => bookingServiceReturningSuccessfulSave.SaveTicketsAsync(It.IsAny<List<FlightTicket>>())).ReturnsAsync(true);
        mockBookingService.Setup(bookingServiceReturningValidPassengers => bookingServiceReturningValidPassengers.ValidatePassengers(It.IsAny<List<PassengerData>>())).Returns(string.Empty);
        mockPricingService.Setup(pricingServiceReturningBreakdown => pricingServiceReturningBreakdown.CalculatePriceBreakdown(It.IsAny<Flight>(), It.IsAny<Customer>(), It.IsAny<List<FlightTicket>>()))
            .Returns(new PriceBreakdown { FinalTotal = BaseTicketPrice });

        await viewModel.InitializeAsync(flight, customer, DefaultRequestedPassengers);

        var passenger = viewModel.Passengers[0];
        passenger.FirstName = "Andrei";
        passenger.LastName = "Tudor";
        passenger.Email = TestAlternateEmail;
        passenger.SelectedSeat = "1A";

        var isBookingConfirmedRaised = false;
        viewModel.BookingConfirmed += (sender, eventArgs) => isBookingConfirmedRaised = true;

        viewModel.ConfirmBookingCommand.Execute(null);

        int retries = MaxEventWaitRetries;
        while (!isBookingConfirmedRaised && retries > 0)
        {
            await Task.Delay(EventWaitDelayMs);
            retries--;
        }

        mockBookingService.Verify(bookingServiceToVerifySave => bookingServiceToVerifySave.SaveTicketsAsync(It.IsAny<List<FlightTicket>>()), Times.Once);
        isBookingConfirmedRaised.Should().BeTrue();
    }

    [TestMethod]
    public async Task OnNavigatedToAsync_NotAuthenticated_RedirectsToAuthenticationAsync()
    {
        UserSession.CurrentUser = null;
        var flight = new Flight
        {
            Id = TestFlightId,
            Route = new Route { Capacity = DefaultFlightCapacity, DepartureTime = DateTime.Now, ArrivalTime = DateTime.Now.AddHours(2) }
        };

        await viewModel.OnNavigatedToAsync(new object[] { flight });

        mockNavigationService.Verify(navServiceToVerifyAuthRedirect => navServiceToVerifyAuthRedirect.NavigateTo(typeof(AirportApp.Src.View.AuthPage), null), Times.Once);
    }

    [TestMethod]
    public async Task OnNavigatedToAsync_NoFlight_ReturnsFalseAsync()
    {
        var navigationResult = await viewModel.OnNavigatedToAsync(new object?[] { null });

        navigationResult.Should().BeFalse();
    }
}





