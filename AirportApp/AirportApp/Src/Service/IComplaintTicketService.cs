using System;
using System.Collections.Generic;
using AirportApp.Src.Dto;
using AirportApp.Src.Model;
using AirportApp.Src.Model.Ticket;
using AirportApp.Src.ViewModel;

namespace AirportApp.Src.Service.Interfaces
{
    public interface IComplaintTicketService
    {
        void CreateTicket(int ticketId, User ticketCreator, ComplaintTicketStatusEnum initialStatus,
            ComplaintTicketCategory category, ComplaintTicketSubcategory subcategory,
            string subject, string description, DateTime creationTimestamp,
            ComplaintTicketUrgencyLevelEnum? initialUrgencyLevel = null);

        void AddTicket(ComplaintTicket ticketEntity);

        void DeleteTicketById(int ticketId);

        ComplaintTicket GetTicketById(int ticketId);

        IEnumerable<ComplaintTicket> GetAllTickets();

        void UpdateTicketById(int id, ComplaintTicket ticket);

        void UpdateUrgencyLevel(int ticketId, ComplaintTicketUrgencyLevelEnum newUrgencyLevel);

        void UpdateStatus(int ticketId, ComplaintTicketStatusEnum newStatus);

        IEnumerable<TicketDTO> FilterTicketsByStatus(IEnumerable<TicketDTO> tickets, TicketFilterStatusEnum filter);
    }
}