using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;

namespace AirportApp.ClassLibrary.Repository.Interfaces
{
    public interface ITicketSubcategoryRepository
    {
        Task<IEnumerable<TicketSubcategory>> GetAllAsync();

        Task<TicketSubcategory> GetByIdAsync(int subcategoryId);

        Task<IEnumerable<TicketSubcategory>> GetByCategoryIdAsync(int categoryId);
    }
}