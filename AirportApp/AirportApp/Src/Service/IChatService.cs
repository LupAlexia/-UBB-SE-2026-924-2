using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Chats;

namespace AirportApp.Src.Service
{
    public interface IChatService
    {
        Task<Chat> OpenChatAsync(int userId);
        Task CloseChatAsync(int chatId);
    }
}
