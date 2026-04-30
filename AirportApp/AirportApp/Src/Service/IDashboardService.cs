using System.Collections.Generic;
using AirportApp.Src.Domain;

namespace AirportApp.Src.Service
{
    public interface IDashboardService
    {
        IEnumerable<FlightTicket> GetUserTickets(int userId, string ticketFilter);
        string GenerateTicketPdf(FlightTicket ticket);
    }
}
