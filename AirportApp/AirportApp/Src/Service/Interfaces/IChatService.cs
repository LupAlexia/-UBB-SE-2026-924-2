using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Src.Service.Interfaces
{
    public interface IChatService
    {
        Task<Chat> OpenChatAsync(User userToOpenChatFor);
        Task CloseChatAsync(int chatId);
    }
}
