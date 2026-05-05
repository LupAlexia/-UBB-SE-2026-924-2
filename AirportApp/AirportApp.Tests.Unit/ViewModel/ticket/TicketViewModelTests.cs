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
        private ITicketService _ticketService;
        private ITicketCategoryService _categoryService;
        private ITicketSubcategoryService _subcategoryService;
        private IUserService _userService;
        private IMapper _mapper;
        private TicketsViewModel _ticketsViewModel;

        private TicketCategory _testCategory;
        private TicketSubcategory _testSubcategory;
        private User _testUser;

        [TestInitialize]
        public void Setup()
        {
            _ticketService = Substitute.For<ITicketService>();
            _categoryService = Substitute.For<ITicketCategoryService>();
            _subcategoryService = Substitute.For<ITicketSubcategoryService>();
            _userService = Substitute.For<IUserService>();
            _mapper = Substitute.For<IMapper>();

            _testCategory = new TicketCategory(1, "Hardware", TicketUrgencyLevelEnum.MEDIUM);
            _testSubcategory = new TicketSubcategory(10, "Monitor", 100, _testCategory);
            _testUser = new User(42, "Dede", "dedeee@airport.com");

            _mapper.Map<TicketDTO>(Arg.Any<Ticket>()).Returns(callInfo => MapToDto((Ticket)callInfo[0]));

            var initialTickets = new List<Ticket>
            {
                new Ticket(1, _testUser, TicketStatusEnum.OPEN, _testCategory, _testSubcategory, "Issue 1", "Desc 1", DateTime.Now)
            };

            _ticketService.GetAllTicketsAsync().Returns(Task.FromResult((IEnumerable<Ticket>)initialTickets));
            _categoryService.GetAllCategoriesAsync().Returns(Task.FromResult((IEnumerable<TicketCategory>)new List<TicketCategory> { _testCategory }));

            _ticketsViewModel = new TicketsViewModel(_ticketService, _categoryService, _subcategoryService, _userService, _mapper);
        }

        [TestMethod]
        public void Constructor_WhenCalled_ShouldInitializeCollectionsAndLoadData()
        {
            Assert.AreEqual(1, _ticketsViewModel.AllTickets.Count);
            Assert.AreEqual(1, _ticketsViewModel.Categories.Count);
            _ticketService.Received(1).GetAllTicketsAsync();
        }

        [TestMethod]
        public void GetAllTickets_WhenCalled_ShouldReturnAllTickets()
        {
            var resultedTickets = _ticketsViewModel.GetAllTickets();
            Assert.IsNotNull(resultedTickets);
            Assert.AreEqual(1, resultedTickets.Count());
            Assert.AreEqual(_ticketsViewModel.AllTickets.First(), resultedTickets.First());
        }

        [TestMethod]
        public void GetTotalTicketCount_WhenCalled_ShouldReturnCountOfAllTickets()
        {
            var ticketsCount = _ticketsViewModel.GetTotalTicketCount();
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

            _userService.GetByIdAsync(42).Returns(Task.FromResult(_testUser));
            _categoryService.GetCategoryByIdAsync(1).Returns(Task.FromResult(_testCategory));
            _subcategoryService.GetSubcategoryByIdAsync(10).Returns(Task.FromResult(_testSubcategory));

            await _ticketsViewModel.CreateTicketAsync(ticketDataTransferObject);

            await _ticketService.Received(1).AddTicketAsync(Arg.Is<Ticket>(ticket =>
                ticket.Id == 101 &&
                ticket.Subject == "Broken Screen" &&
                ticket.Creator.Id == 42));

            await _ticketService.Received(2).GetAllTicketsAsync();
        }

        [TestMethod]
        public async Task UpdateStatus_WhenCalled_ShouldTriggerServiceUpdate()
        {
            await _ticketsViewModel.UpdateStatusAsync(1, TicketStatusEnum.RESOLVED);

            await _ticketService.Received(1).UpdateStatusAsync(1, TicketStatusEnum.RESOLVED);
            await _ticketService.Received(2).GetAllTicketsAsync();
        }

        [TestMethod]
        public async Task UpdateUrgencyLevel_WhenCalled_ShouldCallServiceAndUpdateLocalList()
        {
            int targetId = 1;
            var newUrgency = TicketUrgencyLevelEnum.HIGH;

            await _ticketsViewModel.UpdateUrgencyLevelAsync(targetId, newUrgency);
            await _ticketService.Received(1).UpdateUrgencyLevelAsync(targetId, newUrgency);
            await _ticketService.Received(2).GetAllTicketsAsync();
        }

        [TestMethod]
        public void FilterByStatus_WhenCalled_ShouldUpdateFilteredDisplayCollection()
        {
            var filteredResults = new List<TicketDTO> { _ticketsViewModel.AllTickets[0] };
            _ticketService.FilterTicketsByStatus(Arg.Any<IEnumerable<TicketDTO>>(), TicketFilterStatusEnum.OPEN).Returns(filteredResults);

            _ticketsViewModel.SelectedFilterStatus = AirportApp.Src.ViewModel.TicketFilterStatusEnum.OPEN;

            Assert.AreEqual(1, _ticketsViewModel.FilteredTicketsForDisplay.Count);
            _ticketService.Received().FilterTicketsByStatus(Arg.Any<IEnumerable<TicketDTO>>(), AirportApp.Src.ViewModel.TicketFilterStatusEnum.OPEN);
        }

        [TestMethod]
        public async Task LoadSubcategories_WhenCalled_ShouldPopulateCorrectCategory()
        {
            var subcategoriesList = new List<TicketSubcategory> { _testSubcategory };
            _subcategoryService.GetSubcategoriesByCategoryIdAsync(1).Returns(Task.FromResult((IEnumerable<TicketSubcategory>)subcategoriesList));

            await _ticketsViewModel.LoadSubcategoriesAsync(1);

            Assert.AreEqual(1, _ticketsViewModel.Subcategories.Count);
            Assert.AreEqual("Monitor", _ticketsViewModel.Subcategories[0].SubcategoryName);
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
                ticket.CreationTimestamp
            );
        }
    }
}
