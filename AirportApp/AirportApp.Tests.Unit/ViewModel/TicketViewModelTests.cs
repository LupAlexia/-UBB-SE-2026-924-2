using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interfaces;
using AirportApp.Src.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AirportApp.Tests.Unit.ViewModel
{
    [TestClass]
    public class TicketsViewModelTests
    {
        private IComplaintTicketService ticketService;
        private IComplaintTicketCategoryService categoryService;
        private IComplaintTicketSubcategoryService subcategoryService;
        private IUserService userService;
        private IMapper mapper;
        private TicketsViewModel ticketsViewModel;

        private ComplaintTicketCategory testCategory;
        private ComplaintTicketSubcategory testSubcategory;
        private User testUser;

        [TestInitialize]
        public void Setup()
        {
            ticketService = Substitute.For<IComplaintTicketService>();
            categoryService = Substitute.For<IComplaintTicketCategoryService>();
            subcategoryService = Substitute.For<IComplaintTicketSubcategoryService>();
            userService = Substitute.For<IUserService>();
            mapper = Substitute.For<IMapper>();

            testCategory = new ComplaintTicketCategory(1, "Hardware", ComplaintTicketUrgencyLevelEnum.MEDIUM);
            testSubcategory = new ComplaintTicketSubcategory(10, "Monitor", 100, testCategory);
            testUser = new User(42, "Dede", "dedeee@airport.com");

            mapper.Map<TicketDTO>(Arg.Any<ComplaintTicket>()).Returns(callInfo => MapToDto((ComplaintTicket)callInfo[0]));

            var initialTickets = new List<ComplaintTicket>
            {
                new ComplaintTicket(1, testUser, ComplaintTicketStatusEnum.OPEN, testCategory, testSubcategory, "Issue 1", "Desc 1", DateTime.Now)
            };

            ticketService.GetAllTicketsAsync().Returns(Task.FromResult((IEnumerable<ComplaintTicket>)initialTickets));
            categoryService.GetAllCategoriesAsync().Returns(Task.FromResult((IEnumerable<ComplaintTicketCategory>)new List<ComplaintTicketCategory> { testCategory }));

            ticketsViewModel = new TicketsViewModel(ticketService, categoryService, subcategoryService, userService, mapper);
        }

        [TestMethod]
        public void Constructor_WhenCalled_ShouldInitializeCollectionsAndLoadData()
        {
            Assert.AreEqual(1, ticketsViewModel.AllTickets.Count);
            Assert.AreEqual(1, ticketsViewModel.Categories.Count);
            ticketService.Received(1).GetAllTicketsAsync();
        }

        [TestMethod]
        public void GetAllTickets_WhenCalled_ShouldReturnAllTickets()
        {
            var resultedTickets = ticketsViewModel.GetAllTickets();
            Assert.IsNotNull(resultedTickets);
            Assert.AreEqual(1, resultedTickets.Count());
            Assert.AreEqual(ticketsViewModel.AllTickets.First(), resultedTickets.First());
        }

        [TestMethod]
        public void GetTotalTicketCount_WhenCalled_ShouldReturnCountOfAllTickets()
        {
            var ticketsCount = ticketsViewModel.GetTotalTicketCount();
            Assert.AreEqual(1, ticketsCount);
        }

        [TestMethod]
        public async Task CreateTicket_WithValidEntity_SucceedsAndCallsService()
        {
            var ticketDataTransferObject = new TicketDTO(
                101, 42, "dede_the_racoon@gmail.com",
                ComplaintTicketUrgencyLevelEnum.HIGH, ComplaintTicketStatusEnum.OPEN,
                1, "Hardware", 10, "Monitor",
                "Broken Screen", "The screen is crackedddd", DateTime.Now);

            userService.GetByIdAsync(42).Returns(Task.FromResult(testUser));
            categoryService.GetCategoryByIdAsync(1).Returns(Task.FromResult(testCategory));
            subcategoryService.GetSubcategoryByIdAsync(10).Returns(Task.FromResult(testSubcategory));

            await ticketsViewModel.CreateTicketAsync(ticketDataTransferObject);

            await ticketService.Received(1).AddTicketAsync(Arg.Is<ComplaintTicket>(ticket =>
                ticket.Id == 101 &&
                ticket.Subject == "Broken Screen" &&
                ticket.Creator.Id == 42));

            await ticketService.Received(2).GetAllTicketsAsync();
        }

        [TestMethod]
        public async Task UpdateStatus_WhenCalled_ShouldTriggerServiceUpdate()
        {
            await ticketsViewModel.UpdateStatusAsync(1, ComplaintTicketStatusEnum.RESOLVED);

            await ticketService.Received(1).UpdateStatusAsync(1, ComplaintTicketStatusEnum.RESOLVED);
            await ticketService.Received(2).GetAllTicketsAsync();
        }

        [TestMethod]
        public async Task UpdateUrgencyLevel_WhenCalled_ShouldCallServiceAndUpdateLocalList()
        {
            int targetId = 1;
            var newUrgency = ComplaintTicketUrgencyLevelEnum.HIGH;

            await ticketsViewModel.UpdateUrgencyLevelAsync(targetId, newUrgency);
            await ticketService.Received(1).UpdateUrgencyLevelAsync(targetId, newUrgency);
            await ticketService.Received(2).GetAllTicketsAsync();
        }

        [TestMethod]
        public void FilterByStatus_WhenCalled_ShouldUpdateFilteredDisplayCollection()
        {
            var filteredResults = new List<TicketDTO> { ticketsViewModel.AllTickets[0] };
            ticketService.FilterTicketsByStatusAsync(Arg.Any<IEnumerable<TicketDTO>>(), TicketFilterStatusEnum.OPEN).Returns(Task.FromResult((IEnumerable<TicketDTO>)filteredResults));

            ticketsViewModel.SelectedFilterStatus = TicketFilterStatusEnum.OPEN;

            // Note: Since ApplyFilterLogicAsync is fired and forgotten in the setter,
            // verifying the exact count immediately might have race conditions in a real async environment,
            // but for this NSubstitute mock without delay, it usually executes inline.
            Assert.AreEqual(1, ticketsViewModel.FilteredTicketsForDisplay.Count);
            ticketService.Received().FilterTicketsByStatusAsync(Arg.Any<IEnumerable<TicketDTO>>(), TicketFilterStatusEnum.OPEN);
        }

        [TestMethod]
        public async Task LoadSubcategories_WhenCalled_ShouldPopulateCorrectCategory()
        {
            var subcategoriesList = new List<ComplaintTicketSubcategory> { testSubcategory };
            subcategoryService.GetSubcategoriesByCategoryIdAsync(1).Returns(Task.FromResult((IEnumerable<ComplaintTicketSubcategory>)subcategoriesList));

            await ticketsViewModel.LoadSubcategoriesAsync(1);

            Assert.AreEqual(1, ticketsViewModel.Subcategories.Count);
            Assert.AreEqual("Monitor", ticketsViewModel.Subcategories[0].SubcategoryName);
        }

        private static TicketDTO MapToDto(ComplaintTicket ticket)
        {
            return new TicketDTO(
                ticket.Id,
                ticket.Creator.Id,
                ticket.Creator.EmailAddress,
                ticket.UrgencyLevel,
                ticket.CurrentStatus,
                ticket.Category.Id,
                ticket.Category.CategoryName,
                ticket.Subcategory.Id,
                ticket.Subcategory.SubcategoryName,
                ticket.Subject,
                ticket.Description,
                ticket.CreationTimestamp);
        }
    }
}
