using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interfaces;

using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Src.ViewModel
{
    public class TicketsViewModel
    {
        private readonly IComplaintTicketService ticketService;
        private readonly IMapper mapper;
        private readonly IComplaintTicketCategoryService categoryService;
        private readonly IComplaintTicketSubcategoryService subcategoryService;
        private readonly IUserService userService;

        public ObservableCollection<TicketDTO> AllTickets { get; } = new ();

        private ObservableCollection<TicketDTO> filteredTicketsForDisplay = new ();
        public ObservableCollection<TicketDTO> FilteredTicketsForDisplay => filteredTicketsForDisplay;

        private TicketFilterStatusEnum selectedFilter = TicketFilterStatusEnum.ALL;

        public ObservableCollection<ComplaintTicketCategory> Categories { get; } = new ();
        public ObservableCollection<ComplaintTicketSubcategory> Subcategories { get; } = new ();

        public TicketsViewModel(IComplaintTicketService ticketService, IComplaintTicketCategoryService categoryService, IComplaintTicketSubcategoryService subcategoryService, IUserService userService, IMapper mapper)
        {
            this.ticketService = ticketService;
            this.categoryService = categoryService;
            this.subcategoryService = subcategoryService;
            this.userService = userService;
            this.mapper = mapper;

            _ = LoadTicketsAsync();
            _ = LoadCategoriesAsync();
        }

        // =================================
        // PUBLIC API (UNCHANGED)
        // =================================
        public IEnumerable<TicketDTO> GetAllTickets()
        {
            return AllTickets;
        }

        public int GetTotalTicketCount()
        {
            return AllTickets.Count;
        }

        public TicketFilterStatusEnum SelectedFilterStatus
        {
            get => selectedFilter;
            set
            {
                if (selectedFilter != value)
                {
                    selectedFilter = value;
                    ApplyFilterLogic();
                }
            }
        }

        public string SelectedFilterString
        {
            get => SelectedFilterStatus.ToString();
            set
            {
                if (Enum.TryParse<TicketFilterStatusEnum>(value, out var filter))
                {
                    SelectedFilterStatus = filter;
                }
            }
        }

        // =================================
        // LOAD FROM DATABASE
        // =================================
        private async Task LoadTicketsAsync()
        {
            var ticketsFromDatabase = await ticketService.GetAllTicketsAsync();

            AllTickets.Clear();

            foreach (var ticketEntity in ticketsFromDatabase)
            {
                var ticketDateTime = mapper.Map<TicketDTO>(ticketEntity);
                AllTickets.Add(ticketDateTime);
            }

            ApplyFilterLogic();
        }

        // =================================
        // FILTER
        // =================================
        private void ApplyFilterLogic()
        {
            filteredTicketsForDisplay.Clear();

            IEnumerable<TicketDTO> filteredResults = ticketService.FilterTicketsByStatus(
                AllTickets,
                SelectedFilterStatus);

            foreach (var ticket in filteredResults)
            {
                filteredTicketsForDisplay.Add(ticket);
            }
        }

        // =================================
        // UPDATE STATUS
        // =================================
        public async Task UpdateStatusAsync(int ticketId, ComplaintTicketStatusEnum newStatus)
        {
            await ticketService.UpdateStatusAsync(ticketId, newStatus);
            await LoadTicketsAsync();
        }

        // =================================
        // UPDATE URGENCY
        // =================================
        public async Task UpdateUrgencyLevelAsync(int ticketId, ComplaintTicketUrgencyLevelEnum newUrgencyLevel)
        {
            await ticketService.UpdateUrgencyLevelAsync(ticketId, newUrgencyLevel);
            await LoadTicketsAsync();
        }

        // =================================
        // CREATE TICKET
        // =================================
        public async Task CreateTicketAsync(TicketDTO ticketDataTransferObject)
        {
            // Fetch related entities from DB
            var creator = await userService.GetByIdAsync(ticketDataTransferObject.creatorAccountId);
            var category = await categoryService.GetCategoryByIdAsync(ticketDataTransferObject.categoryId);
            var subcategory = await subcategoryService.GetSubcategoryByIdAsync(ticketDataTransferObject.subcategoryId);

            var ticket = new ComplaintTicket(
                ticketDataTransferObject.ticketId,
                creator,
                ticketDataTransferObject.currentStatus,
                category,
                subcategory,
                ticketDataTransferObject.subject,
                ticketDataTransferObject.description,
                ticketDataTransferObject.creationTimestamp,
                ticketDataTransferObject.urgencyLevel);

            await ticketService.AddTicketAsync(ticket);
            await LoadTicketsAsync();
        }

        private async Task LoadCategoriesAsync()
        {
            Categories.Clear();
            foreach (var categoryEntity in await categoryService.GetAllCategoriesAsync())
            {
                Categories.Add(categoryEntity);
            }
        }

        public async Task LoadSubcategoriesAsync(int categoryId)
        {
            Subcategories.Clear();
            foreach (var subcategoryEntity in await subcategoryService.GetSubcategoriesByCategoryIdAsync(categoryId))
            {
                Subcategories.Add(subcategoryEntity);
            }
        }
    }
}