using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.Src.Model.Ticket;

namespace AirportApp.Src.Repository.Interfaces
{
    public interface ITicketCategoryRepository
    {
        IEnumerable<TicketCategory> GetAll();

        TicketCategory GetById(int categoryId);
    }
}