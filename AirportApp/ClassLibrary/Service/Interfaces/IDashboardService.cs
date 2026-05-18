using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interfaces
{
    public interface IDashboardService
    {
        Task<IEnumerable<FlightTicket>> GetUserTicketsAsync(int userId, string ticketFilter);
        Task<IEnumerable<FlightTicket>> GetTicketsByUserIdAsync(int userId);
        string GenerateTicketPdf(FlightTicket ticket);
        Task AddTicketAsync(FlightTicket ticket);
        Task UpdateTicketStatusAsync(int ticketId, string status);
        Task AddTicketAddOnsAsync(int ticketId, IEnumerable<int> addOnIds);
        Task<IEnumerable<string>> GetOccupiedSeatsAsync(int flightId);
        Task<bool> IsSeatAvailableAsync(int flightId, string seat);
        Task<bool> SaveTicketsWithAddOnsAsync(List<FlightTicket> tickets, List<List<int>> addOnIds);
    }
}