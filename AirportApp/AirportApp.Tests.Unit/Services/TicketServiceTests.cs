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
        private const int TicketId1 = 1;
        private const int TicketId2 = 2;
        private const int TicketId3 = 7;
        private const int UserId = 1;
        private const int CategoryId = 1;
        private const int WrongCategoryId = 99;
        private const int SubcategoryId = 10;
        private const int SubcategoryTicketCount = 101;
        private const string UserName = "Dede";
        private const string UserEmail = "dede_the_racoon@gmail.com";
        private const string CategoryName = "IT";
        private const string WrongCategoryName = "ThisIsWronggggg";
        private const string SubcategoryName = "Hardware";
        private const string ValidSubject = "Subject";
        private const string ValidDescription = "Description";
        private const string EmptySubject = "";
        private const string EmptyDescription = "";
        private const string TicketEmail1 = "myoneemail";
        private const string TicketDomain = "ISSbestDomain";
        private const string TicketSubdomain = "Some subdomain";
        private const string TicketSubject1 = "Subj";
        private const string TicketSubject2 = "Sub2";
        private const string TicketDesc1 = "D1";
        private const string TicketDesc2 = "D2";
        private const string UpdatedSubject = "Subiect Test";
        private const string UpdatedDescription = "Descriere Test";

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

            testUser = new User(UserId, UserName, UserEmail);
            testCategory = new ComplaintTicketCategory(CategoryId, CategoryName, ComplaintTicketUrgencyLevelEnum.HIGH);
            testSubcategory = new ComplaintTicketSubcategory(SubcategoryId, SubcategoryName, SubcategoryTicketCount, testCategory);
        }

        [TestMethod]
        public async Task CreateTicket_WithValidData_CallsRepository()
        {
            await ticketService.CreateTicketAsync(TicketId1, testUser, ComplaintTicketStatusEnum.OPEN, testCategory, testSubcategory, ValidSubject, ValidDescription, DateTime.Now);
            await ticketRepository.Received(1).CreateNewEntityAsync(Arg.Any<ComplaintTicket>());
        }

        [TestMethod]
        public async Task CreateTicket_WithInvalidData_DoesNotCallRepository()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => ticketService.CreateTicketAsync(TicketId1, testUser, ComplaintTicketStatusEnum.OPEN, testCategory, testSubcategory, EmptySubject, ValidDescription, DateTime.Now));

            await ticketRepository.DidNotReceive().CreateNewEntityAsync(Arg.Any<ComplaintTicket>());
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
                new ComplaintTicket(TicketId1, null, ComplaintTicketStatusEnum.OPEN, testCategory, testSubcategory, ValidSubject, ValidDescription, DateTime.Now));
        }

        [TestMethod]
        public void ValidateTicket_SubcategoryNotMatchingCategory_ThrowsArgumentException()
        {
            var wrongCategory = new ComplaintTicketCategory(WrongCategoryId, WrongCategoryName, ComplaintTicketUrgencyLevelEnum.LOW);
            var invalidTicket = new ComplaintTicket(TicketId1, testUser, ComplaintTicketStatusEnum.OPEN, wrongCategory, testSubcategory, ValidSubject, ValidDescription, DateTime.Now);

            Assert.ThrowsException<ArgumentException>(() => ticketService.ValidateTicket(invalidTicket));
        }

        [TestMethod]
        public void ValidateTicket_WithEmptySubject_ThrowsException()
        {
            var invalidTicket = new ComplaintTicket(TicketId1, testUser, ComplaintTicketStatusEnum.OPEN, testCategory, testSubcategory, EmptySubject, ValidDescription, DateTime.Now);
            Assert.ThrowsException<ArgumentNullException>(() => ticketService.ValidateTicket(invalidTicket));
        }

        [TestMethod]
        public void ValidateTicket_WithEmptyDescription_ThrowsException()
        {
            var invalidTicket = new ComplaintTicket(TicketId1, testUser, ComplaintTicketStatusEnum.OPEN, testCategory, testSubcategory, ValidSubject, EmptyDescription, DateTime.Now);
            Assert.ThrowsException<ArgumentNullException>(() => ticketService.ValidateTicket(invalidTicket));
        }

        [TestMethod]
        public void TicketConstructor_WhenCategoryIsNull_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
               new ComplaintTicket(TicketId1, testUser, ComplaintTicketStatusEnum.OPEN, null, testSubcategory, ValidSubject, ValidDescription, DateTime.Now));
        }

        [TestMethod]
        public void ValidateTicket_WhenSubcategoryIsNull_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ComplaintTicket(TicketId1, testUser, ComplaintTicketStatusEnum.OPEN, testCategory, null, ValidSubject, ValidDescription, DateTime.Now));
        }

        [TestMethod]
        public async Task UpdateStatus_ExistingTicket_UpdatesAndSaves()
        {
            var ticket = new ComplaintTicket(TicketId1, testUser, ComplaintTicketStatusEnum.OPEN, testCategory, testSubcategory, ValidSubject, ValidDescription, DateTime.Now);
            ticketRepository.GetByIdAsync(TicketId1).Returns(Task.FromResult(ticket));

            await ticketService.UpdateStatusAsync(TicketId1, ComplaintTicketStatusEnum.RESOLVED);

            await ticketRepository.Received(1).GetByIdAsync(TicketId1);
            await ticketRepository.Received(1).UpdateByIdAsync(TicketId1, Arg.Is<ComplaintTicket>(t => t.CurrentStatus == ComplaintTicketStatusEnum.RESOLVED));
        }

        [TestMethod]
        public async Task UpdateUrgencyLevel_ExistingTicket_UpdatesAndSaves()
        {
            var ticket = new ComplaintTicket(TicketId1, testUser, ComplaintTicketStatusEnum.OPEN, testCategory, testSubcategory, ValidSubject, ValidDescription, DateTime.Now, ComplaintTicketUrgencyLevelEnum.LOW);
            ticketRepository.GetByIdAsync(TicketId1).Returns(Task.FromResult(ticket));

            await ticketService.UpdateUrgencyLevelAsync(TicketId1, ComplaintTicketUrgencyLevelEnum.HIGH);

            await ticketRepository.Received(1).GetByIdAsync(TicketId1);
            await ticketRepository.Received(1).UpdateByIdAsync(TicketId1, Arg.Is<ComplaintTicket>(t => t.UrgencyLevel == ComplaintTicketUrgencyLevelEnum.HIGH));
        }

        [TestMethod]
        public void FilterTicketsByStatus_WithInProgressFilter_ReturnsOnlyInProgressTickets()
        {
            var tickets = new List<TicketDTO>
            {
                new TicketDTO(TicketId1, UserId, TicketEmail1, ComplaintTicketUrgencyLevelEnum.HIGH, ComplaintTicketStatusEnum.IN_PROGRESS, CategoryId, TicketDomain, SubcategoryId, TicketSubdomain, TicketSubject1, TicketDesc1, DateTime.Now),
                new TicketDTO(TicketId2, UserId, TicketEmail1, ComplaintTicketUrgencyLevelEnum.LOW, ComplaintTicketStatusEnum.OPEN, CategoryId, TicketDomain, SubcategoryId, TicketSubdomain, TicketSubject2, TicketDesc2, DateTime.Now)
            };

            var result = ticketService.FilterTicketsByStatus(tickets, TicketFilterStatusEnum.IN_PROGRESS).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(ComplaintTicketStatusEnum.IN_PROGRESS, result.First().currentStatus);
        }

        [TestMethod]
        public void FilterTicketsByStatus_WithResolvedFilter_ReturnsOnlyResolvedTickets()
        {
            var tickets = new List<TicketDTO>
            {
                new TicketDTO(TicketId1, UserId, TicketEmail1, ComplaintTicketUrgencyLevelEnum.HIGH, ComplaintTicketStatusEnum.RESOLVED, CategoryId, TicketDomain, SubcategoryId, TicketSubdomain, TicketSubject1, TicketDesc1, DateTime.Now),
                new TicketDTO(TicketId2, UserId, TicketEmail1, ComplaintTicketUrgencyLevelEnum.LOW, ComplaintTicketStatusEnum.OPEN, CategoryId, TicketDomain, SubcategoryId, TicketSubdomain, TicketSubject2, TicketDesc2, DateTime.Now)
            };

            var result = ticketService.FilterTicketsByStatus(tickets, TicketFilterStatusEnum.RESOLVED).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(ComplaintTicketStatusEnum.RESOLVED, result.First().currentStatus);
        }

        [TestMethod]
        public void FilterTicketsByStatus_WithOpenFilter_ReturnsOnlyOpenTickets()
        {
            var tickets = new List<TicketDTO>
            {
                new TicketDTO(TicketId1, UserId, TicketEmail1, ComplaintTicketUrgencyLevelEnum.HIGH, ComplaintTicketStatusEnum.OPEN, CategoryId, TicketDomain, SubcategoryId, TicketSubdomain, TicketSubject1, TicketDesc1, DateTime.Now),
                new TicketDTO(TicketId2, UserId, TicketEmail1, ComplaintTicketUrgencyLevelEnum.LOW, ComplaintTicketStatusEnum.RESOLVED, CategoryId, TicketDomain, SubcategoryId, TicketSubdomain, TicketSubject2, TicketDesc2, DateTime.Now)
            };

            var result = ticketService.FilterTicketsByStatus(tickets, TicketFilterStatusEnum.OPEN).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(ComplaintTicketStatusEnum.OPEN, result.First().currentStatus);
        }

        [TestMethod]
        public void FilterTicketsByStatus_WithUndefinedFilter_ReturnsAllTickets()
        {
            var tickets = new List<TicketDTO>
            {
                new TicketDTO(TicketId1, UserId, TicketEmail1, ComplaintTicketUrgencyLevelEnum.HIGH, ComplaintTicketStatusEnum.OPEN, CategoryId, TicketDomain, SubcategoryId, TicketSubdomain, TicketSubject1, TicketDesc1, DateTime.Now),
                new TicketDTO(TicketId2, UserId, TicketEmail1, ComplaintTicketUrgencyLevelEnum.LOW, ComplaintTicketStatusEnum.RESOLVED, CategoryId, TicketDomain, SubcategoryId, TicketSubdomain, TicketSubject2, TicketDesc2, DateTime.Now)
            };

            var unknownFilter = (TicketFilterStatusEnum)999;
            var result = ticketService.FilterTicketsByStatus(tickets, unknownFilter).ToList();

            Assert.AreEqual(2, result.Count);
        }
    }
}