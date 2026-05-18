using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interfaces
{
    public interface IBookingService
    {
        List<FlightTicket> CreateTickets(Flight flight, Customer user, List<PassengerData> passengers, float basePrice);
        Task<bool> SaveTicketsAsync(List<FlightTicket> tickets);
        Task<List<AddOn>> GetAvailableAddOnsAsync();
        Task<List<AddOn>> GetAddOnsByIdsAsync(List<int> ids);
        Task<List<string>> GetOccupiedSeatsAsync(int flightId);
        Task<string> ValidatePassengersAsync(List<PassengerData> passengers);
        Task<int> CalculateMaxPassengersAsync(int routeCapacity, int occupiedSeatCount, int requestedPassengerCount);
        BookingParametersResult ParseBookingParameters(object parameter);
        void StorePendingBooking(Flight flight, int requestedPassengers);
        Task<(List<SeatDescriptor> Layout, int RowCount)> BuildSeatMapLayoutAsync(int capacity);
        IList<string> ApplySeatSelection(IList<string> currentSeats, int targetPassengerIndex, string clickedSeat);
        void ApplyAddOnUpdates(IList<AddOn> currentAddOns, IEnumerable<AddOn> toAdd, IEnumerable<AddOn> toRemove);
        Task<int> GetInitialPassengerCountAsync(int maxPassengers, int requestedCount);
    }
}