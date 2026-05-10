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
        private readonly IMembershipRepository membershipRepository;

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

            var customerTransferObject = new AirportApp.ClassLibrary.Entity.Dto.CustomerDTO(
                customer.Id,
                customer.Email,
                customer.Phone,
                customer.Username,
                customer.PasswordHash,
                customer.Membership != null ? customer.Membership.Id : null,
                customer.Membership != null ? new AirportApp.ClassLibrary.Entity.Dto.MembershipDTO(
                    customer.Membership.Id,
                    customer.Membership.Name,
                    customer.Membership.FlightDiscountPercentage) : null);

            return Ok(customerTransferObject);
        }

        [HttpGet("by-email")]
        public async Task<ActionResult<AirportApp.ClassLibrary.Entity.Dto.CustomerDTO>> GetByEmailAsync([FromQuery] string email)
        {
            Customer? customer = await customerRepository.GetByEmailAsync(email);
            if (customer == null)
            {
                return NotFound();
            }

            var customerTransferObject = new AirportApp.ClassLibrary.Entity.Dto.CustomerDTO(
                customer.Id,
                customer.Email,
                customer.Phone,
                customer.Username,
                customer.PasswordHash,
                customer.Membership != null ? customer.Membership.Id : null,
                customer.Membership != null ? new AirportApp.ClassLibrary.Entity.Dto.MembershipDTO(
                    customer.Membership.Id,
                    customer.Membership.Name,
                    customer.Membership.FlightDiscountPercentage) : null);

            return Ok(customerTransferObject);
        }

        [HttpPost]
        public async Task<ActionResult> AddUserAsync([FromBody] AirportApp.ClassLibrary.Entity.Dto.CustomerDTO customerData)
        {
            var customer = new Customer
            {
                Id = customerData.id,
                Email = customerData.email,
                Phone = customerData.phone,
                Username = customerData.username,
                PasswordHash = customerData.passwordHash,
                Membership = customerData.membership != null ? new Membership { Id = customerData.membership.id, Name = customerData.membership.name, FlightDiscountPercentage = customerData.membership.flightDiscountPercentage } : null
            };
            await customerRepository.AddUserAsync(customer);

            var createdCustomerTransferObject = new AirportApp.ClassLibrary.Entity.Dto.CustomerDTO(
                customer.Id,
                customer.Email,
                customer.Phone,
                customer.Username,
                customer.PasswordHash,
                customer.Membership != null ? customer.Membership.Id : null,
                null);

            return Ok(createdCustomerTransferObject);
        }

        [HttpPut("{id}/membership")]
        public async Task<ActionResult> UpdateMembershipAsync(int id, [FromBody] int newMembershipId)
        {
            await customerRepository.UpdateUserMembershipAsync(id, newMembershipId);
            return NoContent();
        }
    }
}
