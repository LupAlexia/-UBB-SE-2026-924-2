using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirportApp.ClassLibrary.Entity.Domain.Ticket
{
    [Table("Tickets")]
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
        public TicketUrgencyLevelEnum UrgencyLevel { get; set; }

        [Required]
        [Column("Status")]
        public TicketStatusEnum CurrentStatus { get; set; }

        // 2. Navigation Properties & Foreign Keys

        [Required]
        [Column("Creator_Id")]
        public int CreatorId { get; set; }

        [ForeignKey(nameof(CreatorId))]
        public User Creator { get; set; } = null!;

        [Required]
        [Column("Category_Id")]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public TicketCategory Category { get; set; } = null!;

        [Required]
        [Column("Subcategory_Id")]
        public int SubcategoryId { get; set; }

        [ForeignKey(nameof(SubcategoryId))]
        public TicketSubcategory Subcategory { get; set; } = null!;

        public Ticket() { }
        public Ticket(int ticketId, User ticketCreator, TicketStatusEnum initialStatus, TicketCategory category, TicketSubcategory subcategory, string ticketSubject, string description, DateTime creationTimestamp, TicketUrgencyLevelEnum? initialUrgencyLevel = null)
        {
            Id = ticketId;
            Creator = ticketCreator;
            CreatorId = ticketCreator.UserId;
            UrgencyLevel = initialUrgencyLevel ?? category.CategoryUrgencyLevel;
            CurrentStatus = initialStatus;
            Category = category;
            CategoryId = category.CategoryId;
            Subcategory = subcategory;
            SubcategoryId = subcategory.SubcategoryId;
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

