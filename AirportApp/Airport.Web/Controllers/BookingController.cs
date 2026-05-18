using System.Collections.Generic;
using System.Linq;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Airport.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService bookingService;

        public BookingController(IBookingService bookingService)
        {
            this.bookingService = bookingService;
        }

        [HttpPost("validate-passengers")]
        public async Task<ActionResult<string>> ValidatePassengersAsync([FromBody] List<PassengerDataDTO> passengerDtos)
        {
            var passengers = passengerDtos.Select(dto => new PassengerData
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                SelectedSeat = dto.SelectedSeat,
                SelectedAddOns = dto.SelectedAddOns?.Select(a => new AddOn { Id = a.Id, BasePrice = a.BasePrice }).ToList() ?? new List<AddOn>()
            }).ToList();

            var validationResult = await bookingService.ValidatePassengersAsync(passengers);
            return Ok(validationResult);
        }

        [HttpGet("calculate-max-passengers")]
        public async Task<ActionResult<int>> CalculateMaxPassengersAsync([FromQuery] int routeCapacity, [FromQuery] int occupiedSeatCount, [FromQuery] int requestedPassengerCount)
        {
            var maxPassengers = await bookingService.CalculateMaxPassengersAsync(routeCapacity, occupiedSeatCount, requestedPassengerCount);
            return Ok(maxPassengers);
        }

        [HttpGet("build-seat-map")]
        public async Task<ActionResult<SeatMapResponseDTO>> BuildSeatMapLayoutAsync([FromQuery] int capacity)
        {
            var (layout, rowCount) = await bookingService.BuildSeatMapLayoutAsync(capacity);
            return Ok(new SeatMapResponseDTO { Layout = layout, RowCount = rowCount });
        }

        [HttpGet("initial-passenger-count")]
        public async Task<ActionResult<int>> GetInitialPassengerCountAsync([FromQuery] int maxPassengers, [FromQuery] int requestedCount)
        {
            var count = await bookingService.GetInitialPassengerCountAsync(maxPassengers, requestedCount);
            return Ok(count);
        }
    }
}
