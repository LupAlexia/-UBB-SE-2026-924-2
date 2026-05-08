using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;

namespace AirportApp.ClassLibrary.Repository.Interfaces
{
    public interface ITicketSubcategoryRepository
    {
        Task<IEnumerable<ComplaintTicketSubcategory>> GetAllAsync();

        Task<ComplaintTicketSubcategory> GetByIdAsync(int subcategoryId);

        Task<IEnumerable<ComplaintTicketSubcategory>> GetByCategoryIdAsync(int categoryId);
    }
}