using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.Service.Interfaces;

namespace AirportApp.Src.Service
{
    public class BookingService : IBookingService
    {
        private const string CancelledStatus = "Cancelled";
        private const string ActiveStatus = "Active";
        private const int FlightAndUserAndCountArgumentLength = 3;
        private const int FlightAndSecondArgumentLength = 2;
        private const int FlightArgumentIndex = 0;
        private const int SecondArgumentIndex = 1;
        private const int PassengerCountArgumentIndex = 2;
        private const int SeatsPerRow = 6;
        private const int AisleAfterColumn = 3;
        private const int DefaultInitialPassengerCount = 1;

        private readonly IFlightTicketRepository ticketRepository;
        private readonly IAddOnRepository addOnRepository;

        public BookingService(IFlightTicketRepository ticketRepository, IAddOnRepository addOnRepository)
        {
            this.ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
            this.addOnRepository = addOnRepository ?? throw new ArgumentNullException(nameof(addOnRepository));
        }

        public List<FlightTicket> CreateTickets(Flight flight, Customer user, List<PassengerData> passengers, float basePrice)
        {
            var tickets = new List<FlightTicket>();

            foreach (var passenger in passengers)
            {
                var ticket = new FlightTicket
                {
                    Flight = flight,
                    User = user,
                    PassengerFirstName = passenger.FirstName,
                    PassengerLastName = passenger.LastName,
                    PassengerEmail = passenger.Email,
                    PassengerPhone = passenger.Phone,
                    Seat = passenger.SelectedSeat,
                    Price = basePrice,
                    Status = ActiveStatus,
                    SelectedAddOns = passenger.SelectedAddOns.ToList()
                };
                tickets.Add(ticket);
            }

            return tickets;
        }

        public string ValidatePassengers(List<PassengerData> passengers)
        {
            if (passengers == null || passengers.Count == 0)
            {
                return "At least one passenger is required.";
            }

            for (int index = 0; index < passengers.Count; index++)
            {
                var passenger = passengers[index];
                int passengerNumber = index + 1;

                if (string.IsNullOrWhiteSpace(passenger.FirstName))
                {
                    return $"Passenger {passengerNumber}: first name is required.";
                }

                if (string.IsNullOrWhiteSpace(passenger.LastName))
                {
                    return $"Passenger {passengerNumber}: last name is required.";
                }

                if (!string.IsNullOrWhiteSpace(passenger.Email) && !ValidationHelper.IsValidEmail(passenger.Email))
                {
                    return $"Passenger {passengerNumber}: email format is invalid.";
                }

                if (string.IsNullOrWhiteSpace(passenger.SelectedSeat))
                {
                    return $"Passenger {passengerNumber}: please select a seat.";
                }
            }

            return string.Empty;
        }

        public int CalculateMaxPassengers(int routeCapacity, int occupiedSeatCount, int requestedPassengerCount)
        {
            int remainingCapacity = routeCapacity - occupiedSeatCount;

            if (requestedPassengerCount > 0)
            {
                return Math.Min(requestedPassengerCount, remainingCapacity);
            }

            return remainingCapacity;
        }

        public async Task<bool> SaveTicketsAsync(List<FlightTicket> tickets)
        {
            if (tickets == null || tickets.Count == 0)
            {
                return false;
            }

            bool duplicateSeatInRequest = tickets
                .Where(ticket => !string.IsNullOrWhiteSpace(ticket.Seat))
                .GroupBy(ticket => ticket.Seat)
                .Any(group => group.Count() > 1);

            if (duplicateSeatInRequest)
            {
                return false;
            }

            foreach (var ticket in tickets)
            {
                if (!string.IsNullOrWhiteSpace(ticket.Seat))
                {
                    bool seatAvailable = await this.ticketRepository.IsSeatAvailableAsync(ticket.Flight?.Id ?? 0, ticket.Seat);
                    if (!seatAvailable)
                    {
                        return false;
                    }
                }
            }

            return await this.ticketRepository.SaveTicketsWithAddOnsAsync(tickets);
        }

