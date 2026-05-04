using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Message;

namespace AirportApp.ClassLibrary.Repository.Interfaces
{
    public interface IMessageRepository : IRepository<int, Message>
    {
        Task<IEnumerable<Message>> GetByChatIdAsync(int chatId);

        Task<IEnumerable<Message>> GetMessagesSinceAsync(int chatId, int firstMessageId);
    }
}
