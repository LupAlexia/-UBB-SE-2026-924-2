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

        public FlightTicketController(IFlightTicketRepository flightTicketRepository)
        {
            this.flightTicketRepository = flightTicketRepository;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<AirportApp.ClassLibrary.Entity.Dto.FlightTicketDTO>>> GetByUserIdAsync(int userId)
        {
            IEnumerable<FlightTicket> tickets = await flightTicketRepository.GetTicketsByUserIdAsync(userId);
            var dtos = new List<AirportApp.ClassLibrary.Entity.Dto.FlightTicketDTO>();
            foreach (var ticket in tickets)
            {
                dtos.Add(new AirportApp.ClassLibrary.Entity.Dto.FlightTicketDTO(
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
                    ticket.SelectedAddOns?.Select(a => new AirportApp.ClassLibrary.Entity.Dto.AddOnDTO(a.Id, a.Name, a.BasePrice)).ToList() ?? new List<AirportApp.ClassLibrary.Entity.Dto.AddOnDTO>()));
            }
            return Ok(dtos);
        }

        [HttpPost]
        public async Task<ActionResult> AddTicketAsync([FromBody] AirportApp.ClassLibrary.Entity.Dto.FlightTicketDTO dto)
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
                PassengerPhone = dto.passengerPhone
                // Add-ons are usually handled via separate endpoint or batch
            };
            await flightTicketRepository.AddTicketAsync(ticket);
            return Ok(dto);
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
            foreach (var dto in request.Tickets)
            {
                tickets.Add(new FlightTicket
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
                    PassengerPhone = dto.passengerPhone
                });
            }

            bool isSuccess = await flightTicketRepository.SaveTicketsWithAddOnsAsync(tickets, request.AddOnIds);
            if (!isSuccess)
            {
                return BadRequest();
            }

            return Ok(isSuccess);
        }
    }
}
