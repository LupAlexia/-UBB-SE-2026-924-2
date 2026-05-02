using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.Src.Model.Ticket;
using AirportApp.Src.Repository;

namespace AirportApp.Src.Dto
{
    public record TicketDTO(
        int ticketId,
        int creatorAccountId,
        string creatorEmailAddress,
        ComplaintTicketUrgencyLevelEnum urgencyLevel,
        ComplaintTicketStatusEnum currentStatus,
        int categoryId,
        string categoryName,
        int subcategoryId,
        string subcategoryName,
        string subject,
        string description,
        DateTime creationTimestamp)
    { }
}
