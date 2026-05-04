using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;

namespace AirportApp.Src.Service.Interfaces
{
    public interface ITicketCategoryService
    {
        Task<TicketCategory> GetCategoryByIdAsync(int categoryId);

        Task<IEnumerable<TicketCategory>> GetAllCategoriesAsync();
    }
}