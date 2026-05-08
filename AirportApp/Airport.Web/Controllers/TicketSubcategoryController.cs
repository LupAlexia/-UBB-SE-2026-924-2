using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
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
        public async Task<ActionResult<IEnumerable<ComplaintTicketSubcategory>>> GetAllAsync()
        {
            IEnumerable<ComplaintTicketSubcategory> subcategories = await ticketSubcategoryRepository.GetAllAsync();
            return Ok(subcategories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ComplaintTicketSubcategory>> GetByIdAsync(int id)
        {
            try
            {
                ComplaintTicketSubcategory subcategory = await ticketSubcategoryRepository.GetByIdAsync(id);
                return Ok(subcategory);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("by-category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<ComplaintTicketSubcategory>>> GetByCategoryIdAsync(int categoryId)
        {
            IEnumerable<ComplaintTicketSubcategory> subcategories = await ticketSubcategoryRepository.GetByCategoryIdAsync(categoryId);
            return Ok(subcategories);
        }
    }
}
