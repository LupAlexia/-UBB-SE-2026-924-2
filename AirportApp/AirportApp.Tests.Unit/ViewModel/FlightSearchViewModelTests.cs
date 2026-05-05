using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.Src.Service;
using AirportApp.Src.ViewModel;
using AirportApp.Src.Service.Interfaces;
using AirportEntity = AirportApp.ClassLibrary.Entity.Domain.Airport;

namespace AirportApp.Tests.Unit.ViewModel
{
    [TestClass]
    [DoNotParallelize]
    public class FlightSearchViewModelTests
    {
        private const int ExpectedUserId = 1;
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

        [TestMethod]
        public async Task SearchCommand_ValidLocationAndDate_PopulatesAvailableFlightsCollection()
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
                    Id = ExpectedFlightId,
                    Route = new Route
                    {
                        Airport = new AirportEntity { City = ValidLocation },
                        Capacity = ExpectedCapacity
                    }
                }
            };

            viewModel.Location = ValidLocation;
            viewModel.Passengers = ExpectedPassengerCount.ToString();
            viewModel.IsDeparture = true;

            mockFlightSearchService.Setup(service => service.ParsePassengerCount(ExpectedPassengerCount.ToString())).Returns(ExpectedPassengerCount);
            mockFlightSearchService.Setup(service => service.SearchFlightsAsync(ValidLocation, true, It.IsAny<DateTime?>(), ExpectedPassengerCount)).ReturnsAsync(flightList);
            mockPricingService.Setup(service => service.CalculateBasePrice(It.IsAny<Flight>())).Returns(DefaultBasePrice);

            viewModel.SearchCommand.Execute(null);

            Assert.IsTrue(viewModel.AvailableFlights.Count > 0, "AvailableFlights collection should be populated with the search results.");
            Assert.AreEqual(ValidLocation, viewModel.AvailableFlights.First().Flight.Route?.Airport?.City);
        }

        [TestMethod]
        public async Task SearchCommand_EmptyResults_SetsSearchResultMessageToNoFlightsFound()
        {
            const string ValidLocation = "Bucuresti";
            const string ExpectedNoResultsMessage = "No flights found for the selected criteria.";

            viewModel.Location = ValidLocation;

            mockFlightSearchService.Setup(service => service.SearchFlightsAsync(ValidLocation, true, It.IsAny<DateTime?>(), It.IsAny<int?>()))
                .ReturnsAsync(new List<Flight>());

            viewModel.SearchCommand.Execute(null);

            Assert.AreEqual(0, viewModel.AvailableFlights.Count);
            Assert.AreEqual(ExpectedNoResultsMessage, viewModel.SearchResultMessage);
        }

        [TestMethod]
        public void SearchCommand_EmptyLocation_DoesNotInvokeSearchService()
        {
            viewModel.Location = string.Empty;

            viewModel.SearchCommand.Execute(null);

            mockFlightSearchService.Verify(service => service.SearchFlightsAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<DateTime?>(), It.IsAny<int?>()), Times.Never);
        }

        [TestMethod]
        public void BookFlightCommand_UserNotLoggedIn_NavigatesToAuthenticationPage()
        {
            UserSession.CurrentUser = null;
            const float DefaultBasePrice = 100.0f;
            const int ExpectedFlightId = 1;
            var flightDisplayModel = new FlightDisplayModel(new Flight { Id = ExpectedFlightId }, DefaultBasePrice);
            const string PassengerInput = "1";
            viewModel.Passengers = PassengerInput;

            mockFlightSearchService.Setup(service => service.ParsePassengerCount(PassengerInput)).Returns(1);

            viewModel.BookFlightCommand.Execute(flightDisplayModel);

            mockNavigationService.Verify(service => service.NavigateTo(It.IsAny<Type>(), It.IsAny<object?>()), Times.AtLeastOnce);
            Assert.IsNotNull(UserSession.PendingBookingParameters);
        }

        [TestMethod]
        public void BookFlightCommand_UserLoggedIn_NavigatesToBookingPage()
        {
            const string ExpectedEmailAddress = "test@test.com";
            UserSession.CurrentUser = new Customer { Id = ExpectedUserId, Email = ExpectedEmailAddress };

            const float DefaultBasePrice = 100.0f;
            const int ExpectedFlightId = 1;
            var flightDisplayModel = new FlightDisplayModel(new Flight { Id = ExpectedFlightId }, DefaultBasePrice);
            const string PassengerInput = "1";
            viewModel.Passengers = PassengerInput;

            mockFlightSearchService.Setup(service => service.ParsePassengerCount(PassengerInput)).Returns(1);

            viewModel.BookFlightCommand.Execute(flightDisplayModel);

            mockNavigationService.Verify(service => service.NavigateTo(It.IsAny<Type>(), It.IsAny<object?>()), Times.AtLeastOnce);
        }
    }
}
