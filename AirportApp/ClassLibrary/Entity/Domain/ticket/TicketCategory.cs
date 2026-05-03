using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportApp.ClassLibrary.Entity.Domain.Ticket
{
    public class TicketCategory
    {
        public int CategoryId { get; }
        public string CategoryName { get; }

        public TicketUrgencyLevelEnum CategoryUrgencyLevel { get; }

        public TicketCategory(int categoryId, string categoryName, TicketUrgencyLevelEnum categoryUrgencyLevel)
        {
            CategoryId = categoryId;
            CategoryName = categoryName;
            CategoryUrgencyLevel = categoryUrgencyLevel;
        }
    }
}
