using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Airport.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddOnController : ControllerBase
    {
        private readonly IBookingService bookingService;

        public AddOnController(IBookingService bookingService)
        {
            this.bookingService = bookingService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AddOnDTO>>> GetAllAsync()
        {
            List<AddOn> addOns = await bookingService.GetAvailableAddOnsAsync();
            var addOnTransferObjectList = new List<AddOnDTO>();
            foreach (var addOn in addOns)
            {
                addOnTransferObjectList.Add(new AddOnDTO(addOn.Id, addOn.Name, addOn.BasePrice));
            }

            return Ok(addOnTransferObjectList);
        }

        [HttpPost("by-ids")]
        public async Task<ActionResult<IEnumerable<AddOnDTO>>> GetByIdsAsync([FromBody] List<int> ids)
        {
            List<AddOn> addOns = await bookingService.GetAddOnsByIdsAsync(ids);
            var addOnTransferObjectList = new List<AddOnDTO>();
            foreach (var addOn in addOns)
            {
                addOnTransferObjectList.Add(new AddOnDTO(addOn.Id, addOn.Name, addOn.BasePrice));
            }

            return Ok(addOnTransferObjectList);
        }
    }
}