using AirportApp.Src.ViewModel;
using System;
using FluentAssertions;
using Moq;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service;
using AirportApp.Tests.Unit.Fixtures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AirportApp.Tests.Unit.Services;

[TestClass]
public class CancellationServiceTests
{
    private const int DefaultId = 1;
    private const int FutureFlightDaysOffset = 5;
    private const int PastFlightDaysOffset = -1;
    private const string ActiveStatus = "Active";
    private const string CancelledStatus = "Cancelled";
    private const string AlreadyCancelledMessage = "already cancelled";
    private const string PastFlightMessage = "in the past";

    private readonly Mock<IFlightTicketRepository> mockTicketRepository;
    private readonly CancellationService cancellationService;

    public CancellationServiceTests()
    {
        mockTicketRepository = new Mock<IFlightTicketRepository>();
        cancellationService = new CancellationService(mockTicketRepository.Object);
    }

    [TestMethod]
    public void CanCancelTicket_ValidActiveTicket_ReturnsTrue()
    {
        var futureDate = DateTime.Now.AddDays(FutureFlightDaysOffset);
        var flight = FlightFixture.CreateValidTestFlight(departureTime: futureDate);
        var ticket = new FlightTicket { Id = DefaultId, Status = ActiveStatus, Flight = flight };

        var (canCancelResult, cancelReason) = cancellationService.CanCancelTicket(ticket);

        canCancelResult.Should().BeTrue();
        cancelReason.Should().BeEmpty();
    }

    [TestMethod]
    public void CanCancelTicket_AlreadyCancelledTicket_ReturnsFalse()
    {
        var flight = FlightFixture.CreateValidTestFlight();
        var ticket = new FlightTicket { Id = DefaultId, Status = CancelledStatus, Flight = flight };

        var (canCancelResult, cancelReason) = cancellationService.CanCancelTicket(ticket);

        canCancelResult.Should().BeFalse();
        cancelReason.Should().Contain(AlreadyCancelledMessage);
    }

    [TestMethod]
    public void CanCancelTicket_PastFlightDate_ReturnsFalse()
    {
        var pastDate = DateTime.Now.AddDays(PastFlightDaysOffset);
        var flight = FlightFixture.CreateValidTestFlight(departureTime: pastDate);
        var ticket = new FlightTicket { Id = DefaultId, Status = ActiveStatus, Flight = flight };

        var (canCancelResult, cancelReason) = cancellationService.CanCancelTicket(ticket);

        canCancelResult.Should().BeFalse();
        cancelReason.Should().Contain(PastFlightMessage);
    }

    [TestMethod]
    public void CanCancelTicket_NullTicket_ReturnsFalse()
    {
        var (canCancelResult, cancelReason) = cancellationService.CanCancelTicket(null);

        canCancelResult.Should().BeFalse();
        cancelReason.Should().Be("Ticket not found.");
    }
}
