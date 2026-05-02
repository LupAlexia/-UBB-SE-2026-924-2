using System.Collections.Generic;
using AirportApp.Src.Model.Ticket;

namespace AirportApp.Src.Service.Interfaces
{
    public interface IComplaintTicketSubcategoryService
    {
        ComplaintTicketSubcategory GetSubcategoryById(int subcategoryId);

        IEnumerable<ComplaintTicketSubcategory> GetSubcategoriesByCategoryId(int categoryId);
    }
}