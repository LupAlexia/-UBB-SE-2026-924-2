using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;

namespace AirportApp.ClassLibrary.Repository.Interfaces
{
    public interface ITicketCategoryRepository
    {
        Task<IEnumerable<TicketCategory>> GetAllAsync();

        Task<TicketCategory> GetByIdAsync(int categoryId);
    }
}