using System;
using FluentAssertions;
using Moq;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service;
using AirportApp.Tests.Unit.Fixtures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AirportApp.Tests.Unit.Services
{
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
        private const string TicketNotFoundMessage = "Ticket not found.";

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

            var (canCancel, reason) = cancellationService.CanCancelTicket(ticket);

            canCancel.Should().BeTrue();
            reason.Should().BeEmpty();
        }

        [TestMethod]
        public void CanCancelTicket_AlreadyCancelledTicket_ReturnsFalse()
        {
            var flight = FlightFixture.CreateValidTestFlight();
            var ticket = new FlightTicket { Id = DefaultId, Status = CancelledStatus, Flight = flight };

            var (canCancel, reason) = cancellationService.CanCancelTicket(ticket);

            canCancel.Should().BeFalse();
            reason.Should().Contain(AlreadyCancelledMessage);
        }

        [TestMethod]
        public void CanCancelTicket_PastFlightDate_ReturnsFalse()
        {
            var pastDate = DateTime.Now.AddDays(PastFlightDaysOffset);
            var flight = FlightFixture.CreateValidTestFlight(departureTime: pastDate);
            var ticket = new FlightTicket { Id = DefaultId, Status = ActiveStatus, Flight = flight };

            var (canCancel, reason) = cancellationService.CanCancelTicket(ticket);

            canCancel.Should().BeFalse();
            reason.Should().Contain(PastFlightMessage);
        }

        [TestMethod]
        public void CanCancelTicket_NullTicket_ReturnsFalse()
        {
            var (canCancel, reason) = cancellationService.CanCancelTicket(null);

            canCancel.Should().BeFalse();
            reason.Should().Be(TicketNotFoundMessage);
        }

        [TestMethod]
        public void CanCancelTicket_NullFlight_ReturnsTrue()
        {
            // No flight means no date to check — should be cancellable
            var ticket = new FlightTicket { Id = DefaultId, Status = ActiveStatus, Flight = null };

            var (canCancel, reason) = cancellationService.CanCancelTicket(ticket);

            canCancel.Should().BeTrue();
            reason.Should().BeEmpty();
        }
    }
}