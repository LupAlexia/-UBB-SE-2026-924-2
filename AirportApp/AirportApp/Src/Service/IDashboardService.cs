using System.Collections.Generic;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Src.Service
{
    public interface IDashboardService
    {
        IEnumerable<FlightTicket> GetUserTickets(int userId, string ticketFilter);
        string GenerateTicketPdf(FlightTicket ticket);
    }
}
