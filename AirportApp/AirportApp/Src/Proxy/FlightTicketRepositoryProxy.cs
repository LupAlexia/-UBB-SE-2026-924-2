using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.Src.Proxy
{
    public class FlightTicketRepositoryProxy : IFlightTicketRepository
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/flightticket";

        public FlightTicketRepositoryProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<IEnumerable<FlightTicket>> GetTicketsByUserIdAsync(int userId)
        {
            try
            {
                var flightTicketTransferObjectList = await httpClient.GetFromJsonAsync<IEnumerable<AirportApp.ClassLibrary.Entity.Dto.FlightTicketDTO>>($"{BaseUrl}/user/{userId}");
                if (flightTicketTransferObjectList == null)
                {
                    return new List<FlightTicket>();
                }
                var tickets = new List<FlightTicket>();
                foreach (var flightTicketTransferObject in flightTicketTransferObjectList)
                {
                    var user = new Customer
                    {
                        Id = flightTicketTransferObject.userId
                    };

                    var flight = new Flight
                    {
                        Id = flightTicketTransferObject.flight?.id ?? flightTicketTransferObject.flightId,
                        Date = flightTicketTransferObject.flight?.date ?? default,
                        FlightNumber = flightTicketTransferObject.flight?.flightNumber ?? string.Empty,
                        Route = flightTicketTransferObject.flight?.route != null
                            ? new Route
                            {
                                Id = flightTicketTransferObject.flight.route.id,
                                RouteType = flightTicketTransferObject.flight.route.routeType,
                                DepartureTime = flightTicketTransferObject.flight.route.departureTime,
                                ArrivalTime = flightTicketTransferObject.flight.route.arrivalTime,
                                Capacity = flightTicketTransferObject.flight.route.capacity,
                                Airport = flightTicketTransferObject.flight.route.airport != null
                                    ? new Airport
                                    {
                                        Id = flightTicketTransferObject.flight.route.airport.id,
                                        AirportCode = flightTicketTransferObject.flight.route.airport.airportCode,
                                        City = flightTicketTransferObject.flight.route.airport.city
                                    }
                                    : null!,
                                Company = flightTicketTransferObject.flight.route.company != null
                                    ? new Company
                                    {
                                        Id = flightTicketTransferObject.flight.route.company.id,
                                        Name = flightTicketTransferObject.flight.route.company.name
                                    }
                                    : null!
                            }
                            : null!,
                        Gate = flightTicketTransferObject.flight != null
                            ? new Gate
                            {
                                Id = flightTicketTransferObject.flight.gateId
                            }
                            : null!
                    };

                    var ticket = new FlightTicket
                    {
                        Id = flightTicketTransferObject.id,
                        User = user,
                        Flight = flight,
                        Seat = flightTicketTransferObject.seat,
                        Price = flightTicketTransferObject.price,
                        Status = flightTicketTransferObject.status,
                        PassengerFirstName = flightTicketTransferObject.passengerFirstName,
                        PassengerLastName = flightTicketTransferObject.passengerLastName,
                        PassengerEmail = flightTicketTransferObject.passengerEmail,
                        PassengerPhone = flightTicketTransferObject.passengerPhone,
                        SelectedAddOns = flightTicketTransferObject.selectedAddOns?.Select(addOnObject => new AddOn(addOnObject.id, addOnObject.name, addOnObject.basePrice)).ToList() ?? new List<AddOn>(),
                    };
                    tickets.Add(ticket);
                }
                return tickets;
            }
            catch (HttpRequestException exception)
            {
                throw new InvalidOperationException($"Failed to retrieve tickets for user {userId}.", exception);
            }
        }

        public async Task AddTicketAsync(FlightTicket ticket)
        {
            try
            {
                var dataTransferObject = new AirportApp.ClassLibrary.Entity.Dto.FlightTicketDTO(
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
                    ticket.SelectedAddOns?.Select(addOnObject => new AirportApp.ClassLibrary.Entity.Dto.AddOnDTO(addOnObject.Id, addOnObject.Name, addOnObject.BasePrice)).ToList() ?? new List<AirportApp.ClassLibrary.Entity.Dto.AddOnDTO>());

                var response = await httpClient.PostAsJsonAsync(BaseUrl, dataTransferObject);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException("Failed to add ticket to server.", httpRequestException);
            }
        }

        public async Task UpdateTicketStatusAsync(int ticketId, string status)
        {
            try
            {
                var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{ticketId}/status", status);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException exception)
            {
                throw new InvalidOperationException($"Failed to update status for ticket {ticketId}.", exception);
            }
        }

        public async Task AddTicketAddOnsAsync(int ticketId, IEnumerable<int> addOnIds)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/{ticketId}/addons", addOnIds);
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
                return await httpClient.GetFromJsonAsync<IEnumerable<string>>($"{BaseUrl}/flight/{flightId}/occupied-seats")
                       ?? new List<string>();
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
                var result = await httpClient.GetFromJsonAsync<bool>($"{BaseUrl}/flight/{flightId}/seat-available/{Uri.EscapeDataString(seat)}");
                return result;
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to check seat availability for flight {flightId}.", httpRequestException);
            }
        }

        public async Task<bool> SaveTicketsWithAddOnsAsync(List<FlightTicket> tickets, List<List<int>> addOnIds = null)
        {
            try
            {
                var flightTicketTransferObjects = tickets.Select(ticket => new AirportApp.ClassLibrary.Entity.Dto.FlightTicketDTO(
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
                    ticket.SelectedAddOns?.Select(addOnEntity => new AirportApp.ClassLibrary.Entity.Dto.AddOnDTO(addOnEntity.Id, addOnEntity.Name, addOnEntity.BasePrice)).ToList() ?? new List<AirportApp.ClassLibrary.Entity.Dto.AddOnDTO>())).ToList();

                var request = new AirportApp.ClassLibrary.Entity.Dto.SaveTicketsRequestDTO
                {
                    Tickets = flightTicketTransferObjects,
                    AddOnIds = addOnIds ?? new List<List<int>>()
                };

                var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/batch", request);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<bool>();
                return result;
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException("Failed to save tickets with add-ons to server.", httpRequestException);
            }
        }
    }
}