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
        public async Task<ActionResult<IEnumerable<AirportApp.ClassLibrary.Entity.Dto.MembershipDTO>>> GetAllAsync()
        {
            IEnumerable<Membership> memberships = await membershipRepository.GetAllMembershipsAsync();
            var dtos = new List<AirportApp.ClassLibrary.Entity.Dto.MembershipDTO>();
            foreach (var membership in memberships)
            {
                dtos.Add(new AirportApp.ClassLibrary.Entity.Dto.MembershipDTO(membership.Id, membership.Name, membership.FlightDiscountPercentage));
            }
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AirportApp.ClassLibrary.Entity.Dto.MembershipDTO>> GetByIdAsync(int id)
        {
            Membership? membership = await membershipRepository.GetMembershipByIdAsync(id);
            if (membership == null)
            {
                return NotFound();
            }

            var dto = new AirportApp.ClassLibrary.Entity.Dto.MembershipDTO(membership.Id, membership.Name, membership.FlightDiscountPercentage);
            return Ok(dto);
        }

        [HttpGet("{id}/addon-discounts")]
        public async Task<ActionResult<IEnumerable<AirportApp.ClassLibrary.Entity.Dto.MembershipAddonDiscountDTO>>> GetAddonDiscountsAsync(int id)
        {
            IEnumerable<MembershipAddonDiscount> discounts = await membershipRepository.GetAddonDiscountsAsync(id);
            var dtos = new List<AirportApp.ClassLibrary.Entity.Dto.MembershipAddonDiscountDTO>();
            foreach (var discount in discounts)
            {
                dtos.Add(new AirportApp.ClassLibrary.Entity.Dto.MembershipAddonDiscountDTO(
                    id,
                    discount.AddOn?.Id ?? 0,
                    discount.DiscountPercentage,
                    discount.AddOn?.Name ?? string.Empty));
            }
            return Ok(dtos);
        }
    }
}
