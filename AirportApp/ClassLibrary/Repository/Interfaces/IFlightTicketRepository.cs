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


}
