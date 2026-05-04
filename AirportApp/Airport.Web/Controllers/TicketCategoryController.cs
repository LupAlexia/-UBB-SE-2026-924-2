using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using AirportApp.ClassLibrary.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Airport.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketCategoryController : ControllerBase
    {
        private readonly ITicketCategoryRepository ticketCategoryRepository;

        public TicketCategoryController(ITicketCategoryRepository ticketCategoryRepository)
        {
            this.ticketCategoryRepository = ticketCategoryRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketCategory>>> GetAllAsync()
        {
            IEnumerable<TicketCategory> categories = await ticketCategoryRepository.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TicketCategory>> GetByIdAsync(int id)
        {
            try
            {
                TicketCategory category = await ticketCategoryRepository.GetByIdAsync(id);
                return Ok(category);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
