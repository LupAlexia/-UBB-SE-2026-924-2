using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;

namespace AirportApp.Src.Service.Interfaces
{
    public interface ITicketSubcategoryService
    {
        Task<TicketSubcategory> GetSubcategoryByIdAsync(int subcategoryId);

        Task<IEnumerable<TicketSubcategory>> GetSubcategoriesByCategoryIdAsync(int categoryId);
    }
}