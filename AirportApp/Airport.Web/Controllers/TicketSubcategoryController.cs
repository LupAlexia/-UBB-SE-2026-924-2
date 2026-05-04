using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using AirportApp.ClassLibrary.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Airport.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketSubcategoryController : ControllerBase
    {
        private readonly ITicketSubcategoryRepository ticketSubcategoryRepository;

        public TicketSubcategoryController(ITicketSubcategoryRepository ticketSubcategoryRepository)
        {
            this.ticketSubcategoryRepository = ticketSubcategoryRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketSubcategory>>> GetAllAsync()
        {
            IEnumerable<TicketSubcategory> subcategories = await ticketSubcategoryRepository.GetAllAsync();
            return Ok(subcategories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TicketSubcategory>> GetByIdAsync(int id)
        {
            try
            {
                TicketSubcategory subcategory = await ticketSubcategoryRepository.GetByIdAsync(id);
                return Ok(subcategory);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("by-category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<TicketSubcategory>>> GetByCategoryIdAsync(int categoryId)
        {
            IEnumerable<TicketSubcategory> subcategories = await ticketSubcategoryRepository.GetByCategoryIdAsync(categoryId);
            return Ok(subcategories);
        }
    }
}
