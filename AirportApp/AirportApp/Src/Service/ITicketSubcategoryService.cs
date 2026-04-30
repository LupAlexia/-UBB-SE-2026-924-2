using System.Collections.Generic;
using AirportApp.Src.Model.Ticket;

namespace AirportApp.Src.Service.Interfaces
{
    public interface ITicketSubcategoryService
    {
        TicketSubcategory GetSubcategoryById(int subcategoryId);

        IEnumerable<TicketSubcategory> GetSubcategoriesByCategoryId(int categoryId);
    }
}