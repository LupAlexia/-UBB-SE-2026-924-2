using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.Src.Model.Chats;

namespace AirportApp.Src.Dto
{
    public record ChatDTO(int chatId, int userId, ChatStatus status, int messageCount);
}
