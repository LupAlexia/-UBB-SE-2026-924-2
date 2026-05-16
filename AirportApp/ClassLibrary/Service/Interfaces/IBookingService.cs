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
        string ValidatePassengers(List<PassengerData> passengers);
        int CalculateMaxPassengers(int routeCapacity, int occupiedSeatCount, int requestedPassengerCount);
        BookingParametersResult ParseBookingParameters(object parameter);
        void StorePendingBooking(Flight flight, int requestedPassengers);
        (List<SeatDescriptor> Layout, int RowCount) BuildSeatMapLayout(int capacity);
        IList<string> ApplySeatSelection(IList<string> currentSeats, int targetPassengerIndex, string clickedSeat);
        void ApplyAddOnUpdates(IList<AddOn> currentAddOns, IEnumerable<AddOn> toAdd, IEnumerable<AddOn> toRemove);
        int GetInitialPassengerCount(int maxPassengers, int requestedCount);
    }
}