using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interfaces;

namespace AirportApp.ClassLibrary.Proxy.ServiceProxies
{
    public class BookingServiceProxy : IBookingService
    {
        private readonly HttpClient httpClient;
        private const string AddOnBaseUrl = "api/addon";
        private const string FlightTicketBaseUrl = "api/flightticket";

        public BookingServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
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
                    Status = "Active",
                    SelectedAddOns = passenger.SelectedAddOns != null ? new List<AddOn>(passenger.SelectedAddOns) : new List<AddOn>()
                };
                tickets.Add(ticket);
            }

            return tickets;
        }

        public async Task<bool> SaveTicketsAsync(List<FlightTicket> tickets)
        {
            try
            {
                var ticketTransferObjectList = new List<FlightTicketDTO>();
                foreach (FlightTicket ticket in tickets)
                {
                    var addOnTransferObjectList = new List<AddOnDTO>();
                    if (ticket.SelectedAddOns != null)
                    {
                        foreach (AddOn addOn in ticket.SelectedAddOns)
                        {
                            addOnTransferObjectList.Add(new AddOnDTO(addOn.Id, addOn.Name, addOn.BasePrice));
                        }
                    }

                    ticketTransferObjectList.Add(new FlightTicketDTO(
                        ticket.Id,
                        ticket.User.Id,
                        ticket.Flight.Id,
                        ticket.Seat,
                        ticket.Price,
                        ticket.Status,
                        ticket.PassengerFirstName,
                        ticket.PassengerLastName,
                        ticket.PassengerEmail,
                        ticket.PassengerPhone,
                        addOnTransferObjectList));
                }

                var requestBody = new SaveTicketsRequestDTO
                {
                    Tickets = ticketTransferObjectList
                };

                HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{FlightTicketBaseUrl}/batch", requestBody);
                response.EnsureSuccessStatusCode();

                bool isSuccess = await response.Content.ReadFromJsonAsync<bool>();
                return isSuccess;
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException("Failed to save tickets through the service proxy.", httpRequestException);
            }
        }

        public async Task<List<AddOn>> GetAvailableAddOnsAsync()
        {
            try
            {
                List<AddOnDTO> addOnTransferObjectList = await httpClient.GetFromJsonAsync<List<AddOnDTO>>(AddOnBaseUrl);
                if (addOnTransferObjectList == null)
                {
                    return new List<AddOn>();
                }

                var addOns = new List<AddOn>();
                foreach (AddOnDTO addOnTransferObject in addOnTransferObjectList)
                {
                    addOns.Add(new AddOn
                    {
                        Id = addOnTransferObject.id,
                        Name = addOnTransferObject.name,
                        BasePrice = addOnTransferObject.basePrice
                    });
                }

                return addOns;
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException("Failed to retrieve available add-ons.", httpRequestException);
            }
        }

        public async Task<List<AddOn>> GetAddOnsByIdsAsync(List<int> ids)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{AddOnBaseUrl}/by-ids", ids);
                response.EnsureSuccessStatusCode();

                List<AddOnDTO> addOnTransferObjectList = await response.Content.ReadFromJsonAsync<List<AddOnDTO>>();
                if (addOnTransferObjectList == null)
                {
                    return new List<AddOn>();
                }

                var addOns = new List<AddOn>();
                foreach (AddOnDTO addOnTransferObject in addOnTransferObjectList)
                {
                    addOns.Add(new AddOn
                    {
                        Id = addOnTransferObject.id,
                        Name = addOnTransferObject.name,
                        BasePrice = addOnTransferObject.basePrice
                    });
                }

                return addOns;
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException("Failed to retrieve add-ons by IDs.", httpRequestException);
            }
        }

        public async Task<List<string>> GetOccupiedSeatsAsync(int flightId)
        {
            try
            {
                List<string> seats = await httpClient.GetFromJsonAsync<List<string>>($"{FlightTicketBaseUrl}/flight/{flightId}/occupied-seats");
                return seats ?? new List<string>();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to retrieve occupied seats for flight {flightId}.", httpRequestException);
            }
        }

        public async Task<string> ValidatePassengersAsync(List<PassengerData> passengers)
        {
            try
            {
                var dtos = passengers.Select(p => new PassengerDataDTO
                {
                    FirstName = p.FirstName ?? string.Empty,
                    LastName = p.LastName ?? string.Empty,
                    Email = p.Email ?? string.Empty,
                    Phone = p.Phone ?? string.Empty,
                    SelectedSeat = p.SelectedSeat ?? string.Empty,
                    SelectedAddOns = p.SelectedAddOns?.Select(a => new PricingAddOnDTO { Id = a.Id, BasePrice = a.BasePrice }).ToList() ?? new List<PricingAddOnDTO>()
                }).ToList();

                HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/booking/validate-passengers", dtos);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException("Failed to validate passengers.", ex);
            }
        }

        public async Task<int> CalculateMaxPassengersAsync(int routeCapacity, int occupiedSeatCount, int requestedPassengerCount)
        {
            try
            {
                return await httpClient.GetFromJsonAsync<int>($"api/booking/calculate-max-passengers?routeCapacity={routeCapacity}&occupiedSeatCount={occupiedSeatCount}&requestedPassengerCount={requestedPassengerCount}");
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException("Failed to calculate max passengers.", ex);
            }
        }

        public BookingParametersResult ParseBookingParameters(object parameter)
        {
            Flight? selectedFlight = null;
            Customer? user = null;
            int requestedPassengers = 0;

            if (parameter is object[] arguments && arguments.Length > 0)
            {
                selectedFlight = arguments[0] as Flight;

                if (arguments.Length >= 3)
                {
                    user = arguments[1] as Customer;
                    if (arguments[2] is int count)
                    {
                        requestedPassengers = count;
                    }
                }
                else if (arguments.Length >= 2)
                {
                    if (arguments[1] is int count)
                    {
                        requestedPassengers = count;
                    }
                    else
                    {
                        user = arguments[1] as Customer;
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

        public async Task<(List<SeatDescriptor> Layout, int RowCount)> BuildSeatMapLayoutAsync(int capacity)
        {
            try
            {
                var response = await httpClient.GetFromJsonAsync<SeatMapResponseDTO>($"api/booking/build-seat-map?capacity={capacity}");
                if (response != null)
                {
                    return (response.Layout, response.RowCount);
                }
                return (new List<SeatDescriptor>(), 0);
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException("Failed to build seat map layout.", ex);
            }
        }

        public IList<string> ApplySeatSelection(IList<string> currentSeats, int targetPassengerIndex, string clickedSeat)
        {
            var updated = new List<string>(currentSeats);

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

        public async Task<int> GetInitialPassengerCountAsync(int maxPassengers, int requestedCount)
        {
            try
            {
                return await httpClient.GetFromJsonAsync<int>($"api/booking/initial-passenger-count?maxPassengers={maxPassengers}&requestedCount={requestedCount}");
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException("Failed to get initial passenger count.", ex);
            }
        }
    }
}
