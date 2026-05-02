using System.Collections.Generic;
using AirportApp.Src.Model.Ticket;

namespace AirportApp.Src.Service.Interfaces
{
    public interface IComplaintTicketCategoryService
    {
        ComplaintTicketCategory GetCategoryById(int categoryId);

        IEnumerable<ComplaintTicketCategory> GetAllCategories();
    }
}