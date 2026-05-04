using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using TicketManager.Domain;
using TicketManager.Repository;
using TicketManager.Service;

namespace TicketManager.Tests.Unit.Services;

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

    [Fact]
    public void SearchFlights_MatchingCriteria_ReturnsFlights()
    {
        var flights = new List<Flight>
        {
            new Flight { FlightId = FlightId1, FlightNumber = FlightNumber1, Route = new Route { Capacity = DefaultCapacity } },
            new Flight { FlightId = FlightId2, FlightNumber = FlightNumber2, Route = new Route { Capacity = DefaultCapacity } }
        };
        mockFlightRepository.Setup(repoWithMatchingFlights => repoWithMatchingFlights.GetFlightsByRoute(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>()))
            .Returns(flights);
        mockFlightRepository.Setup(repoWithAvailableSeats => repoWithAvailableSeats.GetOccupiedSeatCount(It.IsAny<int>())).Returns(OccupiedSeatsLow);

        var foundFlights = flightSearchService.SearchFlights(BucharestLocation, true, DateTime.Now.AddDays(DaysOffsetTarget), SinglePassenger);

        foundFlights.Should().NotBeNull();
        foundFlights.Should().HaveCount(ExpectedFlightsCountMatching);
    }

    [Fact]
    public void SearchFlights_NoMatches_ReturnsEmptyList()
    {
        mockFlightRepository.Setup(repoWithNoMatchingFlights => repoWithNoMatchingFlights.GetFlightsByRoute(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>()))
            .Returns(new List<Flight>());

        var foundFlights = flightSearchService.SearchFlights(ClujNapovaLocation, true, DateTime.Now.AddDays(DaysOffsetNoMatch), SinglePassenger);

        foundFlights.Should().NotBeNull();
        foundFlights.Should().BeEmpty();
    }

    [Fact]
    public void SearchFlights_CapacityFilter_FiltersFlights()
    {
        var flight1 = new Flight { FlightId = FlightId1, FlightNumber = FlightNumber1, Route = new Route { Capacity = DefaultCapacity } };
        var flight2 = new Flight { FlightId = FlightId2, FlightNumber = FlightNumber2, Route = new Route { Capacity = DefaultCapacity } };
        var flight3 = new Flight { FlightId = FlightId3, FlightNumber = FlightNumber3, Route = new Route { Capacity = DefaultCapacity } };
        var flights = new List<Flight> { flight1, flight2, flight3 };

        mockFlightRepository.Setup(repoWithMultipleFlights => repoWithMultipleFlights.GetFlightsByRoute(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>()))
            .Returns(flights);
        mockFlightRepository.Setup(repoWithFlight1Seats => repoWithFlight1Seats.GetOccupiedSeatCount(FlightId1)).Returns(Flight1OccupiedSeats);
        mockFlightRepository.Setup(repoWithFlight2Seats => repoWithFlight2Seats.GetOccupiedSeatCount(FlightId2)).Returns(Flight2OccupiedSeats);
        mockFlightRepository.Setup(repoWithFlight3Seats => repoWithFlight3Seats.GetOccupiedSeatCount(FlightId3)).Returns(Flight3OccupiedSeats);

        var foundFlights = flightSearchService.SearchFlights(GenericLocation, true, null, GroupPassengers);

        foundFlights.Should().HaveCount(ExpectedFlightsCountFiltered);
        foundFlights.First().FlightId.Should().Be(FlightId3);
    }

    [Fact]
    public void ParsePassengerCount_EmptyInput_ReturnsNull()
    {
        var resultNull = flightSearchService.ParsePassengerCount(NullPassengerInput);
        resultNull.Should().BeNull();

        var resultEmpty = flightSearchService.ParsePassengerCount(EmptyPassengerInput);
        resultEmpty.Should().BeNull();
    }

    [Fact]
    public void ParsePassengerCount_ValidPositiveNumber_ReturnsParsedValue()
    {
        var parsedValue = flightSearchService.ParsePassengerCount(ValidPassengerInputStr);
        parsedValue.Should().Be(ValidPassengerInputInt);
    }

    [Fact]
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

