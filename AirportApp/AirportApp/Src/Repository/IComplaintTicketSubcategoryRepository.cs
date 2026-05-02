using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.Src.Model.Ticket;

namespace AirportApp.Src.Repository
{
    public interface IComplaintTicketSubcategoryRepository
    {
        IEnumerable<ComplaintTicketSubcategory> GetAll();

        ComplaintTicketSubcategory GetById(int subcategoryId);

        IEnumerable<ComplaintTicketSubcategory> GetByCategoryId(int categoryId);
    }
}