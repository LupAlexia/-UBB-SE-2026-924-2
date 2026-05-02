using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.Src.Dto;
using AirportApp.Src.Model;
using AirportApp.Src.Model.Ticket;
using AirportApp.Src.Repository;
using AirportApp.Src.Repository.Interfaces;
using AirportApp.Src.Service.Interfaces;
using AirportApp.Src.ViewModel;
using Windows.System;
using User = AirportApp.Src.Model.User;

namespace AirportApp.Src.Service
{
    public class ComplaintTicketService : IComplaintTicketService
    {
        private readonly IComplaintTicketRepository ticketRepository;

        public ComplaintTicketService(IComplaintTicketRepository ticketRepository)
        {
            this.ticketRepository = ticketRepository;
        }

        public void CreateTicket(int ticketId, User ticketCreator, ComplaintTicketStatusEnum initialStatus, ComplaintTicketCategory category, ComplaintTicketSubcategory subcategory, string subject, string description, DateTime creationTimestamp, ComplaintTicketUrgencyLevelEnum? initialUrgencyLevel = null)
        {
            ComplaintTicket newTicket = new ComplaintTicket(ticketId, ticketCreator, initialStatus, category, subcategory, subject, description, creationTimestamp, initialUrgencyLevel);

            ValidateTicket(newTicket);
            AddTicket(newTicket);
        }

        public void AddTicket(ComplaintTicket ticketEntity)
        {
            ticketRepository.CreateNewEntity(ticketEntity);
        }
        public void DeleteTicketById(int ticketId)
        {
            ticketRepository.DeleteById(ticketId);
        }
        public ComplaintTicket GetTicketById(int ticketId)
        {
            return ticketRepository.GetById(ticketId);
        }

        public IEnumerable<ComplaintTicket> GetAllTickets()
        {
            return ticketRepository.GetAll();
        }
        public void UpdateTicketById(int identificationNumber, ComplaintTicket ticket)
        {
            ticketRepository.UpdateById(identificationNumber, ticket);
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
            if (ticket.Subcategory.ParentCategory.CategoryId != ticket.Category.CategoryId)
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

        public void UpdateUrgencyLevel(int ticketId, ComplaintTicketUrgencyLevelEnum newUrgencyLevel)
        {
            ComplaintTicket targetTicket = ticketRepository.GetById(ticketId);
            targetTicket.UpdateUrgencyLevel(newUrgencyLevel);
            ticketRepository.UpdateById(ticketId, targetTicket);
        }

        public void UpdateStatus(int ticketId, ComplaintTicketStatusEnum newStatus)
        {
            ComplaintTicket targetTicket = ticketRepository.GetById(ticketId);
            targetTicket.UpdateStatus(newStatus);
            ticketRepository.UpdateById(ticketId, targetTicket);
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
