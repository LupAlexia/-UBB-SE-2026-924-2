using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportApp.ClassLibrary.Entity.Domain.Ticket
{
    [Table("TicketCategories")]
    public class TicketCategory
    {
        [Key]
        [Column("Category_Id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("Category_Name")]
        public string CategoryName { get; set; } = string.Empty;

        [Required]
        [Column("Default_Urgency_Level")]
        public TicketUrgencyLevelEnum CategoryUrgencyLevel { get; set; }

        public TicketCategory() { }
        public TicketCategory(int categoryId, string categoryName, TicketUrgencyLevelEnum categoryUrgencyLevel)
        {
            Id = categoryId;
            CategoryName = categoryName;
            CategoryUrgencyLevel = categoryUrgencyLevel;
        }
    }
}
