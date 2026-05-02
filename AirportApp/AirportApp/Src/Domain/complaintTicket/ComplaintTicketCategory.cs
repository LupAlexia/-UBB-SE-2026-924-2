using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportApp.Src.Model.Ticket
{
    public class ComplaintTicketCategory
    {
        public int CategoryId { get; }
        public string CategoryName { get; }

        public ComplaintTicketUrgencyLevelEnum CategoryUrgencyLevel { get; }

        public ComplaintTicketCategory(int categoryId, string categoryName, ComplaintTicketUrgencyLevelEnum categoryUrgencyLevel)
        {
            CategoryId = categoryId;
            CategoryName = categoryName;
            CategoryUrgencyLevel = categoryUrgencyLevel;
        }
    }
}
