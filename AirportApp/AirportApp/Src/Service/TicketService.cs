using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.Src.Service.Interfaces;

using User = AirportApp.ClassLibrary.Entity.Domain.User;
using AirportApp.Src.ViewModel;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.Src.Service
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository ticketRepository;

        public TicketService(ITicketRepository ticketRepository)
        {
            this.ticketRepository = ticketRepository;
        }

        public async Task CreateTicketAsync(int ticketId, User ticketCreator, TicketStatusEnum initialStatus, TicketCategory category, TicketSubcategory subcategory, string subject, string description, DateTime creationTimestamp, TicketUrgencyLevelEnum? initialUrgencyLevel = null)
        {
            Ticket newTicket = new Ticket(ticketId, ticketCreator, initialStatus, category, subcategory, subject, description, creationTimestamp, initialUrgencyLevel);

            ValidateTicket(newTicket);
            await AddTicketAsync(newTicket);
        }

        public async Task AddTicketAsync(Ticket ticketEntity)
        {
            await ticketRepository.CreateNewEntityAsync(ticketEntity);
        }

        public async Task DeleteTicketByIdAsync(int ticketId)
        {
            await ticketRepository.DeleteByIdAsync(ticketId);
        }

        public async Task<Ticket> GetTicketByIdAsync(int ticketId)
        {
            return await ticketRepository.GetByIdAsync(ticketId);
        }

        public async Task<IEnumerable<Ticket>> GetAllTicketsAsync()
        {
            return await ticketRepository.GetAllAsync();
        }

        public async Task UpdateTicketByIdAsync(int identificationNumber, Ticket ticket)
        {
            await ticketRepository.UpdateByIdAsync(identificationNumber, ticket);
        }

        public void ValidateTicket(Ticket ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException("The newTicket does not have any data.");
            }
            if (ticket.Creator == null)
            {
                throw new ArgumentNullException("The ticketCreator does not have any data.");
            }
            if (ticket.Category == null)
            {
                throw new ArgumentNullException("Null Category.");
            }
            if (ticket.Subcategory == null)
            {
                throw new ArgumentNullException("Null Subcategory.");
            }
            if (ticket.Subcategory.ParentCategory.Id != ticket.Category.Id)
            {
                throw new ArgumentException($"The subcategory '{ticket.Subcategory.SubcategoryName}' does not belong to the category '{ticket.Category.CategoryName}'");
            }
            if (string.IsNullOrWhiteSpace(ticket.Subject))
            {
                throw new ArgumentNullException("The Subject is empty.");
            }
            if (string.IsNullOrWhiteSpace(ticket.Description))
            {
                throw new ArgumentNullException("The Description is empty.");
            }
        }

        public async Task UpdateUrgencyLevelAsync(int ticketId, TicketUrgencyLevelEnum newUrgencyLevel)
        {
            Ticket targetTicket = await ticketRepository.GetByIdAsync(ticketId);
            targetTicket.UpdateUrgencyLevel(newUrgencyLevel);
            await ticketRepository.UpdateByIdAsync(ticketId, targetTicket);
        }

        public async Task UpdateStatusAsync(int ticketId, TicketStatusEnum newStatus)
        {
            Ticket targetTicket = await ticketRepository.GetByIdAsync(ticketId);
            targetTicket.UpdateStatus(newStatus);
            await ticketRepository.UpdateByIdAsync(ticketId, targetTicket);
        }

        public IEnumerable<TicketDTO> FilterTicketsByStatus(IEnumerable<TicketDTO> tickets, TicketFilterStatusEnum filter)
        {
            switch (filter)
            {
                case TicketFilterStatusEnum.OPEN:
                    return tickets.Where(IsStatusOpen);
                case TicketFilterStatusEnum.IN_PROGRESS:
                    return tickets.Where(IsStatusInProgress);
                case TicketFilterStatusEnum.RESOLVED:
                    return tickets.Where(IsStatusResolved);
                default:
                    return tickets;
            }
        }

        private bool IsStatusOpen(TicketDTO ticket) => ticket.currentStatus == TicketStatusEnum.OPEN;
        private bool IsStatusInProgress(TicketDTO ticket) => ticket.currentStatus == TicketStatusEnum.IN_PROGRESS;
        private bool IsStatusResolved(TicketDTO ticket) => ticket.currentStatus == TicketStatusEnum.RESOLVED;
    }
}
