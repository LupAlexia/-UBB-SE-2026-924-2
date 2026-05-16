using System;
using System.Collections.Generic;
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
            throw new NotSupportedException("CreateTickets is not available through the service proxy.");
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

        public string ValidatePassengers(List<PassengerData> passengers)
        {
            throw new NotSupportedException("ValidatePassengers is not available through the service proxy.");
        }

        public int CalculateMaxPassengers(int routeCapacity, int occupiedSeatCount, int requestedPassengerCount)
        {
            throw new NotSupportedException("CalculateMaxPassengers is not available through the service proxy.");
        }

        public BookingParametersResult ParseBookingParameters(object parameter)
        {
            throw new NotSupportedException("ParseBookingParameters is not available through the service proxy.");
        }

        public void StorePendingBooking(Flight flight, int requestedPassengers)
        {
            throw new NotSupportedException("StorePendingBooking is not available through the service proxy.");
        }

        public (List<SeatDescriptor> Layout, int RowCount) BuildSeatMapLayout(int capacity)
        {
            throw new NotSupportedException("BuildSeatMapLayout is not available through the service proxy.");
        }

        public IList<string> ApplySeatSelection(IList<string> currentSeats, int targetPassengerIndex, string clickedSeat)
        {
            throw new NotSupportedException("ApplySeatSelection is not available through the service proxy.");
        }

        public void ApplyAddOnUpdates(IList<AddOn> currentAddOns, IEnumerable<AddOn> toAdd, IEnumerable<AddOn> toRemove)
        {
            throw new NotSupportedException("ApplyAddOnUpdates is not available through the service proxy.");
        }

        public int GetInitialPassengerCount(int maxPassengers, int requestedCount)
        {
            throw new NotSupportedException("GetInitialPassengerCount is not available through the service proxy.");
        }
    }
}
