using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using AirportApp.ClassLibrary.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using AirportApp.ClassLibrary.Entity.Dto;

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
        public async Task<ActionResult<IEnumerable<ComplaintTicket>>> GetAllAsync()
        {
            IEnumerable<ComplaintTicket> tickets = await ticketRepository.GetAllAsync();
            return Ok(tickets);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ComplaintTicket>> GetByIdAsync(int id)
        {
            try
            {
                ComplaintTicket ticket = await ticketRepository.GetByIdAsync(id);
                return Ok(ticket);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] CreateTicketDTO dto)
        {

            var ticket = new ComplaintTicket
            {
                CreatorId = dto.CreatorId,
                CategoryId = dto.CategoryId,
                SubcategoryId = dto.SubcategoryId,
                Subject = dto.Subject,
                Description = dto.Description,
                CreationTimestamp = dto.CreationTimestamp,
                CurrentStatus = dto.CurrentStatus,
                UrgencyLevel = dto.UrgencyLevel
            };

            int createdId = await ticketRepository.CreateNewEntityAsync(ticket);
            return CreatedAtAction(nameof(GetByIdAsync), new { id = createdId }, ticket);
        }

        //[HttpPut("{id}")]
        //public async Task<ActionResult> UpdateAsync(int id, [FromBody] Ticket ticket)
        //{
        //    if (id != ticket.Id)
        //    {
        //        return BadRequest("ID in URL does not match ID in body.");
        //    }

        //    await ticketRepository.UpdateByIdAsync(id, ticket);
        //    return NoContent();
        //}

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            await ticketRepository.DeleteByIdAsync(id);
            return NoContent();
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(int id, [FromBody] ComplaintTicket ticket)
        {
            if (id != ticket.Id)
            {
                return BadRequest("ID in URL does not match ID in body.");
            }

            await ticketRepository.UpdateByIdAsync(id, ticket);
            return NoContent();
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult> UpdateStatusAsync(int id, [FromBody] UpdateStatusRequest request)
        {
            await ticketRepository.UpdateStatusByIdAsync(id, request.CurrentStatus);
            return NoContent();
        }

        [HttpPut("{id}/urgency")]
        public async Task<ActionResult> UpdateUrgencyAsync(int id, [FromBody] UpdateUrgencyRequest request)
        {
            await ticketRepository.UpdateUrgencyLevelByIdAsync(id, request.UrgencyLevel);
            return NoContent();
        }
    }

    public class UpdateStatusRequest
    {
        public ComplaintTicketStatusEnum CurrentStatus { get; set; }
    }

    public class UpdateUrgencyRequest
    {
        public ComplaintTicketUrgencyLevelEnum UrgencyLevel { get; set; }
    }
}
