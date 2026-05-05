
using AirportApp.Src.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AirportApp.Tests.Unit.Services;

public class DashboardServiceTests
{
    private const int TargetUserId = 1;
    private const string PastFilter = "Past";
    private const string UpcomingFilter = "Upcoming";
    private const int PastDaysOffset = -5;
    private const int UpcomingDaysOffset = 5;

    private readonly Mock<IFlightTicketRepository> mockTicketRepository;
    private readonly DashboardService dashboardService;

    public DashboardServiceTests()
    {
        mockTicketRepository = new Mock<IFlightTicketRepository>();
        dashboardService = new DashboardService(mockTicketRepository.Object);
    }

    [TestMethod]
    public void GetUserTickets_TicketsWithNoFlight_ExcludesThem()
    {
        var ticketWithFlight = new FlightTicket { Flight = new Flight { Date = DateTime.Now.AddDays(UpcomingDaysOffset) } };
        var ticketWithoutFlight = new FlightTicket { Flight = null };

        mockTicketRepository.Setup(repo => repo.GetTicketsByUserIdAsync(TargetUserId))
            .ReturnsAsync(new List<FlightTicket> { ticketWithFlight, ticketWithoutFlight });

        var results = dashboardService.GetUserTicketsAsync(TargetUserId, UpcomingFilter).Result.ToList();

        results.Should().ContainSingle();
        results.First().Should().Be(ticketWithFlight);
    }

    [TestMethod]
    public void GetUserTickets_PastFlights_FiltersAndSortsDescending()
    {
        var olderFlight = new Flight { Date = DateTime.Now.AddDays(PastDaysOffset - 2) };
        var recentPastFlight = new Flight { Date = DateTime.Now.AddDays(PastDaysOffset) };
        var futureFlight = new Flight { Date = DateTime.Now.AddDays(UpcomingDaysOffset) };

        var ticketOlder = new FlightTicket { Flight = olderFlight };
        var ticketRecentPast = new FlightTicket { Flight = recentPastFlight };
        var ticketFuture = new FlightTicket { Flight = futureFlight };

        mockTicketRepository.Setup(repo => repo.GetTicketsByUserIdAsync(TargetUserId))
            .ReturnsAsync(new List<FlightTicket> { ticketOlder, ticketFuture, ticketRecentPast });

        var results = dashboardService.GetUserTicketsAsync(TargetUserId, PastFilter).Result.ToList();

        results.Should().HaveCount(2);
        results.First().Should().Be(ticketRecentPast);
        results.Last().Should().Be(ticketOlder);
    }

    [TestMethod]
    public void GetUserTickets_UpcomingFlights_FiltersAndSortsAscending()
    {
        var olderFlight = new Flight { Date = DateTime.Now.AddDays(PastDaysOffset) };
        var nearFutureFlight = new Flight { Date = DateTime.Now.AddDays(UpcomingDaysOffset) };
        var farFutureFlight = new Flight { Date = DateTime.Now.AddDays(UpcomingDaysOffset + 2) };

        var ticketOlder = new FlightTicket { Flight = olderFlight };
        var ticketNearFuture = new FlightTicket { Flight = nearFutureFlight };
        var ticketFarFuture = new FlightTicket { Flight = farFutureFlight };

        mockTicketRepository.Setup(repo => repo.GetTicketsByUserIdAsync(TargetUserId))
            .ReturnsAsync(new List<FlightTicket> { ticketFarFuture, ticketOlder, ticketNearFuture });

        var results = dashboardService.GetUserTicketsAsync(TargetUserId, UpcomingFilter).Result.ToList();

        results.Should().HaveCount(2);
        results.First().Should().Be(ticketNearFuture);
        results.Last().Should().Be(ticketFarFuture);
    }
}



