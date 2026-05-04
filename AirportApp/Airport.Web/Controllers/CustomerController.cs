using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Airport.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            this.customerRepository = customerRepository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetByIdAsync(int id)
        {
            Customer? customer = await customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        [HttpGet("by-email")]
        public async Task<ActionResult<Customer>> GetByEmailAsync([FromQuery] string email)
        {
            Customer? customer = await customerRepository.GetByEmailAsync(email);
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        [HttpPost]
        public async Task<ActionResult> AddUserAsync([FromBody] Customer customer)
        {
            await customerRepository.AddUserAsync(customer);
            return Ok(customer);
        }

        [HttpPut("{id}/membership")]
        public async Task<ActionResult> UpdateMembershipAsync(int id, [FromBody] int newMembershipId)
        {
            await customerRepository.UpdateUserMembershipAsync(id, newMembershipId);
            return NoContent();
        }
    }
}
