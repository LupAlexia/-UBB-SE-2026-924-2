using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Service.Interfaces;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Entity.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Airport.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly IComplaintTicketService ticketService;
        private readonly IUserService userService;
        private readonly IComplaintTicketCategoryService ticketCategoryService;
        private readonly IComplaintTicketSubcategoryService ticketSubcategoryService;

        public TicketController(
            IComplaintTicketService ticketService,
            IUserService userService,
            IComplaintTicketCategoryService ticketCategoryService,
            IComplaintTicketSubcategoryService ticketSubcategoryService)
        {
            this.ticketService = ticketService;
            this.userService = userService;
            this.ticketCategoryService = ticketCategoryService;
            this.ticketSubcategoryService = ticketSubcategoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ComplaintTicket>>> GetAllAsync()
        {
            IEnumerable<ComplaintTicket> tickets = await ticketService.GetAllTicketsAsync();
            return Ok(tickets);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ComplaintTicket>> GetByIdAsync(int id)
        {
            try
            {
                ComplaintTicket ticket = await ticketService.GetTicketByIdAsync(id);
                return Ok(ticket);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] CreateTicketDTO ticketCreationData)
        {
            var ticket = new ComplaintTicket
            {
                Creator = await userService.GetByIdAsync(ticketCreationData.creatorId),
                Category = await ticketCategoryService.GetCategoryByIdAsync(ticketCreationData.categoryId),
                Subcategory = await ticketSubcategoryService.GetSubcategoryByIdAsync(ticketCreationData.subcategoryId),
                Subject = ticketCreationData.subject,
                Description = ticketCreationData.description,
                CreationTimestamp = ticketCreationData.creationTimestamp,
                CurrentStatus = ticketCreationData.currentStatus,
                UrgencyLevel = ticketCreationData.urgencyLevel
            };

            await ticketService.AddTicketAsync(ticket);
            return CreatedAtAction(nameof(GetByIdAsync), new { id = ticket.Id }, ticket);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            await ticketService.DeleteTicketByIdAsync(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(int id, [FromBody] ComplaintTicket ticket)
        {
            if (id != ticket.Id)
            {
                return BadRequest("ID in URL does not match ID in body.");
            }

            await ticketService.UpdateTicketByIdAsync(id, ticket);
            return NoContent();
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult> UpdateStatusAsync(int id, [FromBody] UpdateStatusRequest request)
        {
            await ticketService.UpdateStatusAsync(id, request.CurrentStatus);
            return NoContent();
        }

        [HttpPut("{id}/urgency")]
        public async Task<ActionResult> UpdateUrgencyAsync(int id, [FromBody] UpdateUrgencyRequest request)
        {
            await ticketService.UpdateUrgencyLevelAsync(id, request.UrgencyLevel);
            return NoContent();
        }

        [HttpPost("filter")]
        public async Task<ActionResult<IEnumerable<TicketDTO>>> FilterAsync([FromQuery] TicketFilterStatusEnum filter, [FromBody] IEnumerable<TicketDTO> tickets)
        {
            IEnumerable<TicketDTO> filteredTickets = await ticketService.FilterTicketsByStatusAsync(tickets, filter);
            return Ok(filteredTickets);
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