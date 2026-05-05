using AirportApp.Src.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service;

namespace AirportApp.Tests.Unit.Services;

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
        mockFlightRepository.Setup(repoWithMatchingFlights => repoWithMatchingFlights.GetFlightsByRouteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>()))
            .ReturnsAsync(flights);
        mockFlightRepository.Setup(repoWithAvailableSeats => repoWithAvailableSeats.GetOccupiedSeatCountAsync(It.IsAny<int>())).ReturnsAsync(OccupiedSeatsLow);

        var foundFlights = flightSearchService.SearchFlightsAsync(BucharestLocation, true, DateTime.Now.AddDays(DaysOffsetTarget), SinglePassenger);

        foundFlights.Result.Should().NotBeNull();
        foundFlights.Result.Should().HaveCount(ExpectedFlightsCountMatching);
    }

    [TestMethod]
    public void SearchFlights_NoMatches_ReturnsEmptyList()
    {
        mockFlightRepository.Setup(repoWithNoMatchingFlights => repoWithNoMatchingFlights.GetFlightsByRouteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>()))
            .ReturnsAsync(new List<Flight>());

        var foundFlights = flightSearchService.SearchFlightsAsync(ClujNapovaLocation, true, DateTime.Now.AddDays(DaysOffsetNoMatch), SinglePassenger);

        foundFlights.Result.Should().NotBeNull();
        foundFlights.Result.Should().BeEmpty();
    }

    [TestMethod]
    public void SearchFlights_CapacityFilter_FiltersFlights()
    {
        var flight1 = new Flight { Id = FlightId1, FlightNumber = FlightNumber1, Route = new Route { Capacity = DefaultCapacity } };
        var flight2 = new Flight { Id = FlightId2, FlightNumber = FlightNumber2, Route = new Route { Capacity = DefaultCapacity } };
        var flight3 = new Flight { Id = FlightId3, FlightNumber = FlightNumber3, Route = new Route { Capacity = DefaultCapacity } };
        var flights = new List<Flight> { flight1, flight2, flight3 };

        mockFlightRepository.Setup(repoWithMultipleFlights => repoWithMultipleFlights.GetFlightsByRouteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>()))
            .ReturnsAsync(flights);
        mockFlightRepository.Setup(repoWithFlight1Seats => repoWithFlight1Seats.GetOccupiedSeatCountAsync(FlightId1)).ReturnsAsync(Flight1OccupiedSeats);
        mockFlightRepository.Setup(repoWithFlight2Seats => repoWithFlight2Seats.GetOccupiedSeatCountAsync(FlightId2)).ReturnsAsync(Flight2OccupiedSeats);
        mockFlightRepository.Setup(repoWithFlight3Seats => repoWithFlight3Seats.GetOccupiedSeatCountAsync(FlightId3)).ReturnsAsync(Flight3OccupiedSeats);

        var foundFlights = flightSearchService.SearchFlightsAsync(GenericLocation, true, null, GroupPassengers);

        foundFlights.Result.Should().HaveCount(ExpectedFlightsCountFiltered);
        foundFlights.Result.First().Id.Should().Be(FlightId3);
    }

    [TestMethod]
    public void ParsePassengerCount_EmptyInput_ReturnsNull()
    {
        var resultNull = flightSearchService.ParsePassengerCount(NullPassengerInput);
        resultNull.Should().BeNull();

        var resultEmpty = flightSearchService.ParsePassengerCount(EmptyPassengerInput);
        resultEmpty.Should().BeNull();
    }

    [TestMethod]
    public void ParsePassengerCount_ValidPositiveNumber_ReturnsParsedValue()
    {
        var parsedValue = flightSearchService.ParsePassengerCount(ValidPassengerInputStr);
        parsedValue.Should().Be(ValidPassengerInputInt);
    }

    [TestMethod]
    public void ParsePassengerCount_InvalidNumber_ReturnsDefault()
    {
        var resultZero = flightSearchService.ParsePassengerCount(ZeroPassengerInput);
        resultZero.Should().Be(DefaultPassengerFallback);

        var resultNegative = flightSearchService.ParsePassengerCount(NegativePassengerInput);
        resultNegative.Should().Be(DefaultPassengerFallback);

        var resultText = flightSearchService.ParsePassengerCount(InvalidTextPassengerInput);
        resultText.Should().Be(DefaultPassengerFallback);
    }
}




