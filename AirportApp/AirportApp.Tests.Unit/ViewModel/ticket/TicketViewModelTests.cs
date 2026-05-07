using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using AirportApp.Src.Service.Interfaces;
using AirportApp.Src.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AirportApp.Tests.Unit.Src.ViewModel
{
    [TestClass]
    public class TicketsViewModelTests
    {
        private ITicketService ticketService;
        private ITicketCategoryService categoryService;
        private ITicketSubcategoryService subcategoryService;
        private IUserService userService;
        private IMapper mapper;
        private TicketsViewModel ticketsViewModel;

        private TicketCategory testCategory;
        private TicketSubcategory testSubcategory;
        private User testUser;

        [TestInitialize]
        public void Setup()
        {
            ticketService = Substitute.For<ITicketService>();
            categoryService = Substitute.For<ITicketCategoryService>();
            subcategoryService = Substitute.For<ITicketSubcategoryService>();
            userService = Substitute.For<IUserService>();
            mapper = Substitute.For<IMapper>();

            testCategory = new TicketCategory(1, "Hardware", TicketUrgencyLevelEnum.MEDIUM);
            testSubcategory = new TicketSubcategory(10, "Monitor", 100, testCategory);
            testUser = new User(42, "Dede", "dedeee@airport.com");

            mapper.Map<TicketDTO>(Arg.Any<Ticket>()).Returns(callInfo => MapToDto((Ticket)callInfo[0]));

            var initialTickets = new List<Ticket>
            {
                new Ticket(1, testUser, TicketStatusEnum.OPEN, testCategory, testSubcategory, "Issue 1", "Desc 1", DateTime.Now)
            };

            ticketService.GetAllTicketsAsync().Returns(Task.FromResult((IEnumerable<Ticket>)initialTickets));
            categoryService.GetAllCategoriesAsync().Returns(Task.FromResult((IEnumerable<TicketCategory>)new List<TicketCategory> { testCategory }));

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
                TicketUrgencyLevelEnum.HIGH, TicketStatusEnum.OPEN,
                1, "Hardware", 10, "Monitor",
                "Broken Screen", "The screen is crackedddd", DateTime.Now);

            userService.GetByIdAsync(42).Returns(Task.FromResult(testUser));
            categoryService.GetCategoryByIdAsync(1).Returns(Task.FromResult(testCategory));
            subcategoryService.GetSubcategoryByIdAsync(10).Returns(Task.FromResult(testSubcategory));

            await ticketsViewModel.CreateTicketAsync(ticketDataTransferObject);

            await ticketService.Received(1).AddTicketAsync(Arg.Is<Ticket>(ticket =>
                ticket.Id == 101 &&
                ticket.Subject == "Broken Screen" &&
                ticket.Creator.Id == 42));

            await ticketService.Received(2).GetAllTicketsAsync();
        }

        [TestMethod]
        public async Task UpdateStatus_WhenCalled_ShouldTriggerServiceUpdate()
        {
            await ticketsViewModel.UpdateStatusAsync(1, TicketStatusEnum.RESOLVED);

            await ticketService.Received(1).UpdateStatusAsync(1, TicketStatusEnum.RESOLVED);
            await ticketService.Received(2).GetAllTicketsAsync();
        }

        [TestMethod]
        public async Task UpdateUrgencyLevel_WhenCalled_ShouldCallServiceAndUpdateLocalList()
        {
            int targetId = 1;
            var newUrgency = TicketUrgencyLevelEnum.HIGH;

            await ticketsViewModel.UpdateUrgencyLevelAsync(targetId, newUrgency);
            await ticketService.Received(1).UpdateUrgencyLevelAsync(targetId, newUrgency);
            await ticketService.Received(2).GetAllTicketsAsync();
        }

        [TestMethod]
        public void FilterByStatus_WhenCalled_ShouldUpdateFilteredDisplayCollection()
        {
            var filteredResults = new List<TicketDTO> { ticketsViewModel.AllTickets[0] };
            ticketService.FilterTicketsByStatus(Arg.Any<IEnumerable<TicketDTO>>(), TicketFilterStatusEnum.OPEN).Returns(filteredResults);

            ticketsViewModel.SelectedFilterStatus = AirportApp.Src.ViewModel.TicketFilterStatusEnum.OPEN;

            Assert.AreEqual(1, ticketsViewModel.FilteredTicketsForDisplay.Count);
            ticketService.Received().FilterTicketsByStatus(Arg.Any<IEnumerable<TicketDTO>>(), AirportApp.Src.ViewModel.TicketFilterStatusEnum.OPEN);
        }

        [TestMethod]
        public async Task LoadSubcategories_WhenCalled_ShouldPopulateCorrectCategory()
        {
            var subcategoriesList = new List<TicketSubcategory> { testSubcategory };
            subcategoryService.GetSubcategoriesByCategoryIdAsync(1).Returns(Task.FromResult((IEnumerable<TicketSubcategory>)subcategoriesList));

            await ticketsViewModel.LoadSubcategoriesAsync(1);

            Assert.AreEqual(1, ticketsViewModel.Subcategories.Count);
            Assert.AreEqual("Monitor", ticketsViewModel.Subcategories[0].SubcategoryName);
        }

        private static TicketDTO MapToDto(Ticket ticket)
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
