using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Airport.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketCategoryController : ControllerBase
    {
        private readonly IComplaintTicketCategoryService ticketCategoryService;

        public TicketCategoryController(IComplaintTicketCategoryService ticketCategoryService)
        {
            this.ticketCategoryService = ticketCategoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ComplaintTicketCategory>>> GetAllAsync()
        {
            IEnumerable<ComplaintTicketCategory> categories = await ticketCategoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ComplaintTicketCategory>> GetByIdAsync(int id)
        {
            try
            {
                ComplaintTicketCategory category = await ticketCategoryService.GetCategoryByIdAsync(id);
                return Ok(category);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}