using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interfaces
{
    public interface IMessageRepository : IRepository<int, Message>
    {
        Task<IEnumerable<Message>> GetByChatIdAsync(int chatId);

        Task<IEnumerable<Message>> GetMessagesSinceAsync(int chatId, int firstMessageId);
    }
}
