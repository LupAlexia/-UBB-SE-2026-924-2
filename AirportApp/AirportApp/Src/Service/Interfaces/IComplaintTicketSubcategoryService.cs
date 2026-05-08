using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;

namespace AirportApp.Src.Service.Interfaces
{
    public interface IComplaintTicketSubcategoryService
    {
        Task<ComplaintTicketSubcategory> GetSubcategoryByIdAsync(int subcategoryId);

        Task<IEnumerable<ComplaintTicketSubcategory>> GetSubcategoriesByCategoryIdAsync(int categoryId);
    }
}