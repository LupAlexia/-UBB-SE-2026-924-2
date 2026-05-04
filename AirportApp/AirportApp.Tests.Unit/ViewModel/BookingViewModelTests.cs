using System.Collections.ObjectModel;
using FluentAssertions;
using Moq;
using TicketManager.Domain;
using TicketManager.Service;
using TicketManager.ViewModel;

namespace TicketManager.Tests.Unit.ViewModel;

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

    [Fact]
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

    [Fact]
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

    [Fact]
    public void RemovePassengerCommand_OnlyOnePassenger_DoesNotRemove()
    {
        var passenger = new PassengerFormViewModel();
        viewModel.Passengers.Clear();
        viewModel.Passengers.Add(passenger);

        viewModel.RemovePassengerCommand.Execute(passenger);

        viewModel.Passengers.Count.Should().Be(1);
    }

    [Fact]
    public async Task ConfirmBookingCommand_Invoked_CallsServiceAndRaisesEvent()
    {
        var flight = new Flight { FlightId = TestFlightId, Route = new Route { Capacity = DefaultFlightCapacity } };
        var user = new User { UserId = TestUserId, Email = TestEmail };

        mockBookingService.Setup(bookingServiceReturningEmptyAddOns => bookingServiceReturningEmptyAddOns.GetAvailableAddOnsAsync()).ReturnsAsync(new List<AddOn>());
        mockBookingService.Setup(bookingServiceReturningEmptyOccupiedSeats => bookingServiceReturningEmptyOccupiedSeats.GetOccupiedSeatsAsync(It.IsAny<int>())).ReturnsAsync(new List<string>());
        mockBookingService.Setup(bookingServiceReturningMaxPassengers => bookingServiceReturningMaxPassengers.CalculateMaxPassengers(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(MockedMaxPassengers);
        mockBookingService.Setup(bookingServiceReturningCreatedTickets => bookingServiceReturningCreatedTickets.CreateTickets(It.IsAny<Flight>(), It.IsAny<User>(), It.IsAny<List<PassengerData>>(), It.IsAny<float>()))
            .Returns(new List<Ticket> { new Ticket() });
        mockBookingService.Setup(bookingServiceReturningSuccessfulSave => bookingServiceReturningSuccessfulSave.SaveTicketsAsync(It.IsAny<List<Ticket>>())).ReturnsAsync(true);
        mockBookingService.Setup(bookingServiceReturningValidPassengers => bookingServiceReturningValidPassengers.ValidatePassengers(It.IsAny<List<PassengerData>>())).Returns(string.Empty);
        mockPricingService.Setup(pricingServiceReturningBreakdown => pricingServiceReturningBreakdown.CalculatePriceBreakdown(It.IsAny<Flight>(), It.IsAny<User>(), It.IsAny<List<Ticket>>()))
            .Returns(new PriceBreakdown { FinalTotal = BaseTicketPrice });

        await viewModel.InitializeAsync(flight, user, DefaultRequestedPassengers);

        var passenger = viewModel.Passengers[0];
        passenger.FirstName = "Andrei";
        passenger.LastName = "Tudor";
        passenger.Email = TestAlternateEmail;
        passenger.SelectedSeat = "1A";

        var bookingConfirmedRaised = false;
        viewModel.BookingConfirmed += (sender, eventArgs) => bookingConfirmedRaised = true;

        viewModel.ConfirmBookingCommand.Execute(null);

        int retries = MaxEventWaitRetries;
        while (!bookingConfirmedRaised && retries > 0)
        {
            await Task.Delay(EventWaitDelayMs);
            retries--;
        }

        mockBookingService.Verify(bookingServiceToVerifySave => bookingServiceToVerifySave.SaveTicketsAsync(It.IsAny<List<Ticket>>()), Times.Once);
        bookingConfirmedRaised.Should().BeTrue();
    }

    [Fact]
    public async Task OnNavigatedToAsynchronous_NotAuthenticated_RedirectsToAuthentication()
    {
        UserSession.CurrentUser = null;
        var flight = new Flight
        {
            FlightId = TestFlightId,
            Route = new Route { Capacity = DefaultFlightCapacity, DepartureTime = DateTime.Now, ArrivalTime = DateTime.Now.AddHours(2) }
        };

        await viewModel.OnNavigatedToAsync(new object[] { flight });

        mockNavigationService.Verify(navServiceToVerifyAuthRedirect => navServiceToVerifyAuthRedirect.NavigateTo(typeof(View.AuthPage), null), Times.Once);
    }

    [Fact]
    public async Task OnNavigatedToAsynchronous_NoFlight_ReturnsFalse()
    {
        var navigationResult = await viewModel.OnNavigatedToAsync(new object?[] { null });

        navigationResult.Should().BeFalse();
    }
}

