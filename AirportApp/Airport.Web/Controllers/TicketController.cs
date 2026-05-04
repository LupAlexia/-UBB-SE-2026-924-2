using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using AirportApp.ClassLibrary.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Airport.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketRepository ticketRepository;

        public TicketController(ITicketRepository ticketRepository)
        {
            this.ticketRepository = ticketRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetAllAsync()
        {
            IEnumerable<Ticket> tickets = await ticketRepository.GetAllAsync();
            return Ok(tickets);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Ticket>> GetByIdAsync(int id)
        {
            try
            {
                Ticket ticket = await ticketRepository.GetByIdAsync(id);
                return Ok(ticket);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] Ticket ticket)
        {
            int createdId = await ticketRepository.CreateNewEntityAsync(ticket);
            return CreatedAtAction(nameof(GetByIdAsync), new { id = createdId }, ticket);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(int id, [FromBody] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return BadRequest("ID in URL does not match ID in body.");
            }

            await ticketRepository.UpdateByIdAsync(id, ticket);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            await ticketRepository.DeleteByIdAsync(id);
            return NoContent();
        }
    }
}
