using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Airport.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddOnController : ControllerBase
    {
        private readonly IAddOnRepository addOnRepository;

        public AddOnController(IAddOnRepository addOnRepository)
        {
            this.addOnRepository = addOnRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AddOn>>> GetAllAsync()
        {
            IEnumerable<AddOn> addOns = await addOnRepository.GetAllAddOnsAsync();
            return Ok(addOns);
        }

        [HttpGet("by-ids")]
        public async Task<ActionResult<IEnumerable<AddOn>>> GetByIdsAsync([FromQuery] List<int> ids)
        {
            IEnumerable<AddOn> addOns = await addOnRepository.GetAddOnsByIdsAsync(ids);
            return Ok(addOns);
        }
    }
}
