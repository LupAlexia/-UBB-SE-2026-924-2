using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    public enum ComplaintTicketStatusEnum
    {
        OPEN,
        IN_PROGRESS,
        RESOLVED
    }

    public enum TicketFilterStatusEnum
    {
        ALL,
        OPEN,
        IN_PROGRESS,
        RESOLVED
    }
}
