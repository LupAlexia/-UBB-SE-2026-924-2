using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Xunit;
using TicketManager.Domain;
using TicketManager.Service;
using TicketManager.ViewModel;

[assembly: Xunit.CollectionBehavior(DisableTestParallelization = true)]

namespace TicketManager.Tests.Unit.ViewModel
{
    public class FlightSearchViewModelTests
    {
        private readonly Mock<IFlightSearchService> mockFlightSearchService;
        private readonly Mock<INavigationService> mockNavigationService;
        private readonly Mock<IPricingService> mockPricingService;
        private readonly FlightSearchViewModel viewModel;

        public FlightSearchViewModelTests()
        {
            mockFlightSearchService = new Mock<IFlightSearchService>();
            mockNavigationService = new Mock<INavigationService>();
            mockPricingService = new Mock<IPricingService>();

            viewModel = new FlightSearchViewModel(
                mockFlightSearchService.Object,
                mockNavigationService.Object,
                mockPricingService.Object);

            // Ensure a clean session for every test execution
            UserSession.CurrentUser = null;
            UserSession.PendingBookingParameters = null;
        }

        [Fact]
        public void SearchCommand_ValidLocationAndDate_PopulatesAvailableFlightsCollection()
        {
            const string ValidLocation = "Cluj-Napoca";
            const int ExpectedPassengerCount = 2;
            const float DefaultBasePrice = 100.0f;
            const int ExpectedFlightId = 1;
            const int ExpectedCapacity = 150;

            var flightList = new List<Flight>
            {
                new Flight
                {
                    FlightId = ExpectedFlightId,
                    Route = new Route
                    {
                        Airport = new Airport { City = ValidLocation },
                        Capacity = ExpectedCapacity
                    }
                }
            };

            viewModel.Location = ValidLocation;
            viewModel.Passengers = ExpectedPassengerCount.ToString();
            viewModel.IsDeparture = true;

            mockFlightSearchService.Setup(service => service.ParsePassengerCount(ExpectedPassengerCount.ToString())).Returns(ExpectedPassengerCount);
            mockFlightSearchService.Setup(service => service.SearchFlights(ValidLocation, true, It.IsAny<DateTime?>(), ExpectedPassengerCount)).Returns(flightList);
            mockPricingService.Setup(service => service.CalculateBasePrice(It.IsAny<Flight>())).Returns(DefaultBasePrice);

            viewModel.SearchCommand.Execute(null);

            Assert.True(viewModel.AvailableFlights.Count > 0, "AvailableFlights collection should be populated with the search results.");
            Assert.Equal(ValidLocation, viewModel.AvailableFlights.First().Flight.Route?.Airport?.City);
        }

        [Fact]
        public void SearchCommand_EmptyResults_SetsSearchResultMessageToNoFlightsFound()
        {
            const string ValidLocation = "Bucuresti";
            const string ExpectedNoResultsMessage = "No flights found for the selected criteria.";

            viewModel.Location = ValidLocation;

            mockFlightSearchService.Setup(service => service.SearchFlights(ValidLocation, true, It.IsAny<DateTime?>(), It.IsAny<int?>()))
                .Returns(new List<Flight>());

            viewModel.SearchCommand.Execute(null);

            Assert.Empty(viewModel.AvailableFlights);
            Assert.Equal(ExpectedNoResultsMessage, viewModel.SearchResultMessage);
        }

        [Fact]
        public void SearchCommand_EmptyLocation_DoesNotInvokeSearchService()
        {
            viewModel.Location = string.Empty;

            viewModel.SearchCommand.Execute(null);

            mockFlightSearchService.Verify(service => service.SearchFlights(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<DateTime?>(), It.IsAny<int?>()), Times.Never);
        }

        [Fact]
        public void BookFlightCommand_UserNotLoggedIn_NavigatesToAuthenticationPage()
        {
            UserSession.CurrentUser = null;
            const float DefaultBasePrice = 100.0f;
            const int ExpectedFlightId = 1;
            var flightDisplayModel = new FlightDisplayModel(new Flight { FlightId = ExpectedFlightId }, DefaultBasePrice);
            const string PassengerInput = "1";
            viewModel.Passengers = PassengerInput;

            mockFlightSearchService.Setup(service => service.ParsePassengerCount(PassengerInput)).Returns(1);

            viewModel.BookFlightCommand.Execute(flightDisplayModel);

            mockNavigationService.Verify(service => service.NavigateTo(typeof(TicketManager.View.AuthPage), It.IsAny<object>()), Times.Once);
            Assert.NotNull(UserSession.PendingBookingParameters);
        }

        [Fact]
        public void BookFlightCommand_UserLoggedIn_NavigatesToBookingPage()
        {
            const int ExpectedUserId = 1;
            const string ExpectedEmailAddress = "test@test.com";
            UserSession.CurrentUser = new User { UserId = ExpectedUserId, Email = ExpectedEmailAddress };

            const float DefaultBasePrice = 100.0f;
            const int ExpectedFlightId = 1;
            var flightDisplayModel = new FlightDisplayModel(new Flight { FlightId = ExpectedFlightId }, DefaultBasePrice);
            const string PassengerInput = "1";
            viewModel.Passengers = PassengerInput;

            mockFlightSearchService.Setup(service => service.ParsePassengerCount(PassengerInput)).Returns(1);

            viewModel.BookFlightCommand.Execute(flightDisplayModel);

            mockNavigationService.Verify(service => service.NavigateTo(typeof(TicketManager.View.BookingPage), It.IsAny<object[]>()), Times.Once);
        }
    }
}
