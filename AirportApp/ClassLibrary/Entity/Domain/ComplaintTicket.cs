using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    [Table("Tickets")]
    public class ComplaintTicket
    {
        // 1. EF Core Auto-Properties
        [Key]
        [Column("Ticket_Id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("Subject")]
        public string Subject { get; set; } = string.Empty;

        [Required]
        [Column("Description", TypeName = "NVARCHAR(MAX)")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column("Creation_Timestamp")]
        public DateTime CreationTimestamp { get; set; }

        [Required]
        [Column("Urgency_Level")]
        public ComplaintTicketUrgencyLevelEnum UrgencyLevel { get; set; }

        [Required]
        [Column("Status")]
        public ComplaintTicketStatusEnum CurrentStatus { get; set; }

        public User Creator { get; set; } = null!;

        public ComplaintTicketCategory Category { get; set; } = null!;

        public ComplaintTicketSubcategory Subcategory { get; set; } = null!;

        public ComplaintTicket()
        {
        }
        public ComplaintTicket(int ticketId, User ticketCreator, ComplaintTicketStatusEnum initialStatus, ComplaintTicketCategory category, ComplaintTicketSubcategory subcategory, string ticketSubject, string description, DateTime creationTimestamp, ComplaintTicketUrgencyLevelEnum? initialUrgencyLevel = null)
        {
            Id = ticketId;
            Creator = ticketCreator ?? throw new ArgumentNullException(nameof(ticketCreator));
            Category = category ?? throw new ArgumentNullException(nameof(category));
            UrgencyLevel = initialUrgencyLevel ?? category.CategoryUrgencyLevel;
            CurrentStatus = initialStatus;
            Subcategory = subcategory ?? throw new ArgumentNullException(nameof(subcategory));
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

