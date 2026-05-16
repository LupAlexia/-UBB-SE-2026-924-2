using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.Src.Service.Interfaces;

using User = AirportApp.ClassLibrary.Entity.Domain.User;
using AirportApp.Src.ViewModel;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Src.Service
{
    public class ComplaintTicketService : IComplaintTicketService
    {
        private readonly ITicketRepository ticketRepository;

        public ComplaintTicketService(ITicketRepository ticketRepository)
        {
            this.ticketRepository = ticketRepository;
        }

        public async Task CreateTicketAsync(int ticketId, User ticketCreator, ComplaintTicketStatusEnum initialStatus, ComplaintTicketCategory category, ComplaintTicketSubcategory subcategory, string subject, string description, DateTime creationTimestamp, ComplaintTicketUrgencyLevelEnum? initialUrgencyLevel = null)
        {
            ComplaintTicket newTicket = new ComplaintTicket(ticketId, ticketCreator, initialStatus, category, subcategory, subject, description, creationTimestamp, initialUrgencyLevel);

            ValidateTicket(newTicket);
            await AddTicketAsync(newTicket);
        }

        public async Task AddTicketAsync(ComplaintTicket ticketEntity)
        {
            await ticketRepository.CreateNewEntityAsync(ticketEntity);
        }

        public async Task DeleteTicketByIdAsync(int ticketId)
        {
            await ticketRepository.DeleteByIdAsync(ticketId);
        }

        public async Task<ComplaintTicket> GetTicketByIdAsync(int ticketId)
        {
            return await ticketRepository.GetByIdAsync(ticketId);
        }

        public async Task<IEnumerable<ComplaintTicket>> GetAllTicketsAsync()
        {
            return await ticketRepository.GetAllAsync();
        }

        public async Task UpdateTicketByIdAsync(int identificationNumber, ComplaintTicket ticket)
        {
            await ticketRepository.UpdateByIdAsync(identificationNumber, ticket);
        }

        public void ValidateTicket(ComplaintTicket ticket)
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

        public async Task UpdateUrgencyLevelAsync(int ticketId, ComplaintTicketUrgencyLevelEnum newUrgencyLevel)
        {
            await ticketRepository.UpdateUrgencyLevelByIdAsync(ticketId, newUrgencyLevel);
        }

        public async Task UpdateStatusAsync(int ticketId, ComplaintTicketStatusEnum newStatus)
        {
            await ticketRepository.UpdateStatusByIdAsync(ticketId, newStatus);
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

        private bool IsStatusOpen(TicketDTO ticket) => ticket.currentStatus == ComplaintTicketStatusEnum.OPEN;
        private bool IsStatusInProgress(TicketDTO ticket) => ticket.currentStatus == ComplaintTicketStatusEnum.IN_PROGRESS;
        private bool IsStatusResolved(TicketDTO ticket) => ticket.currentStatus == ComplaintTicketStatusEnum.RESOLVED;
    }
}
