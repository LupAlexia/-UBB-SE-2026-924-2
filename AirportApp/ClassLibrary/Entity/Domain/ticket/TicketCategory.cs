using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportApp.ClassLibrary.Entity.Domain.Ticket
{
    public class TicketCategory
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;

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
