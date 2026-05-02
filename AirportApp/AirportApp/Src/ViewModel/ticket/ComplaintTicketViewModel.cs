using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Linq;
using AutoMapper;
using AirportApp.Src.Dto;
using AirportApp.Src.Model;
using AirportApp.Src.Model.Ticket;
using AirportApp.Src.Repository.Database;
using AirportApp.Src.Service;
using Microsoft.Data.SqlClient;
using AirportApp.Src.Service.Interfaces;
using AirportApp.Src.Dto;
using AirportApp.Src.Model.Ticket;
using AirportApp.Src.Service;
using AutoMapper;

namespace AirportApp.Src.ViewModel
{
    public class ComplaintTicketViewModel
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

        public ComplaintTicketViewModel(IComplaintTicketService ticketService, IComplaintTicketCategoryService categoryService, IComplaintTicketSubcategoryService subcategoryService, IUserService userService, IMapper mapper)
        {
            this.ticketService = ticketService;
            this.categoryService = categoryService;
            this.subcategoryService = subcategoryService;
            this.userService = userService;
            this.mapper = mapper;

            LoadTickets();
            LoadCategories();
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
        private void LoadTickets()
        {
            var ticketsFromDatabase = ticketService.GetAllTickets();

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
        public void UpdateStatus(int ticketId, ComplaintTicketStatusEnum newStatus)
        {
            ticketService.UpdateStatus(ticketId, newStatus);
            LoadTickets();
        }

        // =================================
        // UPDATE URGENCY
        // =================================
        public void UpdateUrgencyLevel(int ticketId, ComplaintTicketUrgencyLevelEnum newUrgencyLevel)
        {
            ticketService.UpdateUrgencyLevel(ticketId, newUrgencyLevel);
            LoadTickets();
        }

        // =================================
        // CREATE TICKET
        // =================================
        public void CreateTicket(TicketDTO ticketDataTransferObject)
        {
            // Fetch related entities from DB
            var creator = userService.GetById(ticketDataTransferObject.creatorAccountId);
            var category = categoryService.GetCategoryById(ticketDataTransferObject.categoryId);
            var subcategory = subcategoryService.GetSubcategoryById(ticketDataTransferObject.subcategoryId);

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

            ticketService.AddTicket(ticket);
            LoadTickets();
        }

        private void LoadCategories()
        {
            Categories.Clear();
            foreach (var categoryEntity in categoryService.GetAllCategories())
            {
                Categories.Add(categoryEntity);
            }
        }

        public void LoadSubcategories(int categoryId)
        {
            Subcategories.Clear();
            foreach (var subcategoryEntity in subcategoryService.GetSubcategoriesByCategoryId(categoryId))
            {
                Subcategories.Add(subcategoryEntity);
            }
        }
    }

    public enum TicketFilterStatusEnum
    {
        ALL,
        OPEN,
        IN_PROGRESS,
        RESOLVED
    }
}