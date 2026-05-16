using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Airport.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightTicketController : ControllerBase
    {
        private readonly IDashboardService dashboardService;
        private readonly IAuthService authService;
        private readonly IFlightSearchService flightSearchService;

        public FlightTicketController(IDashboardService dashboardService, IAuthService authService, IFlightSearchService flightSearchService)
        {
            this.dashboardService = dashboardService;
            this.authService = authService;
            this.flightSearchService = flightSearchService;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<FlightTicketDTO>>> GetByUserIdAsync(int userId)
        {
            IEnumerable<FlightTicket> tickets = await dashboardService.GetTicketsByUserIdAsync(userId);
            var flightTicketTransferObjectList = new List<FlightTicketDTO>();
            foreach (var ticket in tickets)
            {
                flightTicketTransferObjectList.Add(new FlightTicketDTO(
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
                    ticket.SelectedAddOns?.Select(addOn => new AddOnDTO(addOn.Id, addOn.Name, addOn.BasePrice)).ToList() ?? new List<AddOnDTO>(),
                    ticket.Flight != null ? new FlightDTO(
                        ticket.Flight.Id,
                        ticket.Flight.Route.Id,
                        ticket.Flight.Gate.Id,
                        ticket.Flight.Date,
                        ticket.Flight.FlightNumber,
                        ticket.Flight.Route != null ? new RouteDTO(
                            ticket.Flight.Route.Id,
                            ticket.Flight.Route.RouteType,
                            ticket.Flight.Route.DepartureTime,
                            ticket.Flight.Route.ArrivalTime,
                            ticket.Flight.Route.Capacity,
                            ticket.Flight.Route.Airport != null ? new AirportDTO(ticket.Flight.Route.Airport.Id, ticket.Flight.Route.Airport.AirportCode, ticket.Flight.Route.Airport.City) : null,
                            ticket.Flight.Route.Company != null ? new CompanyDTO(ticket.Flight.Route.Company.Id, ticket.Flight.Route.Company.Name) : null) : null) : null));
            }

            return Ok(flightTicketTransferObjectList);
        }

        [HttpPost]
        public async Task<ActionResult> AddTicketAsync([FromBody] FlightTicketDTO flightTicketData)
        {
            var ticket = new FlightTicket
            {
                Id = flightTicketData.id,
                User = await authService.GetByIdAsync(flightTicketData.userId),
                Flight = await flightSearchService.GetFlightByIdAsync(flightTicketData.flightId),
                Seat = flightTicketData.seat,
                Price = flightTicketData.price,
                Status = flightTicketData.status,
                PassengerFirstName = flightTicketData.passengerFirstName,
                PassengerLastName = flightTicketData.passengerLastName,
                PassengerEmail = flightTicketData.passengerEmail,
                PassengerPhone = flightTicketData.passengerPhone
            };

            await dashboardService.AddTicketAsync(ticket);
            return Ok(flightTicketData);
        }

        [HttpPut("{ticketId}/status")]
        public async Task<ActionResult> UpdateTicketStatusAsync(int ticketId, [FromBody] string status)
        {
            await dashboardService.UpdateTicketStatusAsync(ticketId, status);
            return NoContent();
        }

        [HttpPost("{ticketId}/addons")]
        public async Task<ActionResult> AddTicketAddOnsAsync(int ticketId, [FromBody] IEnumerable<int> addOnIds)
        {
            await dashboardService.AddTicketAddOnsAsync(ticketId, addOnIds);
            return NoContent();
        }

        [HttpGet("flight/{flightId}/occupied-seats")]
        public async Task<ActionResult<IEnumerable<string>>> GetOccupiedSeatsAsync(int flightId)
        {
            IEnumerable<string> seats = await dashboardService.GetOccupiedSeatsAsync(flightId);
            return Ok(seats);
        }

        [HttpGet("flight/{flightId}/seat-available/{seat}")]
        public async Task<ActionResult<bool>> IsSeatAvailableAsync(int flightId, string seat)
        {
            bool isAvailable = await dashboardService.IsSeatAvailableAsync(flightId, seat);
            return Ok(isAvailable);
        }

        [HttpPost("batch")]
        public async Task<ActionResult<bool>> SaveTicketsWithAddOnsAsync([FromBody] SaveTicketsRequestDTO request)
        {
            if (request?.Tickets == null)
            {
                return BadRequest();
            }

            var tickets = new List<FlightTicket>();
            var addOnIds = request.AddOnIds ?? new List<List<int>>();

            for (int counter = 0; counter < request.Tickets.Count; counter++)
            {
                var ticketTransferObject = request.Tickets[counter];
                tickets.Add(new FlightTicket
                {
                    Id = ticketTransferObject.id,
                    User = await authService.GetByIdAsync(ticketTransferObject.userId),
                    Flight = await flightSearchService.GetFlightByIdAsync(ticketTransferObject.flightId),
                    Seat = ticketTransferObject.seat,
                    Price = ticketTransferObject.price,
                    Status = ticketTransferObject.status,
                    PassengerFirstName = ticketTransferObject.passengerFirstName,
                    PassengerLastName = ticketTransferObject.passengerLastName,
                    PassengerEmail = ticketTransferObject.passengerEmail,
                    PassengerPhone = ticketTransferObject.passengerPhone
                });

                if (addOnIds.Count <= counter && ticketTransferObject.selectedAddOns != null)
                {
                    addOnIds.Add(ticketTransferObject.selectedAddOns.Select(addOn => addOn.id).ToList());
                }
            }

            bool isSuccess = await dashboardService.SaveTicketsWithAddOnsAsync(tickets, addOnIds);
            if (!isSuccess)
            {
                return BadRequest();
            }

            return Ok(isSuccess);
        }
    }
}