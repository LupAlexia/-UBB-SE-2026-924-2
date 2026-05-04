using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Src.Service
{
    public interface IDashboardService
    {
        Task<IEnumerable<FlightTicket>> GetUserTicketsAsync(int userId, string ticketFilter);
        string GenerateTicketPdf(FlightTicket ticket);
    }
}
