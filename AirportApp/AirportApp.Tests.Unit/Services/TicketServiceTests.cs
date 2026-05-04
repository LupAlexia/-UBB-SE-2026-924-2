using CloudSpritzers1.Src.Dto;
using CloudSpritzers1.Src.Model;
using CloudSpritzers1.Src.Model.Ticket;
using CloudSpritzers1.Src.Repository;
using CloudSpritzers1.Src.Repository.Interfaces;
using CloudSpritzers1.Src.Service;
using CloudSpritzers1.Src.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudSpritzers1Tests.Src.Service
{
    [TestClass]
    public class TicketServiceTests
    {
        private ITicketRepository _ticketRepository;
        private TicketService _ticketService;
        private User _testUser;
        private TicketCategory _testCategory;
        private TicketSubcategory _testSubcategory;

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
        public void CreateTicket_WithValidData_CallsRepository()
        {
            var currentTimeAndDate = DateTime.Now;

            _ticketService.CreateTicket(1, _testUser, TicketStatusEnum.OPEN, _testCategory, _testSubcategory, "Subject", "Description", currentTimeAndDate);
            _ticketRepository.Received(1).CreateNewEntity(Arg.Any<Ticket>());
        }

        [TestMethod]
        public void ValidateTicket_WhenTicketIsNull_ThrowsArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _ticketService.ValidateTicket(null));
        }

        [TestMethod]
        public void ValidateTicket_WhenCreatorIsNull_ThrowsArgumentNullException()
        {
            var invalidTicket = new Ticket(1, null, TicketStatusEnum.OPEN, _testCategory, _testSubcategory, "Subject", "Desc", DateTime.Now);
            Assert.ThrowsExactly<ArgumentNullException>(() => _ticketService.ValidateTicket(invalidTicket));
        }

        [TestMethod]
        public void ValidateTicket_SubcategoryNotMatchingCategory_ThrowsArgumentException()
        {

            var wrongCategory = new TicketCategory(99, "ThisIsWronggggg", TicketUrgencyLevelEnum.LOW);
            var invalidTicket = new Ticket(1, _testUser, TicketStatusEnum.OPEN, wrongCategory, _testSubcategory, "Subject", "Desc", DateTime.Now);

            var exceptionThrown = Assert.ThrowsExactly<ArgumentException>(() => _ticketService.ValidateTicket(invalidTicket));

        }

        [TestMethod]
        public void ValidateTicket_WithEmptySubject_ThrowsException()
        {
            var invalidTicket = new Ticket(1, _testUser, TicketStatusEnum.OPEN, _testCategory, _testSubcategory, "", "Desc", DateTime.Now);
            Assert.ThrowsExactly<ArgumentNullException>(() => _ticketService.ValidateTicket(invalidTicket));
        }

        [TestMethod]
        public void ValidateTicket_WithEmptyDescription_ThrowsException()
        {
            var invalidTicket = new Ticket(1, _testUser, TicketStatusEnum.OPEN, _testCategory, _testSubcategory, "Subject", "", DateTime.Now);
            Assert.ThrowsExactly<ArgumentNullException>(() => _ticketService.ValidateTicket(invalidTicket));
        }

        [TestMethod]
        public void TicketConstructor_WhenCategoryIsNull_ThrowsNullReferenceException()
        {

            Assert.ThrowsExactly<NullReferenceException>(() =>
               new Ticket(1, _testUser, TicketStatusEnum.OPEN, null, _testSubcategory, "Sub", "Desc", DateTime.Now)
           );

        }


        [TestMethod]
        public void ValidateTicket_ForcedNullCategory_ReturnsYourCustomMessage()
        {
            //testul asta a fost facut sa forteze testarea null Category , penru ca , quite frankly , constructorul Ticket nu permite sa fie null , iar ValidateTicket nu are cum sa prinda asta , deci am folosit reflection ca sa setez Category la null si sa vad daca ValidateTicket arunca exceptionul cu mesajul custom pe care l-am pus acolo
            // a.k.a innebunescu la propriu ca sa testez ceva ce nu ar trebui sa fie posibil in primul rand
            var ticket = new Ticket(1, _testUser, TicketStatusEnum.OPEN, _testCategory, _testSubcategory, "Subject", "Description", DateTime.Now);
            var categoryField = typeof(Ticket)
                .GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .FirstOrDefault(field => field.FieldType == typeof(TicketCategory));

            if (categoryField == null)
            {
                Assert.Inconclusive("Category field not found");
            }

            categoryField.SetValue(ticket, null);
            var exceptionThrown = Assert.ThrowsExactly<ArgumentNullException>(() => _ticketService.ValidateTicket(ticket));

            Assert.IsTrue(exceptionThrown.Message.Contains("Null Category"), $"Mesajul actual a fost: {exceptionThrown.Message}");
        }

        [TestMethod]
        public void ValidateTicket_WhenSubcategoryIsNull_ThrowsArgumentNullException()
        {
            var invalidTicket = new Ticket(
                1,
                _testUser,
                TicketStatusEnum.OPEN,
                _testCategory,
                null,
                "Subject",
                "Description",
                DateTime.Now
            );

            var exceptionThrown = Assert.ThrowsExactly<ArgumentNullException>(() => _ticketService.ValidateTicket(invalidTicket));
            Assert.IsTrue(exceptionThrown.Message.Contains("Null Subcategory"));
        }


        [TestMethod]
        public void UpdateStatus_ExistingTicket_UpdatesAndSaves()
        {
            var ticket = new Ticket(1, _testUser, TicketStatusEnum.OPEN, _testCategory, _testSubcategory, "Sub", "Desc", DateTime.Now);
            _ticketRepository.GetById(1).Returns(ticket);
            
            _ticketService.UpdateStatus(1, TicketStatusEnum.RESOLVED);
            
            Assert.AreEqual(TicketStatusEnum.RESOLVED, ticket.CurrentStatus);
            _ticketRepository.Received(1).UpdateById(1, ticket);
        }

        [TestMethod]
        public void UpdateUrgencyLevel_ExistingTicket_UpdatesAndSaves()
        {
            var ticket = new Ticket(1, _testUser, TicketStatusEnum.OPEN, _testCategory, _testSubcategory, "Sub", "Desc", DateTime.Now, TicketUrgencyLevelEnum.LOW);
            _ticketRepository.GetById(1).Returns(ticket);

            _ticketService.UpdateUrgencyLevel(1, TicketUrgencyLevelEnum.HIGH);

            Assert.AreEqual(TicketUrgencyLevelEnum.HIGH, ticket.UrgencyLevel);
            _ticketRepository.Received(1).UpdateById(1, ticket);
        }

        [TestMethod]
        public void DeleteTicketById_WhenCalled_CallsRepositoryDelete()
        {
            int ticketIdToDelete = 42;
            _ticketService.DeleteTicketById(ticketIdToDelete);

            _ticketRepository.Received(1).DeleteById(ticketIdToDelete);
        }

        [TestMethod]
        public void GetTicketById_WhenTicketExists_ReturnsCorrectTicket()
        {
            int targetId = 7;
            var expectedTicket = new Ticket(targetId, _testUser, TicketStatusEnum.OPEN, _testCategory, _testSubcategory, "Subiect Test", "Descriere Test", DateTime.Now);

            _ticketRepository.GetById(targetId).Returns(expectedTicket);
            var resultedTicket = _ticketService.GetTicketById(targetId);

            Assert.IsNotNull(resultedTicket);
            Assert.AreEqual(targetId, resultedTicket.TicketId);
            Assert.AreEqual("Subiect Test", resultedTicket.Subject);
            _ticketRepository.Received(1).GetById(targetId);
        }

        [TestMethod]
        public void GetTicketById_WhenTicketDoesNotExist_ReturnsNull()
        {
            int nonExistentId = 999;
            _ticketRepository.GetById(nonExistentId).Returns((Ticket)null);

            var resultedTicket = _ticketService.GetTicketById(nonExistentId);

            Assert.IsNull(resultedTicket);
            _ticketRepository.Received(1).GetById(nonExistentId);
        }


        [TestMethod]
        public void GetAllTickets_WhenCalled_ReturnsAllTicketsFromRepository()
        {
            var tickets = new List<Ticket>
            {
                new Ticket(1, _testUser, TicketStatusEnum.OPEN, _testCategory, _testSubcategory, "S1", "D1", DateTime.Now),
                new Ticket(2, _testUser, TicketStatusEnum.IN_PROGRESS, _testCategory, _testSubcategory, "S2", "D2", DateTime.Now)
            };
            _ticketRepository.GetAll().Returns(tickets);

            var resultedTickets = _ticketService.GetAllTickets();

            Assert.IsNotNull(resultedTickets);
            Assert.AreEqual(2, resultedTickets.Count());
            _ticketRepository.Received(1).GetAll();
        }

        [TestMethod]
        public void UpdateTicketById_WhenCalled_CallsRepositoryUpdateWithCorrectData()
        {
            int targetTicketId = 5;
            var updatedTicket = new Ticket(targetTicketId, _testUser, TicketStatusEnum.RESOLVED, _testCategory, _testSubcategory, "Updated Subject", "Updated Desc", DateTime.Now);

            _ticketService.UpdateTicketById(targetTicketId, updatedTicket);

            _ticketRepository.Received(1).UpdateById(targetTicketId, updatedTicket);
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