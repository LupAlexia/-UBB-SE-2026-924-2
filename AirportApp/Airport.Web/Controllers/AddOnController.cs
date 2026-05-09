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
        public async Task<ActionResult<IEnumerable<AirportApp.ClassLibrary.Entity.Dto.AddOnDTO>>> GetAllAsync()
        {
            IEnumerable<AddOn> addOns = await addOnRepository.GetAllAddOnsAsync();
            var dtos = new List<AirportApp.ClassLibrary.Entity.Dto.AddOnDTO>();
            foreach (var addOn in addOns)
            {
                dtos.Add(new AirportApp.ClassLibrary.Entity.Dto.AddOnDTO(addOn.Id, addOn.Name, addOn.BasePrice));
            }
            return Ok(dtos);
        }

        [HttpPost("by-ids")]
        public async Task<ActionResult<IEnumerable<AirportApp.ClassLibrary.Entity.Dto.AddOnDTO>>> GetByIdsAsync([FromBody] List<int> ids)
        {
            IEnumerable<AddOn> addOns = await addOnRepository.GetAddOnsByIdsAsync(ids);
            var dtos = new List<AirportApp.ClassLibrary.Entity.Dto.AddOnDTO>();
            foreach (var addOn in addOns)
            {
                dtos.Add(new AirportApp.ClassLibrary.Entity.Dto.AddOnDTO(addOn.Id, addOn.Name, addOn.BasePrice));
            }
            return Ok(dtos);
        }
    }
}
