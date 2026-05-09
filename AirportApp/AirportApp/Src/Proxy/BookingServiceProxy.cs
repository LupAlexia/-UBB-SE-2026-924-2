using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service.Interfaces;

namespace AirportApp.Src.Proxy
{
    public class BookingServiceProxy : IBookingService
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
        private const string FlightTicketBaseUrl = "api/flightticket";
        private const string AddOnBaseUrl = "api/addon";

        private readonly HttpClient httpClient;

        public BookingServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        // Pure logic — builds ticket objects in memory, no DB access
        public List<FlightTicket> CreateTickets(
            Flight flight, Customer user, List<PassengerData> passengers, float basePrice)
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

        // Pure logic — input validation, no DB access
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

                if (!string.IsNullOrWhiteSpace(passenger.Email)
                    && !ValidationHelper.IsValidEmail(passenger.Email))
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

        // Pure logic — math calculation, no DB access
        public int CalculateMaxPassengers(
            int routeCapacity, int occupiedSeatCount, int requestedPassengerCount)
        {
            int remainingCapacity = routeCapacity - occupiedSeatCount;

            if (requestedPassengerCount > 0)
            {
                return Math.Min(requestedPassengerCount, remainingCapacity);
            }

            return remainingCapacity;
        }

        // HTTP calls replace: ticketRepository.IsSeatAvailableAsync + ticketRepository.SaveTicketsWithAddOnsAsync
        public async Task<bool> SaveTicketsAsync(List<FlightTicket> tickets)
        {
            if (tickets == null || tickets.Count == 0)
            {
                return false;
            }

            // Pure logic — duplicate seat check stays local
            bool hasDuplicateSeatInRequest = tickets
                .Where(ticket => !string.IsNullOrWhiteSpace(ticket.Seat))
                .GroupBy(ticket => ticket.Seat)
                .Any(group => group.Count() > 1);

            if (hasDuplicateSeatInRequest)
            {
                return false;
            }

            // Extract add-on IDs BEFORE clearing the tickets
            var addOnIds = tickets.Select(t =>
                t.SelectedAddOns?.Select(a => a.Id).ToList() ?? new List<int>()).ToList();

            // Prepare clean versions of tickets for the API (with EMPTY add-ons)
            var preparedTickets = tickets.Select(t => new FlightTicket
            {
                User = t.User,
                Flight = t.Flight,
                Seat = t.Seat,
                Price = t.Price,
                Status = t.Status,
                PassengerFirstName = t.PassengerFirstName,
                PassengerLastName = t.PassengerLastName,
                PassengerEmail = t.PassengerEmail,
                PassengerPhone = t.PassengerPhone,
            }).ToList();

            // Send request with both tickets and add-on IDs
            var request = new SaveTicketsRequest
            {
                Tickets = preparedTickets,
                AddOnIds = addOnIds
            };

            HttpResponseMessage response = await this.httpClient
                .PostAsJsonAsync($"{FlightTicketBaseUrl}/batch", request);

            return response.IsSuccessStatusCode;
        }

        // HTTP call replaces: addOnRepository.GetAllAddOnsAsync
        public async Task<List<AddOn>> GetAvailableAddOnsAsync()
        {
            var addOns = await this.httpClient
                .GetFromJsonAsync<List<AddOn>>(AddOnBaseUrl);

            return addOns ?? new List<AddOn>();
        }

        // HTTP call replaces: ticketRepository.GetOccupiedSeatsAsync
        public async Task<List<string>> GetOccupiedSeatsAsync(int flightId)
        {
            var seats = await this.httpClient
                .GetFromJsonAsync<List<string>>(
                    $"{FlightTicketBaseUrl}/flight/{flightId}/occupied-seats");

            return seats ?? new List<string>();
        }

        // Pure logic — parameter parsing, no DB access
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

        // Pure logic — sets UserSession, no DB access
        public void StorePendingBooking(Flight flight, int requestedPassengers)
        {
            UserSession.PendingBookingParameters = new object[] { flight, requestedPassengers };
        }

        // Pure logic — seat grid math, no DB access
        public (List<SeatDescriptor> Layout, int RowCount) BuildSeatMapLayout(int capacity)
        {
            var layout = new List<SeatDescriptor>();
            char[] seatLetters = { 'A', 'B', 'C', 'D', 'E', 'F' };
            int rowCount = (capacity + SeatsPerRow - 1) / SeatsPerRow;

            for (int row = 0; row < rowCount; row++)
            {
                for (int seatIndex = 0; seatIndex < SeatsPerRow; seatIndex++)
                {
                    int column = seatIndex < AisleAfterColumn
                        ? seatIndex
                        : seatIndex + 1;

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

        // Pure logic — list manipulation, no DB access
        public IList<string> ApplySeatSelection(
            IList<string> currentSeats, int targetPassengerIndex, string clickedSeat)
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

        // Pure logic — list manipulation, no DB access
        public void ApplyAddOnUpdates(
            IList<AddOn> currentAddOns, IEnumerable<AddOn> toAdd, IEnumerable<AddOn> toRemove)
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

        // Pure logic — math calculation, no DB access
        public int GetInitialPassengerCount(int maxPassengers, int requestedCount)
        {
            int initial = requestedCount > 0 ? requestedCount : DefaultInitialPassengerCount;
            return Math.Min(initial, maxPassengers);
        }
    }
}
