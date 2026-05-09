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
                var dtos = await httpClient.GetFromJsonAsync<IEnumerable<AirportApp.ClassLibrary.Entity.Dto.FlightTicketDTO>>($"{BaseUrl}/user/{userId}");
                if (dtos == null)
                {
                    return new List<FlightTicket>();
                }
                var tickets = new List<FlightTicket>();
                foreach (var dto in dtos)
                {
                    var ticket = new FlightTicket
                    {
                        Id = dto.id,
                        UserId = dto.userId,
                        FlightId = dto.flightId,
                        Seat = dto.seat,
                        Price = dto.price,
                        Status = dto.status,
                        PassengerFirstName = dto.passengerFirstName,
                        PassengerLastName = dto.passengerLastName,
                        PassengerEmail = dto.passengerEmail,
                        PassengerPhone = dto.passengerPhone,
                        SelectedAddOns = dto.selectedAddOns?.Select(a => new AddOn(a.id, a.name, a.basePrice)).ToList() ?? new List<AddOn>(),
                        Flight = dto.flight != null ? new Flight
                        {
                            Id = dto.flight.id,
                            RouteId = dto.flight.routeId,
                            GateId = dto.flight.gateId,
                            Date = dto.flight.date,
                            FlightNumber = dto.flight.flightNumber,
                            Route = dto.flight.route != null ? new Route
                            {
                                Id = dto.flight.route.id,
                                RouteType = dto.flight.route.routeType,
                                DepartureTime = dto.flight.route.departureTime,
                                ArrivalTime = dto.flight.route.arrivalTime,
                                Capacity = dto.flight.route.capacity,
                                Airport = dto.flight.route.airport != null ? new Airport { Id = dto.flight.route.airport.id, AirportCode = dto.flight.route.airport.airportCode, City = dto.flight.route.airport.city } : null!,
                                Company = dto.flight.route.company != null ? new Company { Id = dto.flight.route.company.id, Name = dto.flight.route.company.name } : null!
                            }
                            : null!
                        }
                        : null
                    };
                    tickets.Add(ticket);
                }
                return tickets;
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to retrieve tickets for user {userId}.", ex);
            }
        }

        public async Task AddTicketAsync(FlightTicket ticket)
        {
            try
            {
                var dto = new AirportApp.ClassLibrary.Entity.Dto.FlightTicketDTO(
                    ticket.Id,
                    ticket.UserId,
                    ticket.FlightId,
                    ticket.Seat,
                    ticket.Price,
                    ticket.Status,
                    ticket.PassengerFirstName,
                    ticket.PassengerLastName,
                    ticket.PassengerEmail,
                    ticket.PassengerPhone,
                    ticket.SelectedAddOns?.Select(a => new AirportApp.ClassLibrary.Entity.Dto.AddOnDTO(a.Id, a.Name, a.BasePrice)).ToList() ?? new List<AirportApp.ClassLibrary.Entity.Dto.AddOnDTO>());

                var response = await httpClient.PostAsJsonAsync(BaseUrl, dto);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException("Failed to add ticket to server.", ex);
            }
        }

        public async Task UpdateTicketStatusAsync(int ticketId, string status)
        {
            try
            {
                var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{ticketId}/status", status);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to update status for ticket {ticketId}.", ex);
            }
        }

        public async Task AddTicketAddOnsAsync(int ticketId, IEnumerable<int> addOnIds)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/{ticketId}/addons", addOnIds);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to add add-ons to ticket {ticketId}.", ex);
            }
        }

        public async Task<IEnumerable<string>> GetOccupiedSeatsAsync(int flightId)
        {
            try
            {
                return await httpClient.GetFromJsonAsync<IEnumerable<string>>($"{BaseUrl}/flight/{flightId}/occupied-seats")
                       ?? new List<string>();
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to retrieve occupied seats for flight {flightId}.", ex);
            }
        }

        public async Task<bool> IsSeatAvailableAsync(int flightId, string seat)
        {
            try
            {
                var result = await httpClient.GetFromJsonAsync<bool>($"{BaseUrl}/flight/{flightId}/seat-available/{Uri.EscapeDataString(seat)}");
                return result;
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to check seat availability for flight {flightId}.", ex);
            }
        }

        public async Task<bool> SaveTicketsWithAddOnsAsync(List<FlightTicket> tickets, List<List<int>> addOnIds = null)
        {
            try
            {
                var ticketDtos = tickets.Select(ticket => new AirportApp.ClassLibrary.Entity.Dto.FlightTicketDTO(
                    ticket.Id,
                    ticket.UserId,
                    ticket.FlightId,
                    ticket.Seat,
                    ticket.Price,
                    ticket.Status,
                    ticket.PassengerFirstName,
                    ticket.PassengerLastName,
                    ticket.PassengerEmail,
                    ticket.PassengerPhone,
                    ticket.SelectedAddOns?.Select(a => new AirportApp.ClassLibrary.Entity.Dto.AddOnDTO(a.Id, a.Name, a.BasePrice)).ToList() ?? new List<AirportApp.ClassLibrary.Entity.Dto.AddOnDTO>())).ToList();

                var request = new AirportApp.ClassLibrary.Entity.Dto.SaveTicketsRequestDTO
                {
                    Tickets = ticketDtos,
                    AddOnIds = addOnIds ?? new List<List<int>>()
                };

                var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/batch", request);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<bool>();
                return result;
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException("Failed to save tickets with add-ons to server.", ex);
            }
        }
    }
}