        public async Task<List<AddOn>> GetAvailableAddOnsAsync()
        {
            return (await this.addOnRepository.GetAllAddOnsAsync()).ToList();
        }

        public async Task<List<string>> GetOccupiedSeatsAsync(int flightId)
        {
            return (await this.ticketRepository.GetOccupiedSeatsAsync(flightId)).ToList();
        }

        public BookingParametersResult ParseBookingParameters(object parameter)
        {
            Flight? selectedFlight = null;
            Customer? user = null;
            int requestedPassengers = 0;

            if (parameter is object[] arguments && arguments.Length > 0)
            {
                selectedFlight = arguments[FlightArgumentIndex] as Flight;

                if (arguments.Length >= FlightAndUserAndCountArgumentLength)
                {
                    user = arguments[SecondArgumentIndex] as Customer;
                    if (arguments[PassengerCountArgumentIndex] is int count)
                    {
                        requestedPassengers = count;
                    }
                }
                else if (arguments.Length >= FlightAndSecondArgumentLength)
                {
                    if (arguments[SecondArgumentIndex] is int count)
                    {
                        requestedPassengers = count;
                    }
                    else
                    {
                        user = arguments[SecondArgumentIndex] as Customer;
                    }
                }
            }

            user ??= UserSession.CurrentUser;

            return new BookingParametersResult
            {
                Flight = selectedFlight!,
                User = user!,
                RequestedPassengers = requestedPassengers
            };
        }

        public void StorePendingBooking(Flight flight, int requestedPassengers)
        {
            UserSession.PendingBookingParameters = new object[] { flight, requestedPassengers };
        }

        public (List<SeatDescriptor> Layout, int RowCount) BuildSeatMapLayout(int capacity)
        {
            var layout = new List<SeatDescriptor>();
            char[] seatLetters = { 'A', 'B', 'C', 'D', 'E', 'F' };
            int rowCount = (capacity + SeatsPerRow - 1) / SeatsPerRow;

            for (int row = 0; row < rowCount; row++)
            {
                for (int seatIndex = 0; seatIndex < SeatsPerRow; seatIndex++)
                {
                    int column = seatIndex < AisleAfterColumn ? seatIndex : seatIndex + 1;
                    layout.Add(new SeatDescriptor
                    {
                        Row = row,
                        Column = column,
                        Label = $"{row + 1}{seatLetters[seatIndex]}"
                    });
                }
            }

            return (layout, rowCount);
        }

        public IList<string> ApplySeatSelection(IList<string> currentSeats, int targetPassengerIndex, string clickedSeat)
        {
            var updated = currentSeats.ToList();

            if (updated[targetPassengerIndex] == clickedSeat)
            {
                updated[targetPassengerIndex] = string.Empty;
            }
            else
            {
                for (int index = 0; index < updated.Count; index++)
                {
                    if (updated[index] == clickedSeat)
                    {
                        updated[index] = string.Empty;
                    }
                }

                updated[targetPassengerIndex] = clickedSeat;
            }

            return updated;
        }

        public void ApplyAddOnUpdates(IList<AddOn> currentAddOns, IEnumerable<AddOn> toAdd, IEnumerable<AddOn> toRemove)
        {
            foreach (var addOn in toAdd)
            {
                if (!currentAddOns.Contains(addOn))
                {
                    currentAddOns.Add(addOn);
                }
            }

            foreach (var addOn in toRemove)
            {
                currentAddOns.Remove(addOn);
            }
        }

        public int GetInitialPassengerCount(int maxPassengers, int requestedCount)
        {
            int initial = requestedCount > 0 ? requestedCount : DefaultInitialPassengerCount;
            return Math.Min(initial, maxPassengers);
        }

        public async Task<List<AddOn>> GetAddOnsByIdsAsync(List<int> ids)
        {
            return (await this.addOnRepository.GetAddOnsByIdsAsync(ids)).ToList();
        }
    }
}