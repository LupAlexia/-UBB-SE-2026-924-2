using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;

namespace AirportApp.ClassLibrary.Repository.Interfaces
{
    public interface ITicketRepository : IRepository<int, Ticket>
    {
    }
}