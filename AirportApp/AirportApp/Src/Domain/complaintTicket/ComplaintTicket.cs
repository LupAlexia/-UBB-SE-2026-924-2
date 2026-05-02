using System;
using AirportApp.Src.Model;

namespace AirportApp.Src.Model.Ticket
{
    public class ComplaintTicket
    {
        public int TicketId { get; }
        public User Creator { get; }
        public ComplaintTicketUrgencyLevelEnum UrgencyLevel { get; private set; }
        public ComplaintTicketStatusEnum CurrentStatus { get; private set; }
        public ComplaintTicketCategory Category { get; }
        public ComplaintTicketSubcategory Subcategory { get; }
        public string Subject { get; }
        public string Description { get; }
        public DateTime CreationTimestamp { get; }
        public ComplaintTicket(int ticketId, User ticketCreator, ComplaintTicketStatusEnum initialStatus, ComplaintTicketCategory category, ComplaintTicketSubcategory subcategory, string ticketSubject, string description, DateTime creationTimestamp, ComplaintTicketUrgencyLevelEnum? initialUrgencyLevel = null)
        {
            TicketId = ticketId;
            Creator = ticketCreator;
            UrgencyLevel = initialUrgencyLevel ?? category.CategoryUrgencyLevel;
            CurrentStatus = initialStatus;
            Category = category;
            Subcategory = subcategory;
            Subject = ticketSubject;
            Description = description;
            CreationTimestamp = creationTimestamp;
        }
        public void UpdateStatus(ComplaintTicketStatusEnum newStatus)
        {
            CurrentStatus = newStatus;
        }

        public void UpdateUrgencyLevel(ComplaintTicketUrgencyLevelEnum newUrgencyLevel)
        {
            UrgencyLevel = newUrgencyLevel;
        }
    }
 }

