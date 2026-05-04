using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using TicketManager.Domain;
using TicketManager.Repository;
using TicketManager.Service;
using Xunit;

namespace TicketManager.Tests.Unit.Services;

public class DashboardServiceTests
{
    private const int TargetUserId = 1;
    private const string PastFilter = "Past";
    private const string UpcomingFilter = "Upcoming";
    private const int PastDaysOffset = -5;
    private const int UpcomingDaysOffset = 5;

    private readonly Mock<ITicketRepository> mockTicketRepository;
    private readonly DashboardService dashboardService;

    public DashboardServiceTests()
    {
        mockTicketRepository = new Mock<ITicketRepository>();
        dashboardService = new DashboardService(mockTicketRepository.Object);
    }

    [Fact]
    public void GetUserTickets_TicketsWithNoFlight_ExcludesThem()
    {
        var ticketWithFlight = new Ticket { Flight = new Flight { Date = DateTime.Now.AddDays(UpcomingDaysOffset) } };
        var ticketWithoutFlight = new Ticket { Flight = null };

        mockTicketRepository.Setup(repo => repo.GetTicketsByUserId(TargetUserId))
            .Returns(new List<Ticket> { ticketWithFlight, ticketWithoutFlight });

        var results = dashboardService.GetUserTickets(TargetUserId, UpcomingFilter).ToList();

        results.Should().ContainSingle();
        results.First().Should().Be(ticketWithFlight);
    }

    [Fact]
    public void GetUserTickets_PastFlights_FiltersAndSortsDescending()
    {
        var olderFlight = new Flight { Date = DateTime.Now.AddDays(PastDaysOffset - 2) };
        var recentPastFlight = new Flight { Date = DateTime.Now.AddDays(PastDaysOffset) };
        var futureFlight = new Flight { Date = DateTime.Now.AddDays(UpcomingDaysOffset) };

        var ticketOlder = new Ticket { Flight = olderFlight };
        var ticketRecentPast = new Ticket { Flight = recentPastFlight };
        var ticketFuture = new Ticket { Flight = futureFlight };

        mockTicketRepository.Setup(repo => repo.GetTicketsByUserId(TargetUserId))
            .Returns(new List<Ticket> { ticketOlder, ticketFuture, ticketRecentPast });

        var results = dashboardService.GetUserTickets(TargetUserId, PastFilter).ToList();

        results.Should().HaveCount(2);
        results.First().Should().Be(ticketRecentPast);
        results.Last().Should().Be(ticketOlder);
    }

    [Fact]
    public void GetUserTickets_UpcomingFlights_FiltersAndSortsAscending()
    {
        var olderFlight = new Flight { Date = DateTime.Now.AddDays(PastDaysOffset) };
        var nearFutureFlight = new Flight { Date = DateTime.Now.AddDays(UpcomingDaysOffset) };
        var farFutureFlight = new Flight { Date = DateTime.Now.AddDays(UpcomingDaysOffset + 2) };

        var ticketOlder = new Ticket { Flight = olderFlight };
        var ticketNearFuture = new Ticket { Flight = nearFutureFlight };
        var ticketFarFuture = new Ticket { Flight = farFutureFlight };

        mockTicketRepository.Setup(repo => repo.GetTicketsByUserId(TargetUserId))
            .Returns(new List<Ticket> { ticketFarFuture, ticketOlder, ticketNearFuture });

        var results = dashboardService.GetUserTickets(TargetUserId, UpcomingFilter).ToList();

        results.Should().HaveCount(2);
        results.First().Should().Be(ticketNearFuture);
        results.Last().Should().Be(ticketFarFuture);
    }
}
