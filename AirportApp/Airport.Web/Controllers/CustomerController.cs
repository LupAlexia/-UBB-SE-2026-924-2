using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Airport.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly IMembershipService membershipService;

        public CustomerController(IAuthService authService, IMembershipService membershipService)
        {
            this.authService = authService;
            this.membershipService = membershipService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDTO>> GetByIdAsync(int id)
        {
            Customer? customer = await authService.GetByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(MapToDTO(customer));
        }

        [HttpGet("by-email")]
        public async Task<ActionResult<CustomerDTO>> GetByEmailAsync([FromQuery] string email)
        {
            Customer? customer = await authService.GetByEmailAsync(email);
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(MapToDTO(customer));
        }

        [HttpPost]
        public async Task<ActionResult> AddUserAsync([FromBody] CustomerDTO customerData)
        {
            var customer = new Customer
            {
                Id = customerData.id,
                Email = customerData.email,
                Phone = customerData.phone,
                Username = customerData.username,
                PasswordHash = customerData.passwordHash,
                Membership = customerData.membership != null
                    ? new Membership
                    {
                        Id = customerData.membership.id,
                        Name = customerData.membership.name,
                        FlightDiscountPercentage = customerData.membership.flightDiscountPercentage
                    }
                    : null
            };

            await authService.AddUserAsync(customer);

            return Ok(new CustomerDTO(
                customer.Id,
                customer.Email,
                customer.Phone,
                customer.Username,
                customer.PasswordHash,
                customer.Membership?.Id,
                null));
        }

        [HttpPost("login")]
        public async Task<ActionResult<CustomerDTO>> LoginAsync([FromBody] LoginRequestDTO request)
        {
            try
            {
                var customer = await authService.LoginAsync(request.Email, request.Password, request.CurrentUserId);
                return Ok(MapToDTO(customer));
            }
            catch (System.InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (System.ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterAsync([FromBody] RegisterRequestDTO request)
        {
            try
            {
                await authService.RegisterAsync(request.Email, request.Phone, request.Username, request.Password);
                return Ok();
            }
            catch (System.InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (System.ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/membership")]
        public async Task<ActionResult> UpdateMembershipAsync(int id, [FromBody] int newMembershipId)
        {
            await membershipService.UpgradeUserMembershipAsync(id, newMembershipId);
            return NoContent();
        }

        private static CustomerDTO MapToDTO(Customer customer)
        {
            return new CustomerDTO(
                customer.Id,
                customer.Email,
                customer.Phone,
                customer.Username,
                customer.PasswordHash,
                customer.Membership?.Id,
                customer.Membership != null
                    ? new MembershipDTO(
                        customer.Membership.Id,
                        customer.Membership.Name,
                        customer.Membership.FlightDiscountPercentage)
                    : null);
        }
    }
}