using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Airport.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketSubcategoryController : ControllerBase
    {
        private readonly IComplaintTicketSubcategoryService ticketSubcategoryService;

        public TicketSubcategoryController(IComplaintTicketSubcategoryService ticketSubcategoryService)
        {
            this.ticketSubcategoryService = ticketSubcategoryService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ComplaintTicketSubcategory>> GetByIdAsync(int id)
        {
            try
            {
                ComplaintTicketSubcategory subcategory = await ticketSubcategoryService.GetSubcategoryByIdAsync(id);
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
            IEnumerable<ComplaintTicketSubcategory> subcategories = await ticketSubcategoryService.GetSubcategoriesByCategoryIdAsync(categoryId);
            return Ok(subcategories);
        }
    }
}