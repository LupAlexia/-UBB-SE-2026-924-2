using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service;
using AirportApp.Src.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirportApp.Tests.Unit.Src.Service
{
    [TestClass]
    public class TicketServiceTests
    {
        private ITicketRepository _ticketRepository = null!;
        private TicketService _ticketService = null!;
        private User _testUser = null!;
        private TicketCategory _testCategory = null!;
        private TicketSubcategory _testSubcategory = null!;

        [TestInitialize]
        public void Setup()
        {
            _ticketRepository = Substitute.For<ITicketRepository>();
            _ticketService = new TicketService(_ticketRepository);

            _testUser = new User(1, "Dede", "dede_the_racoon@gmail.com");
            _testCategory = new TicketCategory(1, "IT", TicketUrgencyLevelEnum.HIGH);
            _testSubcategory = new TicketSubcategory(10, "Hardware", 101, _testCategory);
        }

        [TestMethod]
        public async Task CreateTicket_WithValidData_CallsRepository()
        {
            var currentTimeAndDate = DateTime.Now;

            await _ticketService.CreateTicketAsync(1, _testUser, TicketStatusEnum.OPEN, _testCategory, _testSubcategory, "Subject", "Description", currentTimeAndDate);
            await _ticketRepository.Received(1).CreateNewEntityAsync(Arg.Any<Ticket>());
        }

        [TestMethod]
        public void ValidateTicket_WhenTicketIsNull_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _ticketService.ValidateTicket(null));
        }

        [TestMethod]
        public void ValidateTicket_WhenCreatorIsNull_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => 
                new Ticket(1, null, TicketStatusEnum.OPEN, _testCategory, _testSubcategory, "Subject", "Desc", DateTime.Now));
        }

        [TestMethod]
        public void ValidateTicket_SubcategoryNotMatchingCategory_ThrowsArgumentException()
        {
            var wrongCategory = new TicketCategory(99, "ThisIsWronggggg", TicketUrgencyLevelEnum.LOW);
            var invalidTicket = new Ticket(1, _testUser, TicketStatusEnum.OPEN, wrongCategory, _testSubcategory, "Subject", "Desc", DateTime.Now);

            Assert.ThrowsException<ArgumentException>(() => _ticketService.ValidateTicket(invalidTicket));
        }

        [TestMethod]
        public void ValidateTicket_WithEmptySubject_ThrowsException()
        {
            var invalidTicket = new Ticket(1, _testUser, TicketStatusEnum.OPEN, _testCategory, _testSubcategory, "", "Desc", DateTime.Now);
            Assert.ThrowsException<ArgumentNullException>(() => _ticketService.ValidateTicket(invalidTicket));
        }

        [TestMethod]
        public void ValidateTicket_WithEmptyDescription_ThrowsException()
        {
            var invalidTicket = new Ticket(1, _testUser, TicketStatusEnum.OPEN, _testCategory, _testSubcategory, "Subject", "", DateTime.Now);
            Assert.ThrowsException<ArgumentNullException>(() => _ticketService.ValidateTicket(invalidTicket));
        }

        [TestMethod]
        public void TicketConstructor_WhenCategoryIsNull_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
               new Ticket(1, _testUser, TicketStatusEnum.OPEN, null, _testSubcategory, "Sub", "Desc", DateTime.Now)
           );
        }

        [TestMethod]
        public void ValidateTicket_WhenSubcategoryIsNull_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => 
                new Ticket(1, _testUser, TicketStatusEnum.OPEN, _testCategory, null, "Subject", "Description", DateTime.Now));
        }

        [TestMethod]
        public async Task UpdateStatus_ExistingTicket_UpdatesAndSaves()
        {
            var ticket = new Ticket(1, _testUser, TicketStatusEnum.OPEN, _testCategory, _testSubcategory, "Sub", "Desc", DateTime.Now);
            _ticketRepository.GetByIdAsync(1).Returns(Task.FromResult(ticket));
            
            await _ticketService.UpdateStatusAsync(1, TicketStatusEnum.RESOLVED);
            
            Assert.AreEqual(TicketStatusEnum.RESOLVED, ticket.CurrentStatus);
            await _ticketRepository.Received(1).UpdateByIdAsync(1, ticket);
        }

        [TestMethod]
        public async Task UpdateUrgencyLevel_ExistingTicket_UpdatesAndSaves()
        {
            var ticket = new Ticket(1, _testUser, TicketStatusEnum.OPEN, _testCategory, _testSubcategory, "Sub", "Desc", DateTime.Now, TicketUrgencyLevelEnum.LOW);
            _ticketRepository.GetByIdAsync(1).Returns(Task.FromResult(ticket));

            await _ticketService.UpdateUrgencyLevelAsync(1, TicketUrgencyLevelEnum.HIGH);

            Assert.AreEqual(TicketUrgencyLevelEnum.HIGH, ticket.UrgencyLevel);
            await _ticketRepository.Received(1).UpdateByIdAsync(1, ticket);
        }

        [TestMethod]
        public async Task DeleteTicketById_WhenCalled_CallsRepositoryDelete()
        {
            int ticketIdToDelete = 42;
            await _ticketService.DeleteTicketByIdAsync(ticketIdToDelete);

            await _ticketRepository.Received(1).DeleteByIdAsync(ticketIdToDelete);
        }

        [TestMethod]
        public async Task GetTicketById_WhenTicketExists_ReturnsCorrectTicket()
        {
            int targetId = 7;
            var expectedTicket = new Ticket(targetId, _testUser, TicketStatusEnum.OPEN, _testCategory, _testSubcategory, "Subiect Test", "Descriere Test", DateTime.Now);

            _ticketRepository.GetByIdAsync(targetId).Returns(Task.FromResult(expectedTicket));
            var resultedTicket = await _ticketService.GetTicketByIdAsync(targetId);

            Assert.IsNotNull(resultedTicket);
            Assert.AreEqual(targetId, resultedTicket.Id);
            Assert.AreEqual("Subiect Test", resultedTicket.Subject);
            await _ticketRepository.Received(1).GetByIdAsync(targetId);
        }

        [TestMethod]
        public async Task GetTicketById_WhenTicketDoesNotExist_ReturnsNull()
        {
            int nonExistentId = 999;
            _ticketRepository.GetByIdAsync(nonExistentId).Returns(Task.FromResult((Ticket)null));

            var resultedTicket = await _ticketService.GetTicketByIdAsync(nonExistentId);

            Assert.IsNull(resultedTicket);
            await _ticketRepository.Received(1).GetByIdAsync(nonExistentId);
        }

        [TestMethod]
        public async Task GetAllTickets_WhenCalled_ReturnsAllTicketsFromRepository()
        {
            var tickets = new List<Ticket>
            {
                new Ticket(1, _testUser, TicketStatusEnum.OPEN, _testCategory, _testSubcategory, "S1", "D1", DateTime.Now),
                new Ticket(2, _testUser, TicketStatusEnum.IN_PROGRESS, _testCategory, _testSubcategory, "S2", "D2", DateTime.Now)
            };
            _ticketRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<Ticket>)tickets));

            var resultedTickets = await _ticketService.GetAllTicketsAsync();

            Assert.IsNotNull(resultedTickets);
            Assert.AreEqual(2, resultedTickets.Count());
            await _ticketRepository.Received(1).GetAllAsync();
        }

        [TestMethod]
        public async Task UpdateTicketById_WhenCalled_CallsRepositoryUpdateWithCorrectData()
        {
            int targetId = 5;
            var updatedTicket = new Ticket(targetId, _testUser, TicketStatusEnum.RESOLVED, _testCategory, _testSubcategory, "Updated Subject", "Updated Desc", DateTime.Now);

            await _ticketService.UpdateTicketByIdAsync(targetId, updatedTicket);

            await _ticketRepository.Received(1).UpdateByIdAsync(targetId, updatedTicket);
        }

        [TestMethod]
        public void FilterTicketsByStatus_WithInProgressFilter_ReturnsOnlyInProgressTickets()
        {
            var ticketsDataTransferObject = new List<TicketDTO>
            {
                new TicketDTO(1, 1, "myoneemail", TicketUrgencyLevelEnum.HIGH, TicketStatusEnum.IN_PROGRESS, 1, "ISSbestDomain", 10, "Some subdomain", "Subj", "D1", DateTime.Now),
                new TicketDTO(2, 1, "myoneemail", TicketUrgencyLevelEnum.LOW, TicketStatusEnum.OPEN, 1, "ISSbestDomain", 10, "Some subdomain", "Subj", "D2", DateTime.Now)
            };
            
            var resultedTickets = _ticketService.FilterTicketsByStatus(ticketsDataTransferObject, TicketFilterStatusEnum.IN_PROGRESS).ToList();
            
            Assert.AreEqual(1, resultedTickets.Count);
            Assert.AreEqual(TicketStatusEnum.IN_PROGRESS, resultedTickets.First().currentStatus);
        }

        [TestMethod]
        public void FilterTicketsByStatus_WithResolvedFilter_ReturnsOnlyResolvedTickets()
        {
            var ticketsDataTransferObject = new List<TicketDTO>
            {
                new TicketDTO(1, 1, "e1", TicketUrgencyLevelEnum.HIGH, TicketStatusEnum.RESOLVED, 1, "C1", 10, "S1", "Sub1", "D1", DateTime.Now),
                new TicketDTO(2, 1, "e1", TicketUrgencyLevelEnum.LOW, TicketStatusEnum.OPEN, 1, "C1", 10, "S1", "Sub2", "D2", DateTime.Now)
            };
            
            var resultedTickets = _ticketService.FilterTicketsByStatus(ticketsDataTransferObject, TicketFilterStatusEnum.RESOLVED).ToList();
            
            Assert.AreEqual(1, resultedTickets.Count);
            Assert.AreEqual(TicketStatusEnum.RESOLVED, resultedTickets.First().currentStatus);
        }

        [TestMethod]
        public void FilterTicketsByStatus_WithOpenFilter_ReturnsOnlyOpenTickets()
        {
            var ticketsDataTransferObject = new List<TicketDTO>
            {
                new TicketDTO(1, 1, "e1", TicketUrgencyLevelEnum.HIGH, TicketStatusEnum.OPEN, 1, "C1", 10, "S1", "Sub1", "D1", DateTime.Now),
                new TicketDTO(2, 1, "e1", TicketUrgencyLevelEnum.LOW, TicketStatusEnum.RESOLVED, 1, "C1", 10, "S1", "Sub2", "D2", DateTime.Now)
            };
            var result = _ticketService.FilterTicketsByStatus(ticketsDataTransferObject, TicketFilterStatusEnum.OPEN).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(TicketStatusEnum.OPEN, result.First().currentStatus);
        }

        [TestMethod]
        public void FilterTicketsByStatus_WithUndefinedFilter_ReturnsAllTickets()
        {
            var ticketsDataTransferObject = new List<TicketDTO>
            {
                new TicketDTO(1, 1, "e1@test.com", TicketUrgencyLevelEnum.HIGH, TicketStatusEnum.OPEN, 1, "IT", 10, "Hardware", "Sub1", "Desc1", DateTime.Now),
                new TicketDTO(2, 1, "e1@test.com", TicketUrgencyLevelEnum.LOW, TicketStatusEnum.RESOLVED, 1, "IT", 10, "Hardware", "Sub2", "Desc2", DateTime.Now)
            };

            var unknownFilter = (TicketFilterStatusEnum)999;
            
            var resultedTickets = _ticketService.FilterTicketsByStatus(ticketsDataTransferObject, unknownFilter).ToList();

            Assert.AreEqual(2, resultedTickets.Count);
            Assert.AreEqual(ticketsDataTransferObject[0].subject, resultedTickets[0].subject);
            Assert.AreEqual(ticketsDataTransferObject[1].subject, resultedTickets[1].subject);
        }
    }
}
