using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.Src.ViewModel;

namespace AirportApp.Src.Service.Interfaces
{
    public interface ITicketService
    {
        Task CreateTicketAsync(int ticketId, User ticketCreator, TicketStatusEnum initialStatus,
            TicketCategory category, TicketSubcategory subcategory,
            string subject, string description, DateTime creationTimestamp,
            TicketUrgencyLevelEnum? initialUrgencyLevel = null);

        Task AddTicketAsync(Ticket ticketEntity);

        Task DeleteTicketByIdAsync(int ticketId);

        Task<Ticket> GetTicketByIdAsync(int ticketId);

        Task<IEnumerable<Ticket>> GetAllTicketsAsync();

        Task UpdateTicketByIdAsync(int id, Ticket ticket);

        Task UpdateUrgencyLevelAsync(int ticketId, TicketUrgencyLevelEnum newUrgencyLevel);

        Task UpdateStatusAsync(int ticketId, TicketStatusEnum newStatus);

        IEnumerable<TicketDTO> FilterTicketsByStatus(IEnumerable<TicketDTO> tickets, TicketFilterStatusEnum filter);
    }
}