using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportApp.ClassLibrary.Entity.Dto
{
    public record CreateMessageDTO(
        int ChatId,
        string Text,
        int? SenderUserId,
        int? SenderEmployeeId,
        DateTimeOffset Timestamp
    );
}
