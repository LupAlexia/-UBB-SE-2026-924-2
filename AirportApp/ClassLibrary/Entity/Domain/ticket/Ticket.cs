using System;

namespace AirportApp.ClassLibrary.Entity.Domain.Ticket
{
    public class Ticket
    {
        //public int Id { get; set; }
        //public User Creator { get; }
        //public TicketUrgencyLevelEnum UrgencyLevel { get; set; }
        //public TicketStatusEnum CurrentStatus { get; set; }
        //public TicketCategory Category { get; }
        //public TicketSubcategory Subcategory { get; }
        //public string Subject { get; set; } = string.Empty;
        //public string Description { get; set; } = string.Empty;
        //public DateTime CreationTimestamp { get; set; }

        // 1. EF Core Auto-Properties
        public int Id { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreationTimestamp { get; set; }
        public TicketUrgencyLevelEnum UrgencyLevel { get; set; }
        public TicketStatusEnum CurrentStatus { get; set; }

        // 2. Navigation Properties & Foreign Keys
        public int CreatorId { get; set; }
        public User Creator { get; set; } = null!;

        public int CategoryId { get; set; }
        public TicketCategory Category { get; set; } = null!;

        public int SubcategoryId { get; set; }
        public TicketSubcategory Subcategory { get; set; } = null!;

        public Ticket() { }
        public Ticket(int ticketId, User ticketCreator, TicketStatusEnum initialStatus, TicketCategory category, TicketSubcategory subcategory, string ticketSubject, string description, DateTime creationTimestamp, TicketUrgencyLevelEnum? initialUrgencyLevel = null)
        {
            Id = ticketId;
            Creator = ticketCreator;
            UrgencyLevel = initialUrgencyLevel ?? category.CategoryUrgencyLevel;
            CurrentStatus = initialStatus;
            Category = category;
            Subcategory = subcategory;
            Subject = ticketSubject;
            Description = description;
            CreationTimestamp = creationTimestamp;
        }
        public void UpdateStatus(TicketStatusEnum newStatus)
        {
            CurrentStatus = newStatus;
        }

        public void UpdateUrgencyLevel(TicketUrgencyLevelEnum newUrgencyLevel)
        {
           UrgencyLevel = newUrgencyLevel;
        }
    }
 }

