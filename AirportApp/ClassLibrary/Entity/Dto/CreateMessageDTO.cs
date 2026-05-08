using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportApp.ClassLibrary.Entity.Dto
{
    public record CreateMessageDTO(
        int chatId,
        string text,
        int? senderUserId,
        int? senderEmployeeId,
        DateTimeOffset timestamp);
}
