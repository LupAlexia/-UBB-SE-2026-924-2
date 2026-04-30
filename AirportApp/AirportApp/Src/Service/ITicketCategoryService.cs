using System.Collections.Generic;
using AirportApp.Src.Model.Ticket;

namespace AirportApp.Src.Service.Interfaces
{
    public interface ITicketCategoryService
    {
        TicketCategory GetCategoryById(int categoryId);

        IEnumerable<TicketCategory> GetAllCategories();
    }
}