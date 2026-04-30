using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.Src.Domain;

namespace AirportApp.Src.Repository
{
    public interface IFlightTicketRepository
    {
        IEnumerable<FlightTicket> GetTicketsByUserId(int userId);
        void AddTicket(FlightTicket ticket);
        void UpdateTicketStatus(int ticketId, string status);
        void AddTicketAddOns(int ticketId, IEnumerable<int> addOnIds);
        IEnumerable<string> GetOccupiedSeats(int flightId);
        Task<bool> SaveTicketsWithAddOnsAsync(List<FlightTicket> tickets);
        Task<bool> IsSeatAvailable(int flightId, string seat);
    }
}
