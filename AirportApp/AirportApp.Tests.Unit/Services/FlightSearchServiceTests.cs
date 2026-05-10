using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AirportApp.Tests.Unit.Services
{
    [TestClass]
    public class FlightSearchServiceTests
    {
        private const int FlightId1 = 1;
        private const int FlightId2 = 2;
        private const int FlightId3 = 3;
        private const int DefaultCapacity = 100;
        private const int OccupiedSeatsLow = 10;
        private const int SinglePassenger = 1;
        private const int GroupPassengers = 10;
        private const int DaysOffsetTarget = 3;
        private const int DaysOffsetNoMatch = 5;
        private const int ExpectedFlightsCountMatching = 2;
        private const int ExpectedFlightsCountFiltered = 1;
        private const int Flight1OccupiedSeats = 95;
        private const int Flight2OccupiedSeats = 99;
        private const int Flight3OccupiedSeats = 90;
        private const string BucharestLocation = "Bucuresti";
        private const string ClujNapovaLocation = "Cluj-Napoca";
        private const string FlightNumber1 = "RO101";
        private const string FlightNumber2 = "RO102";
        private const string FlightNumber3 = "RO103";
        private const string GenericLocation = "location";
        private const string NullPassengerInput = null;
        private const string EmptyPassengerInput = "   ";
        private const string ValidPassengerInputStr = "3";
        private const int ValidPassengerInputInt = 3;
        private const string ZeroPassengerInput = "0";
        private const string NegativePassengerInput = "-2";
        private const string InvalidTextPassengerInput = "abc";
        private const int DefaultPassengerFallback = 1;

        private readonly Mock<IFlightRepository> mockFlightRepository;
        private readonly FlightSearchService flightSearchService;

        public FlightSearchServiceTests()
        {
            mockFlightRepository = new Mock<IFlightRepository>();
            flightSearchService = new FlightSearchService(mockFlightRepository.Object);
        }

        [TestMethod]
        public void SearchFlights_MatchingCriteria_ReturnsFlights()
        {
            var flights = new List<Flight>
            {
                new Flight { Id = FlightId1, FlightNumber = FlightNumber1, Route = new Route { Capacity = DefaultCapacity } },
                new Flight { Id = FlightId2, FlightNumber = FlightNumber2, Route = new Route { Capacity = DefaultCapacity } }
            };
            mockFlightRepository.Setup(r => r.GetFlightsByRouteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>())).ReturnsAsync(flights);
            mockFlightRepository.Setup(r => r.GetOccupiedSeatCountAsync(It.IsAny<int>())).ReturnsAsync(OccupiedSeatsLow);

            var result = flightSearchService.SearchFlightsAsync(BucharestLocation, true, DateTime.Now.AddDays(DaysOffsetTarget), SinglePassenger).Result;

            result.Should().HaveCount(ExpectedFlightsCountMatching);
        }

        [TestMethod]
        public void SearchFlights_NullLocation_ReturnsEmptyList()
        {
            var result = flightSearchService.SearchFlightsAsync(null!, true, null, null).Result;
            result.Should().BeEmpty();
            mockFlightRepository.Verify(r => r.GetFlightsByRouteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>()), Times.Never);
        }

        [TestMethod]
        public void SearchFlights_WhitespaceLocation_ReturnsEmptyList()
        {
            var result = flightSearchService.SearchFlightsAsync("   ", true, null, null).Result;
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void SearchFlights_IsDepartureTrue_PassesDepartureRouteType()
        {
            mockFlightRepository.Setup(r => r.GetFlightsByRouteAsync(It.IsAny<string>(), "Departure", It.IsAny<DateTime?>())).ReturnsAsync(new List<Flight>());

            flightSearchService.SearchFlightsAsync(BucharestLocation, true, null, null).Wait();

            mockFlightRepository.Verify(r => r.GetFlightsByRouteAsync(BucharestLocation, "Departure", null), Times.Once);
        }

        [TestMethod]
        public void SearchFlights_IsDepartureFalse_PassesArrivalRouteType()
        {
            mockFlightRepository.Setup(r => r.GetFlightsByRouteAsync(It.IsAny<string>(), "Arrival", It.IsAny<DateTime?>())).ReturnsAsync(new List<Flight>());

            flightSearchService.SearchFlightsAsync(BucharestLocation, false, null, null).Wait();

            mockFlightRepository.Verify(r => r.GetFlightsByRouteAsync(BucharestLocation, "Arrival", null), Times.Once);
        }

        [TestMethod]
        public void SearchFlights_NoMatches_ReturnsEmptyList()
        {
            mockFlightRepository.Setup(r => r.GetFlightsByRouteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>())).ReturnsAsync(new List<Flight>());

            var result = flightSearchService.SearchFlightsAsync(ClujNapovaLocation, true, DateTime.Now.AddDays(DaysOffsetNoMatch), SinglePassenger).Result;

            result.Should().BeEmpty();
        }

        [TestMethod]
        public void SearchFlights_NullPassengerCount_ReturnsAllFlightsWithoutCapacityFilter()
        {
            var flights = new List<Flight>
            {
                new Flight { Id = FlightId1, Route = new Route { Capacity = DefaultCapacity } },
                new Flight { Id = FlightId2, Route = new Route { Capacity = DefaultCapacity } }
            };
            mockFlightRepository.Setup(r => r.GetFlightsByRouteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>())).ReturnsAsync(flights);

            var result = flightSearchService.SearchFlightsAsync(GenericLocation, true, null, null).Result;

            result.Should().HaveCount(2);
            mockFlightRepository.Verify(r => r.GetOccupiedSeatCountAsync(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void SearchFlights_ZeroPassengerCount_ReturnsAllFlightsWithoutCapacityFilter()
        {
            var flights = new List<Flight>
            {
                new Flight { Id = FlightId1, Route = new Route { Capacity = DefaultCapacity } }
            };
            mockFlightRepository.Setup(r => r.GetFlightsByRouteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>())).ReturnsAsync(flights);

            var result = flightSearchService.SearchFlightsAsync(GenericLocation, true, null, 0).Result;

            result.Should().HaveCount(1);
            mockFlightRepository.Verify(r => r.GetOccupiedSeatCountAsync(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void SearchFlights_CapacityFilter_FiltersFlights()
        {
            var flight1 = new Flight { Id = FlightId1, FlightNumber = FlightNumber1, Route = new Route { Capacity = DefaultCapacity } };
            var flight2 = new Flight { Id = FlightId2, FlightNumber = FlightNumber2, Route = new Route { Capacity = DefaultCapacity } };
            var flight3 = new Flight { Id = FlightId3, FlightNumber = FlightNumber3, Route = new Route { Capacity = DefaultCapacity } };

            mockFlightRepository.Setup(r => r.GetFlightsByRouteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>())).ReturnsAsync(new List<Flight> { flight1, flight2, flight3 });
            mockFlightRepository.Setup(r => r.GetOccupiedSeatCountAsync(FlightId1)).ReturnsAsync(Flight1OccupiedSeats);
            mockFlightRepository.Setup(r => r.GetOccupiedSeatCountAsync(FlightId2)).ReturnsAsync(Flight2OccupiedSeats);
            mockFlightRepository.Setup(r => r.GetOccupiedSeatCountAsync(FlightId3)).ReturnsAsync(Flight3OccupiedSeats);

            var result = flightSearchService.SearchFlightsAsync(GenericLocation, true, null, GroupPassengers).Result;

            result.Should().HaveCount(ExpectedFlightsCountFiltered);
            result.First().Id.Should().Be(FlightId3);
        }

        private const int FullCapacity = 10;

        [TestMethod]
        public void SearchFlights_AllFlightsAtFullCapacity_ReturnsEmpty()
        {
            var flight1 = new Flight { Id = FlightId1, Route = new Route { Capacity = FullCapacity } };
            mockFlightRepository.Setup(r => r.GetFlightsByRouteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>())).ReturnsAsync(new List<Flight> { flight1 });
            mockFlightRepository.Setup(r => r.GetOccupiedSeatCountAsync(FlightId1)).ReturnsAsync(FullCapacity);

            var result = flightSearchService.SearchFlightsAsync(GenericLocation, true, null, SinglePassenger).Result;

            result.Should().BeEmpty();
        }

        [TestMethod]
        public void ParsePassengerCount_NullInput_ReturnsNull()
        {
            flightSearchService.ParsePassengerCount(NullPassengerInput).Should().BeNull();
        }

        [TestMethod]
        public void ParsePassengerCount_WhitespaceInput_ReturnsNull()
        {
            flightSearchService.ParsePassengerCount(EmptyPassengerInput).Should().BeNull();
        }

        [TestMethod]
        public void ParsePassengerCount_ValidPositiveNumber_ReturnsParsedValue()
        {
            flightSearchService.ParsePassengerCount(ValidPassengerInputStr).Should().Be(ValidPassengerInputInt);
        }

        [TestMethod]
        public void ParsePassengerCount_ZeroInput_ReturnsDefault()
        {
            flightSearchService.ParsePassengerCount(ZeroPassengerInput).Should().Be(DefaultPassengerFallback);
        }

        [TestMethod]
        public void ParsePassengerCount_NegativeInput_ReturnsDefault()
        {
            flightSearchService.ParsePassengerCount(NegativePassengerInput).Should().Be(DefaultPassengerFallback);
        }

        [TestMethod]
        public void ParsePassengerCount_InvalidText_ReturnsDefault()
        {
            flightSearchService.ParsePassengerCount(InvalidTextPassengerInput).Should().Be(DefaultPassengerFallback);
        }

        [TestMethod]
        public void ParsePassengerCount_LargeValidNumber_ReturnsParsedValue()
        {
            flightSearchService.ParsePassengerCount(DefaultCapacity.ToString()).Should().Be(DefaultCapacity);
        }
    }
}