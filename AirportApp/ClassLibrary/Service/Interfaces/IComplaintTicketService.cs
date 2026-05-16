using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interfaces
{
    public interface IComplaintTicketService
    {
        Task CreateTicketAsync(int ticketId, User ticketCreator, ComplaintTicketStatusEnum initialStatus,
            ComplaintTicketCategory category, ComplaintTicketSubcategory subcategory,
            string subject, string description, DateTime creationTimestamp,
            ComplaintTicketUrgencyLevelEnum? initialUrgencyLevel = null);

        Task AddTicketAsync(ComplaintTicket ticketEntity);

        Task DeleteTicketByIdAsync(int ticketId);

        Task<ComplaintTicket> GetTicketByIdAsync(int ticketId);

        Task<IEnumerable<ComplaintTicket>> GetAllTicketsAsync();

        Task UpdateTicketByIdAsync(int id, ComplaintTicket ticket);

        Task UpdateUrgencyLevelAsync(int ticketId, ComplaintTicketUrgencyLevelEnum newUrgencyLevel);

        Task UpdateStatusAsync(int ticketId, ComplaintTicketStatusEnum newStatus);

        IEnumerable<TicketDTO> FilterTicketsByStatus(IEnumerable<TicketDTO> tickets, TicketFilterStatusEnum filter);
    }
}