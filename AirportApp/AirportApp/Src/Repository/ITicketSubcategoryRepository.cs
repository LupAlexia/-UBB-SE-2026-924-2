using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.Src.Model.Ticket;

namespace AirportApp.Src.Repository
{
    public interface ITicketSubcategoryRepository
    {
        IEnumerable<TicketSubcategory> GetAll();

        TicketSubcategory GetById(int subcategoryId);

        IEnumerable<TicketSubcategory> GetByCategoryId(int categoryId);
    }
}