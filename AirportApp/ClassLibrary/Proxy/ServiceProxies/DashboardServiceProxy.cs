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
    public class DashboardServiceProxy : IDashboardService
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/flightticket";

        public DashboardServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<IEnumerable<FlightTicket>> GetUserTicketsAsync(int userId, string ticketFilter)
        {
            try
            {
                List<FlightTicketDTO> ticketTransferObjectList = await httpClient.GetFromJsonAsync<List<FlightTicketDTO>>($"{BaseUrl}/user/{userId}/filter?filter={Uri.EscapeDataString(ticketFilter)}");
                if (ticketTransferObjectList == null)
                {
                    return new List<FlightTicket>();
                }

                var tickets = new List<FlightTicket>();
                foreach (FlightTicketDTO ticketTransferObject in ticketTransferObjectList)
                {
                    tickets.Add(MapFlightTicketFromTransferObject(ticketTransferObject));
                }

                return tickets;
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to retrieve filtered tickets for user {userId}.", httpRequestException);
            }
        }

        public async Task<IEnumerable<FlightTicket>> GetTicketsByUserIdAsync(int userId)
        {
            try
            {
                List<FlightTicketDTO> ticketTransferObjectList = await httpClient.GetFromJsonAsync<List<FlightTicketDTO>>($"{BaseUrl}/user/{userId}");
                if (ticketTransferObjectList == null)
                {
                    return new List<FlightTicket>();
                }

                var tickets = new List<FlightTicket>();
                foreach (FlightTicketDTO ticketTransferObject in ticketTransferObjectList)
                {
                    tickets.Add(MapFlightTicketFromTransferObject(ticketTransferObject));
                }

                return tickets;
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to retrieve tickets for user {userId}.", httpRequestException);
            }
        }

        public string GenerateTicketPdf(FlightTicket ticket)
        {
            throw new NotSupportedException("GenerateTicketPdf is not available through the service proxy.");
        }

        public async Task AddTicketAsync(FlightTicket ticket)
        {
            try
            {
                var addOnTransferObjectList = new List<AddOnDTO>();
                if (ticket.SelectedAddOns != null)
                {
                    foreach (AddOn addOn in ticket.SelectedAddOns)
                    {
                        addOnTransferObjectList.Add(new AddOnDTO(addOn.Id, addOn.Name, addOn.BasePrice));
                    }
                }

                var ticketTransferObject = new FlightTicketDTO(
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
                    addOnTransferObjectList);

                HttpResponseMessage response = await httpClient.PostAsJsonAsync(BaseUrl, ticketTransferObject);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException("Failed to add flight ticket.", httpRequestException);
            }
        }

        public async Task UpdateTicketStatusAsync(int ticketId, string status)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{ticketId}/status", status);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to update status for ticket {ticketId}.", httpRequestException);
            }
        }

        public async Task AddTicketAddOnsAsync(int ticketId, IEnumerable<int> addOnIds)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{BaseUrl}/{ticketId}/addons", addOnIds);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to add add-ons to ticket {ticketId}.", httpRequestException);
            }
        }

        public async Task<IEnumerable<string>> GetOccupiedSeatsAsync(int flightId)
        {
            try
            {
                IEnumerable<string> seats = await httpClient.GetFromJsonAsync<IEnumerable<string>>($"{BaseUrl}/flight/{flightId}/occupied-seats");
                return seats ?? new List<string>();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to retrieve occupied seats for flight {flightId}.", httpRequestException);
            }
        }

        public async Task<bool> IsSeatAvailableAsync(int flightId, string seat)
        {
            try
            {
                bool isAvailable = await httpClient.GetFromJsonAsync<bool>($"{BaseUrl}/flight/{flightId}/seat-available/{Uri.EscapeDataString(seat)}");
                return isAvailable;
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to check seat availability for flight {flightId}.", httpRequestException);
            }
        }

        public async Task<bool> SaveTicketsWithAddOnsAsync(List<FlightTicket> tickets, List<List<int>> addOnIds)
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
                    Tickets = ticketTransferObjectList,
                    AddOnIds = addOnIds
                };

                HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{BaseUrl}/batch", requestBody);
                response.EnsureSuccessStatusCode();

                bool isSuccess = await response.Content.ReadFromJsonAsync<bool>();
                return isSuccess;
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException("Failed to save tickets with add-ons.", httpRequestException);
            }
        }

        private static FlightTicket MapFlightTicketFromTransferObject(FlightTicketDTO ticketTransferObject)
        {
            var ticket = new FlightTicket
            {
                Id = ticketTransferObject.id,
                User = new Customer { Id = ticketTransferObject.userId },
                Seat = ticketTransferObject.seat,
                Price = ticketTransferObject.price,
                Status = ticketTransferObject.status,
                PassengerFirstName = ticketTransferObject.passengerFirstName,
                PassengerLastName = ticketTransferObject.passengerLastName,
                PassengerEmail = ticketTransferObject.passengerEmail,
                PassengerPhone = ticketTransferObject.passengerPhone
            };

            if (ticketTransferObject.flight != null)
            {
                ticket.Flight = new Flight
                {
                    Id = ticketTransferObject.flight.id,
                    Gate = new Gate { Id = ticketTransferObject.flight.gateId },
                    Date = ticketTransferObject.flight.date,
                    FlightNumber = ticketTransferObject.flight.flightNumber,
                    Route = ticketTransferObject.flight.route != null
                        ? new Route
                        {
                            Id = ticketTransferObject.flight.route.id,
                            RouteType = ticketTransferObject.flight.route.routeType,
                            DepartureTime = ticketTransferObject.flight.route.departureTime,
                            ArrivalTime = ticketTransferObject.flight.route.arrivalTime,
                            Capacity = ticketTransferObject.flight.route.capacity,
                            Airport = ticketTransferObject.flight.route.airport != null
                                ? new Airport
                                {
                                    Id = ticketTransferObject.flight.route.airport.id,
                                    AirportCode = ticketTransferObject.flight.route.airport.airportCode,
                                    City = ticketTransferObject.flight.route.airport.city
                                }
                                : null!,
                            Company = ticketTransferObject.flight.route.company != null
                                ? new Company
                                {
                                    Id = ticketTransferObject.flight.route.company.id,
                                    Name = ticketTransferObject.flight.route.company.name
                                }
                                : null!
                        }
                        : null
                };
            }
            else
            {
                ticket.Flight = new Flight { Id = ticketTransferObject.flightId };
            }

            if (ticketTransferObject.selectedAddOns != null)
            {
                ticket.SelectedAddOns = new List<AddOn>();
                foreach (AddOnDTO addOnTransferObject in ticketTransferObject.selectedAddOns)
                {
                    ticket.SelectedAddOns.Add(new AddOn
                    {
                        Id = addOnTransferObject.id,
                        Name = addOnTransferObject.name,
                        BasePrice = addOnTransferObject.basePrice
                    });
                }
            }

            return ticket;
        }
    }
}
