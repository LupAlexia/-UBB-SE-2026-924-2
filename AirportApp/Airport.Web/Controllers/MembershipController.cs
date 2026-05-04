using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Airport.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MembershipController : ControllerBase
    {
        private readonly IMembershipRepository membershipRepository;

        public MembershipController(IMembershipRepository membershipRepository)
        {
            this.membershipRepository = membershipRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Membership>>> GetAllAsync()
        {
            IEnumerable<Membership> memberships = await membershipRepository.GetAllMembershipsAsync();
            return Ok(memberships);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Membership>> GetByIdAsync(int id)
        {
            Membership? membership = await membershipRepository.GetMembershipByIdAsync(id);
            if (membership == null)
            {
                return NotFound();
            }

            return Ok(membership);
        }

        [HttpGet("{id}/addon-discounts")]
        public async Task<ActionResult<IEnumerable<MembershipAddonDiscount>>> GetAddonDiscountsAsync(int id)
        {
            IEnumerable<MembershipAddonDiscount> discounts = await membershipRepository.GetAddonDiscountsAsync(id);
            return Ok(discounts);
        }
    }
}
