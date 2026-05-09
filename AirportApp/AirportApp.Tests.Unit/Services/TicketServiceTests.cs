using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service;
using AirportApp.Src.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AirportApp.Tests.Unit.Src.Service
{
    [TestClass]
    public class TicketServiceTests
    {
        private ITicketRepository ticketRepository = null!;
        private ComplaintTicketService ticketService = null!;
        private User testUser = null!;
        private ComplaintTicketCategory testCategory = null!;
        private ComplaintTicketSubcategory testSubcategory = null!;

        [TestInitialize]
        public void Setup()
        {
            ticketRepository = Substitute.For<ITicketRepository>();
            ticketService = new ComplaintTicketService(ticketRepository);

            testUser = new User(1, "Dede", "dede_the_racoon@gmail.com");
            testCategory = new ComplaintTicketCategory(1, "IT", ComplaintTicketUrgencyLevelEnum.HIGH);
            testSubcategory = new ComplaintTicketSubcategory(10, "Hardware", 101, testCategory);
        }

        [TestMethod]
        public async Task CreateTicket_WithValidData_CallsRepository()
        {
            var currentTimeAndDate = DateTime.Now;

            await ticketService.CreateTicketAsync(1, testUser, ComplaintTicketStatusEnum.OPEN, testCategory, testSubcategory, "Subject", "Description", currentTimeAndDate);
            await ticketRepository.Received(1).CreateNewEntityAsync(Arg.Any<ComplaintTicket>());
        }

        [TestMethod]
        public void ValidateTicket_WhenTicketIsNull_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ticketService.ValidateTicket(null));
        }

        [TestMethod]
        public void ValidateTicket_WhenCreatorIsNull_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ComplaintTicket(1, null, ComplaintTicketStatusEnum.OPEN, testCategory, testSubcategory, "Subject", "Desc", DateTime.Now));
        }

        [TestMethod]
        public void ValidateTicket_SubcategoryNotMatchingCategory_ThrowsArgumentException()
        {
            var wrongCategory = new ComplaintTicketCategory(99, "ThisIsWronggggg", ComplaintTicketUrgencyLevelEnum.LOW);
            var invalidTicket = new ComplaintTicket(1, testUser, ComplaintTicketStatusEnum.OPEN, wrongCategory, testSubcategory, "Subject", "Desc", DateTime.Now);

            Assert.ThrowsException<ArgumentException>(() => ticketService.ValidateTicket(invalidTicket));
        }

        [TestMethod]
        public void ValidateTicket_WithEmptySubject_ThrowsException()
        {
            var invalidTicket = new ComplaintTicket(1, testUser, ComplaintTicketStatusEnum.OPEN, testCategory, testSubcategory, string.Empty, "Desc", DateTime.Now);
            Assert.ThrowsException<ArgumentNullException>(() => ticketService.ValidateTicket(invalidTicket));
        }

        [TestMethod]
        public void ValidateTicket_WithEmptyDescription_ThrowsException()
        {
            var invalidTicket = new ComplaintTicket(1, testUser, ComplaintTicketStatusEnum.OPEN, testCategory, testSubcategory, "Subject", string.Empty, DateTime.Now);
            Assert.ThrowsException<ArgumentNullException>(() => ticketService.ValidateTicket(invalidTicket));
        }

        [TestMethod]
        public void TicketConstructor_WhenCategoryIsNull_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
               new ComplaintTicket(1, testUser, ComplaintTicketStatusEnum.OPEN, null, testSubcategory, "Sub", "Desc", DateTime.Now));
        }

        [TestMethod]
        public void ValidateTicket_WhenSubcategoryIsNull_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ComplaintTicket(1, testUser, ComplaintTicketStatusEnum.OPEN, testCategory, null, "Subject", "Description", DateTime.Now));
        }

        [TestMethod]
        public async Task UpdateStatus_ExistingTicket_UpdatesAndSaves()
        {
            var ticket = new ComplaintTicket(1, testUser, ComplaintTicketStatusEnum.OPEN, testCategory, testSubcategory, "Sub", "Desc", DateTime.Now);
            ticketRepository.GetByIdAsync(1).Returns(Task.FromResult(ticket));

            await ticketService.UpdateStatusAsync(1, ComplaintTicketStatusEnum.RESOLVED);

            Assert.AreEqual(ComplaintTicketStatusEnum.RESOLVED, ticket.CurrentStatus);
            await ticketRepository.Received(1).UpdateByIdAsync(1, ticket);
        }

        [TestMethod]
        public async Task UpdateUrgencyLevel_ExistingTicket_UpdatesAndSaves()
        {
            var ticket = new ComplaintTicket(1, testUser, ComplaintTicketStatusEnum.OPEN, testCategory, testSubcategory, "Sub", "Desc", DateTime.Now, ComplaintTicketUrgencyLevelEnum.LOW);
            ticketRepository.GetByIdAsync(1).Returns(Task.FromResult(ticket));

            await ticketService.UpdateUrgencyLevelAsync(1, ComplaintTicketUrgencyLevelEnum.HIGH);

            Assert.AreEqual(ComplaintTicketUrgencyLevelEnum.HIGH, ticket.UrgencyLevel);
            await ticketRepository.Received(1).UpdateByIdAsync(1, ticket);
        }

        [TestMethod]
        public async Task DeleteTicketById_WhenCalled_CallsRepositoryDelete()
        {
            int ticketIdToDelete = 42;
            await ticketService.DeleteTicketByIdAsync(ticketIdToDelete);

            await ticketRepository.Received(1).DeleteByIdAsync(ticketIdToDelete);
        }

        [TestMethod]
        public async Task GetTicketById_WhenTicketExists_ReturnsCorrectTicket()
        {
            int targetId = 7;
            var expectedTicket = new ComplaintTicket(targetId, testUser, ComplaintTicketStatusEnum.OPEN, testCategory, testSubcategory, "Subiect Test", "Descriere Test", DateTime.Now);

            ticketRepository.GetByIdAsync(targetId).Returns(Task.FromResult(expectedTicket));
            var resultedTicket = await ticketService.GetTicketByIdAsync(targetId);

            Assert.IsNotNull(resultedTicket);
            Assert.AreEqual(targetId, resultedTicket.Id);
            Assert.AreEqual("Subiect Test", resultedTicket.Subject);
            await ticketRepository.Received(1).GetByIdAsync(targetId);
        }

        [TestMethod]
        public async Task GetTicketById_WhenTicketDoesNotExist_ReturnsNull()
        {
            int nonExistentId = 999;
            ticketRepository.GetByIdAsync(nonExistentId).Returns(Task.FromResult((ComplaintTicket)null));

            var resultedTicket = await ticketService.GetTicketByIdAsync(nonExistentId);

            Assert.IsNull(resultedTicket);
            await ticketRepository.Received(1).GetByIdAsync(nonExistentId);
        }

        [TestMethod]
        public async Task GetAllTickets_WhenCalled_ReturnsAllTicketsFromRepository()
        {
            var tickets = new List<ComplaintTicket>
            {
                new ComplaintTicket(1, testUser, ComplaintTicketStatusEnum.OPEN, testCategory, testSubcategory, "S1", "D1", DateTime.Now),
                new ComplaintTicket(2, testUser, ComplaintTicketStatusEnum.IN_PROGRESS, testCategory, testSubcategory, "S2", "D2", DateTime.Now)
            };
            ticketRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<ComplaintTicket>)tickets));

            var resultedTickets = await ticketService.GetAllTicketsAsync();

            Assert.IsNotNull(resultedTickets);
            Assert.AreEqual(2, resultedTickets.Count());
            await ticketRepository.Received(1).GetAllAsync();
        }

        [TestMethod]
        public async Task UpdateTicketById_WhenCalled_CallsRepositoryUpdateWithCorrectData()
        {
            int targetId = 5;
            var updatedTicket = new ComplaintTicket(targetId, testUser, ComplaintTicketStatusEnum.RESOLVED, testCategory, testSubcategory, "Updated Subject", "Updated Desc", DateTime.Now);

            await ticketService.UpdateTicketByIdAsync(targetId, updatedTicket);

            await ticketRepository.Received(1).UpdateByIdAsync(targetId, updatedTicket);
        }

        [TestMethod]
        public void FilterTicketsByStatus_WithInProgressFilter_ReturnsOnlyInProgressTickets()
        {
            var ticketsDataTransferObject = new List<TicketDTO>
            {
                new TicketDTO(1, 1, "myoneemail", ComplaintTicketUrgencyLevelEnum.HIGH, ComplaintTicketStatusEnum.IN_PROGRESS, 1, "ISSbestDomain", 10, "Some subdomain", "Subj", "D1", DateTime.Now),
                new TicketDTO(2, 1, "myoneemail", ComplaintTicketUrgencyLevelEnum.LOW, ComplaintTicketStatusEnum.OPEN, 1, "ISSbestDomain", 10, "Some subdomain", "Subj", "D2", DateTime.Now)
            };

            var resultedTickets = ticketService.FilterTicketsByStatus(ticketsDataTransferObject, TicketFilterStatusEnum.IN_PROGRESS).ToList();

            Assert.AreEqual(1, resultedTickets.Count);
            Assert.AreEqual(ComplaintTicketStatusEnum.IN_PROGRESS, resultedTickets.First().currentStatus);
        }

        [TestMethod]
        public void FilterTicketsByStatus_WithResolvedFilter_ReturnsOnlyResolvedTickets()
        {
            var ticketsDataTransferObject = new List<TicketDTO>
            {
                new TicketDTO(1, 1, "e1", ComplaintTicketUrgencyLevelEnum.HIGH, ComplaintTicketStatusEnum.RESOLVED, 1, "C1", 10, "S1", "Sub1", "D1", DateTime.Now),
                new TicketDTO(2, 1, "e1", ComplaintTicketUrgencyLevelEnum.LOW, ComplaintTicketStatusEnum.OPEN, 1, "C1", 10, "S1", "Sub2", "D2", DateTime.Now)
            };

            var resultedTickets = ticketService.FilterTicketsByStatus(ticketsDataTransferObject, TicketFilterStatusEnum.RESOLVED).ToList();

            Assert.AreEqual(1, resultedTickets.Count);
            Assert.AreEqual(ComplaintTicketStatusEnum.RESOLVED, resultedTickets.First().currentStatus);
        }

        [TestMethod]
        public void FilterTicketsByStatus_WithOpenFilter_ReturnsOnlyOpenTickets()
        {
            var ticketsDataTransferObject = new List<TicketDTO>
            {
                new TicketDTO(1, 1, "e1", ComplaintTicketUrgencyLevelEnum.HIGH, ComplaintTicketStatusEnum.OPEN, 1, "C1", 10, "S1", "Sub1", "D1", DateTime.Now),
                new TicketDTO(2, 1, "e1", ComplaintTicketUrgencyLevelEnum.LOW, ComplaintTicketStatusEnum.RESOLVED, 1, "C1", 10, "S1", "Sub2", "D2", DateTime.Now)
            };
            var result = ticketService.FilterTicketsByStatus(ticketsDataTransferObject, TicketFilterStatusEnum.OPEN).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(ComplaintTicketStatusEnum.OPEN, result.First().currentStatus);
        }

        [TestMethod]
        public void FilterTicketsByStatus_WithUndefinedFilter_ReturnsAllTickets()
        {
            var ticketsDataTransferObject = new List<TicketDTO>
            {
                new TicketDTO(1, 1, "e1@test.com", ComplaintTicketUrgencyLevelEnum.HIGH, ComplaintTicketStatusEnum.OPEN, 1, "IT", 10, "Hardware", "Sub1", "Desc1", DateTime.Now),
                new TicketDTO(2, 1, "e1@test.com", ComplaintTicketUrgencyLevelEnum.LOW, ComplaintTicketStatusEnum.RESOLVED, 1, "IT", 10, "Hardware", "Sub2", "Desc2", DateTime.Now)
            };

            var unknownFilter = (TicketFilterStatusEnum)999;

            var resultedTickets = ticketService.FilterTicketsByStatus(ticketsDataTransferObject, unknownFilter).ToList();

            Assert.AreEqual(2, resultedTickets.Count);
            Assert.AreEqual(ticketsDataTransferObject[0].subject, resultedTickets[0].subject);
            Assert.AreEqual(ticketsDataTransferObject[1].subject, resultedTickets[1].subject);
        }
    }
}
