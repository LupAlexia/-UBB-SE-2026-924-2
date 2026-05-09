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
        public async Task<ActionResult<AirportApp.ClassLibrary.Entity.Dto.CustomerDTO>> GetByIdAsync(int id)
        {
            Customer? customer = await customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            var dto = new AirportApp.ClassLibrary.Entity.Dto.CustomerDTO(
                customer.Id,
                customer.Email,
                customer.Phone,
                customer.Username,
                customer.PasswordHash,
                customer.MembershipId,
                customer.Membership != null ? new AirportApp.ClassLibrary.Entity.Dto.MembershipDTO(
                    customer.Membership.Id,
                    customer.Membership.Name,
                    customer.Membership.FlightDiscountPercentage) : null);

            return Ok(dto);
        }

        [HttpGet("by-email")]
        public async Task<ActionResult<AirportApp.ClassLibrary.Entity.Dto.CustomerDTO>> GetByEmailAsync([FromQuery] string email)
        {
            Customer? customer = await customerRepository.GetByEmailAsync(email);
            if (customer == null)
            {
                return NotFound();
            }

            var dto = new AirportApp.ClassLibrary.Entity.Dto.CustomerDTO(
                customer.Id,
                customer.Email,
                customer.Phone,
                customer.Username,
                customer.PasswordHash,
                customer.MembershipId,
                customer.Membership != null ? new AirportApp.ClassLibrary.Entity.Dto.MembershipDTO(
                    customer.Membership.Id,
                    customer.Membership.Name,
                    customer.Membership.FlightDiscountPercentage) : null);

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult> AddUserAsync([FromBody] AirportApp.ClassLibrary.Entity.Dto.CustomerDTO dto)
        {
            var customer = new Customer
            {
                Id = dto.id,
                Email = dto.email,
                Phone = dto.phone,
                Username = dto.username,
                PasswordHash = dto.passwordHash,
                MembershipId = dto.membershipId
            };
            await customerRepository.AddUserAsync(customer);
            return Ok(dto);
        }

        [HttpPut("{id}/membership")]
        public async Task<ActionResult> UpdateMembershipAsync(int id, [FromBody] int newMembershipId)
        {
            await customerRepository.UpdateUserMembershipAsync(id, newMembershipId);
            return NoContent();
        }
    }
}
