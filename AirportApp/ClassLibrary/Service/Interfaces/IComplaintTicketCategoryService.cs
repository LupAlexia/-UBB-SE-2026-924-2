using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interfaces
{
    public interface IComplaintTicketCategoryService
    {
        Task<ComplaintTicketCategory> GetCategoryByIdAsync(int categoryId);

        Task<IEnumerable<ComplaintTicketCategory>> GetAllCategoriesAsync();
    }
}