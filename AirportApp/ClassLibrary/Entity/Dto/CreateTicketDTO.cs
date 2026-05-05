using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportApp.ClassLibrary.Entity.Dto
{
    public record CreateTicketDTO(
        int CreatorId,
        int CategoryId,
        int SubcategoryId,
        string Subject,
        string Description,
        DateTime CreationTimestamp,
        TicketStatusEnum CurrentStatus,
        TicketUrgencyLevelEnum UrgencyLevel
    );
}
