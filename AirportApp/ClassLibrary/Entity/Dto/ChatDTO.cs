using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Entity.Dto
{
    public record ChatDTO(int chatId, int userId, ChatStatus status, int messageCount);
}
