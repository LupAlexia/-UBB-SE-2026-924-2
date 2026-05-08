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
    public class ComplaintTicketCategory
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
        public ComplaintTicketUrgencyLevelEnum CategoryUrgencyLevel { get; set; }

        public ComplaintTicketCategory()
        {
        }
        public ComplaintTicketCategory(int categoryId, string categoryName, ComplaintTicketUrgencyLevelEnum categoryUrgencyLevel)
        {
            Id = categoryId;
            CategoryName = categoryName;
            CategoryUrgencyLevel = categoryUrgencyLevel;
        }
    }
}
