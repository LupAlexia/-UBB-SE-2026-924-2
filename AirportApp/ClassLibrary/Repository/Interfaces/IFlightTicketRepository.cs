using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interfaces
{
    public interface IFlightTicketRepository
    {
        Task<IEnumerable<FlightTicket>> GetTicketsByUserIdAsync(int userId);

        Task AddTicketAsync(FlightTicket ticket);

        Task UpdateTicketStatusAsync(int ticketId, string status);

        Task AddTicketAddOnsAsync(int ticketId, IEnumerable<int> addOnIds);

        Task<IEnumerable<string>> GetOccupiedSeatsAsync(int flightId);

        Task<bool> SaveTicketsWithAddOnsAsync(List<FlightTicket> tickets, List<List<int>> addOnIds = null);

        Task<bool> IsSeatAvailableAsync(int flightId, string seat);
    }

    // DTO to send tickets and add-on IDs to API
    public class SaveTicketsRequest
    {
        public List<FlightTicket> Tickets { get; set; } = new List<FlightTicket>();
        public List<List<int>> AddOnIds { get; set; } = new List<List<int>>();
    }
}
