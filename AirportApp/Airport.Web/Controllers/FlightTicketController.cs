using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Airport.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightTicketController : ControllerBase
    {
        private readonly IFlightTicketRepository flightTicketRepository;
        private readonly ICustomerRepository customerRepository;
        private readonly IFlightRepository flightRepository;

        public FlightTicketController(IFlightTicketRepository flightTicketRepository, ICustomerRepository customerRepository, IFlightRepository flightRepository)
        {
            this.flightTicketRepository = flightTicketRepository;
            this.flightRepository = flightRepository;
            this.customerRepository = customerRepository;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<AirportApp.ClassLibrary.Entity.Dto.FlightTicketDTO>>> GetByUserIdAsync(int userId)
        {
            IEnumerable<FlightTicket> tickets = await flightTicketRepository.GetTicketsByUserIdAsync(userId);
            var flightTicketTransferObjectList = new List<AirportApp.ClassLibrary.Entity.Dto.FlightTicketDTO>();
            foreach (var ticket in tickets)
            {
                flightTicketTransferObjectList.Add(new AirportApp.ClassLibrary.Entity.Dto.FlightTicketDTO(
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
                    ticket.SelectedAddOns?.Select(addOn => new AirportApp.ClassLibrary.Entity.Dto.AddOnDTO(addOn.Id, addOn.Name, addOn.BasePrice)).ToList() ?? new List<AirportApp.ClassLibrary.Entity.Dto.AddOnDTO>(),
                    ticket.Flight != null ? new AirportApp.ClassLibrary.Entity.Dto.FlightDTO(
                        ticket.Flight.Id,
                        ticket.Flight.Route.Id,
                        ticket.Flight.Gate.Id,
                        ticket.Flight.Date,
                        ticket.Flight.FlightNumber,
                        ticket.Flight.Route != null ? new AirportApp.ClassLibrary.Entity.Dto.RouteDTO(
                            ticket.Flight.Route.Id,
                            ticket.Flight.Route.RouteType,
                            ticket.Flight.Route.DepartureTime,
                            ticket.Flight.Route.ArrivalTime,
                            ticket.Flight.Route.Capacity,
                            ticket.Flight.Route.Airport != null ? new AirportApp.ClassLibrary.Entity.Dto.AirportDTO(ticket.Flight.Route.Airport.Id, ticket.Flight.Route.Airport.AirportCode, ticket.Flight.Route.Airport.City) : null,
                            ticket.Flight.Route.Company != null ? new AirportApp.ClassLibrary.Entity.Dto.CompanyDTO(ticket.Flight.Route.Company.Id, ticket.Flight.Route.Company.Name) : null) : null) : null));
            }
            return Ok(flightTicketTransferObjectList);
        }

        [HttpPost]
        public async Task<ActionResult> AddTicketAsync([FromBody] AirportApp.ClassLibrary.Entity.Dto.FlightTicketDTO flightTicketData)
        {
            var ticket = new FlightTicket
            {
                Id = flightTicketData.id,
                User = await customerRepository.GetByIdAsync(flightTicketData.userId),
                Flight = await flightRepository.GetFlightByIdAsync(flightTicketData.flightId),
                Seat = flightTicketData.seat,
                Price = flightTicketData.price,
                Status = flightTicketData.status,
                PassengerFirstName = flightTicketData.passengerFirstName,
                PassengerLastName = flightTicketData.passengerLastName,
                PassengerEmail = flightTicketData.passengerEmail,
                PassengerPhone = flightTicketData.passengerPhone
                // Add-ons are usually handled via separate endpoint or batch
            };
            await flightTicketRepository.AddTicketAsync(ticket);
            return Ok(flightTicketData);
        }

        [HttpPut("{ticketId}/status")]
        public async Task<ActionResult> UpdateTicketStatusAsync(int ticketId, [FromBody] string status)
        {
            await flightTicketRepository.UpdateTicketStatusAsync(ticketId, status);
            return NoContent();
        }

        [HttpPost("{ticketId}/addons")]
        public async Task<ActionResult> AddTicketAddOnsAsync(int ticketId, [FromBody] IEnumerable<int> addOnIds)
        {
            await flightTicketRepository.AddTicketAddOnsAsync(ticketId, addOnIds);
            return NoContent();
        }

        [HttpGet("flight/{flightId}/occupied-seats")]
        public async Task<ActionResult<IEnumerable<string>>> GetOccupiedSeatsAsync(int flightId)
        {
            IEnumerable<string> seats = await flightTicketRepository.GetOccupiedSeatsAsync(flightId);
            return Ok(seats);
        }

        [HttpGet("flight/{flightId}/seat-available/{seat}")]
        public async Task<ActionResult<bool>> IsSeatAvailableAsync(int flightId, string seat)
        {
            bool isAvailable = await flightTicketRepository.IsSeatAvailableAsync(flightId, seat);
            return Ok(isAvailable);
        }

        [HttpPost("batch")]
        public async Task<ActionResult<bool>> SaveTicketsWithAddOnsAsync([FromBody] AirportApp.ClassLibrary.Entity.Dto.SaveTicketsRequestDTO request)
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
                    User = await customerRepository.GetByIdAsync(ticketTransferObject.userId),
                    Flight = await flightRepository.GetFlightByIdAsync(ticketTransferObject.flightId),
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

            bool isSuccess = await flightTicketRepository.SaveTicketsWithAddOnsAsync(tickets, addOnIds);
            if (!isSuccess)
            {
                return BadRequest();
            }

            return Ok(isSuccess);
        }
    }
}
