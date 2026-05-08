using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interfaces
{
    public interface ITicketCategoryRepository
    {
        Task<IEnumerable<ComplaintTicketCategory>> GetAllAsync();

        Task<ComplaintTicketCategory> GetByIdAsync(int categoryId);
    }
}