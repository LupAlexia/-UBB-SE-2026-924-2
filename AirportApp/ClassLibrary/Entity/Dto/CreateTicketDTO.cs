using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Entity.Dto
{
    public record CreateTicketDTO(
        int creatorId,
        int categoryId,
        int subcategoryId,
        string subject,
        string description,
        DateTime creationTimestamp,
        ComplaintTicketStatusEnum currentStatus,
        ComplaintTicketUrgencyLevelEnum urgencyLevel);
}
