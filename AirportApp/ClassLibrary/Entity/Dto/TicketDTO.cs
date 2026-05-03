using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using AirportApp.ClassLibrary.Repository;

namespace AirportApp.ClassLibrary.Entity.Dto
{
    public record TicketDTO(
        int ticketId,
        int creatorAccountId,
        string creatorEmailAddress,
        TicketUrgencyLevelEnum urgencyLevel,
        TicketStatusEnum currentStatus,
        int categoryId,
        string categoryName,
        int subcategoryId,
        string subcategoryName,
        string subject,
        string description,
        DateTime creationTimestamp)
    { }
}
