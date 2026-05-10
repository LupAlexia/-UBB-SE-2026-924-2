using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AirportApp.Tests.Unit.Services
{
    [TestClass]
    public class DashboardServiceTests
    {
        private const int TargetUserId = 1;
        private const string PastFilter = "Past";
        private const string UpcomingFilter = "Upcoming";
        private const int PastDaysOffset = -5;
        private const int PastDaysOffsetFurther = -7;
        private const int UpcomingDaysOffset = 5;
        private const int UpcomingDaysOffsetFurther = 7;
        private const int ImmediateFutureSeconds = 5;

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

            mockTicketRepository.Setup(r => r.GetTicketsByUserIdAsync(TargetUserId))
                .ReturnsAsync(new List<FlightTicket> { ticketWithFlight, ticketWithoutFlight });

            var results = dashboardService.GetUserTicketsAsync(TargetUserId, UpcomingFilter).Result.ToList();

            results.Should().ContainSingle();
            results.First().Should().Be(ticketWithFlight);
        }

        [TestMethod]
        public void GetUserTickets_PastFlights_FiltersAndSortsDescending()
        {
            var olderFlight = new Flight { Date = DateTime.Now.AddDays(PastDaysOffsetFurther) };
            var recentPastFlight = new Flight { Date = DateTime.Now.AddDays(PastDaysOffset) };
            var futureFlight = new Flight { Date = DateTime.Now.AddDays(UpcomingDaysOffset) };

            mockTicketRepository.Setup(r => r.GetTicketsByUserIdAsync(TargetUserId))
                .ReturnsAsync(new List<FlightTicket>
                {
                    new FlightTicket { Flight = olderFlight },
                    new FlightTicket { Flight = futureFlight },
                    new FlightTicket { Flight = recentPastFlight }
                });

            var results = dashboardService.GetUserTicketsAsync(TargetUserId, PastFilter).Result.ToList();

            results.Should().HaveCount(2);
            results.First().Flight!.Date.Should().BeAfter(results.Last().Flight!.Date);
        }

        [TestMethod]
        public void GetUserTickets_UpcomingFlights_FiltersAndSortsAscending()
        {
            var pastFlight = new Flight { Date = DateTime.Now.AddDays(PastDaysOffset) };
            var nearFutureFlight = new Flight { Date = DateTime.Now.AddDays(UpcomingDaysOffset) };
            var farFutureFlight = new Flight { Date = DateTime.Now.AddDays(UpcomingDaysOffsetFurther) };

            mockTicketRepository.Setup(r => r.GetTicketsByUserIdAsync(TargetUserId))
                .ReturnsAsync(new List<FlightTicket>
                {
                    new FlightTicket { Flight = farFutureFlight },
                    new FlightTicket { Flight = pastFlight },
                    new FlightTicket { Flight = nearFutureFlight }
                });

            var results = dashboardService.GetUserTicketsAsync(TargetUserId, UpcomingFilter).Result.ToList();

            results.Should().HaveCount(2);
            results.First().Flight!.Date.Should().BeBefore(results.Last().Flight!.Date);
        }

        [TestMethod]
        public void GetUserTickets_FilterCaseInsensitive_WorksForLowercasePast()
        {
            var pastFlight = new Flight { Date = DateTime.Now.AddDays(PastDaysOffset) };
            mockTicketRepository.Setup(r => r.GetTicketsByUserIdAsync(TargetUserId))
                .ReturnsAsync(new List<FlightTicket> { new FlightTicket { Flight = pastFlight } });

            var results = dashboardService.GetUserTicketsAsync(TargetUserId, "past").Result.ToList();

            results.Should().ContainSingle();
        }

        [TestMethod]
        public void GetUserTickets_EmptyTicketList_ReturnsEmpty()
        {
            mockTicketRepository.Setup(r => r.GetTicketsByUserIdAsync(TargetUserId))
                .ReturnsAsync(new List<FlightTicket>());

            var results = dashboardService.GetUserTicketsAsync(TargetUserId, UpcomingFilter).Result.ToList();

            results.Should().BeEmpty();
        }

        [TestMethod]
        public void GetUserTickets_UnknownFilter_ReturnsUpcomingFlights()
        {
            var futureFlight = new Flight { Date = DateTime.Now.AddDays(UpcomingDaysOffset) };
            var pastFlight = new Flight { Date = DateTime.Now.AddDays(PastDaysOffset) };

            mockTicketRepository.Setup(r => r.GetTicketsByUserIdAsync(TargetUserId))
                .ReturnsAsync(new List<FlightTicket>
                {
                    new FlightTicket { Flight = futureFlight },
                    new FlightTicket { Flight = pastFlight }
                });

            var results = dashboardService.GetUserTicketsAsync(TargetUserId, "SomethingElse").Result.ToList();

            results.Should().ContainSingle();
            results.First().Flight!.Date.Should().BeAfter(DateTime.Now);
        }

        [TestMethod]
        public void GetUserTickets_FlightInImmediateFuture_CountsAsUpcoming()
        {
            var soonFlight = new Flight { Date = DateTime.Now.AddSeconds(ImmediateFutureSeconds) };
            mockTicketRepository.Setup(r => r.GetTicketsByUserIdAsync(TargetUserId))
                .ReturnsAsync(new List<FlightTicket> { new FlightTicket { Flight = soonFlight } });

            var results = dashboardService.GetUserTicketsAsync(TargetUserId, UpcomingFilter).Result.ToList();

            results.Should().ContainSingle();
        }
    }
}