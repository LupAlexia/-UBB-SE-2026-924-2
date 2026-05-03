using System.Collections.Generic;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;

namespace AirportApp.Src.Service.Interfaces
{
    public interface ITicketSubcategoryService
    {
        TicketSubcategory GetSubcategoryById(int subcategoryId);

        IEnumerable<TicketSubcategory> GetSubcategoriesByCategoryId(int categoryId);
    }
}