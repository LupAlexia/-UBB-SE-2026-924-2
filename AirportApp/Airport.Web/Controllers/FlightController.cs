using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Airport.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightController : ControllerBase
    {
        private readonly IFlightRepository flightRepository;

        public FlightController(IFlightRepository flightRepository)
        {
            this.flightRepository = flightRepository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Flight>> GetByIdAsync(int id)
        {
            Flight? flight = await flightRepository.GetFlightByIdAsync(id);
            if (flight == null)
            {
                return NotFound();
            }

            return Ok(flight);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Flight>>> GetByRouteAsync(
            [FromQuery] string location,
            [FromQuery] string routeType,
            [FromQuery] DateTime? date)
        {
            IEnumerable<Flight> flights = await flightRepository.GetFlightsByRouteAsync(location, routeType, date);
            return Ok(flights);
        }

        [HttpGet("{flightId}/occupied-seat-count")]
        public async Task<ActionResult<int>> GetOccupiedSeatCountAsync(int flightId)
        {
            int count = await flightRepository.GetOccupiedSeatCountAsync(flightId);
            return Ok(count);
        }
    }
}
