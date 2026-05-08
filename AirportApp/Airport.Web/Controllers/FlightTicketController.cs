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
        public async Task<ActionResult<IEnumerable<FlightTicket>>> GetByUserIdAsync(int userId)
        {
            IEnumerable<FlightTicket> tickets = await flightTicketRepository.GetTicketsByUserIdAsync(userId);
            return Ok(tickets);
        }

        [HttpPost]
        public async Task<ActionResult> AddTicketAsync([FromBody] FlightTicket ticket)
        {
            await flightTicketRepository.AddTicketAsync(ticket);
            return Ok(ticket);
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
        public async Task<ActionResult<bool>> SaveTicketsWithAddOnsAsync([FromBody] SaveTicketsRequest request)
        {
            if (request?.Tickets == null)
            {
                return BadRequest();
            }

            bool isSuccess = await flightTicketRepository.SaveTicketsWithAddOnsAsync(request.Tickets, request.AddOnIds);
            if (!isSuccess)
            {
                return BadRequest();
            }

            return Ok(isSuccess);
        }
    }
}
