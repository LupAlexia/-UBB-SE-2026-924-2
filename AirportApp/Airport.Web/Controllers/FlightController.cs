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
        public async Task<ActionResult<AirportApp.ClassLibrary.Entity.Dto.FlightDTO>> GetByIdAsync(int id)
        {
            Flight? flight = await flightRepository.GetFlightByIdAsync(id);
            if (flight == null)
            {
                return NotFound();
            }

            var dto = new AirportApp.ClassLibrary.Entity.Dto.FlightDTO(
                flight.Id,
                flight.RouteId,
                flight.GateId,
                flight.Date,
                flight.FlightNumber,
                flight.Route != null ? new AirportApp.ClassLibrary.Entity.Dto.RouteDTO(
                    flight.Route.Id,
                    flight.Route.RouteType,
                    flight.Route.DepartureTime,
                    flight.Route.ArrivalTime,
                    flight.Route.Capacity,
                    flight.Route.Airport != null ? new AirportApp.ClassLibrary.Entity.Dto.AirportDTO(flight.Route.Airport.Id, flight.Route.Airport.AirportCode, flight.Route.Airport.City) : null,
                    flight.Route.Company != null ? new AirportApp.ClassLibrary.Entity.Dto.CompanyDTO(flight.Route.Company.Id, flight.Route.Company.Name) : null) : null);

            return Ok(dto);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<AirportApp.ClassLibrary.Entity.Dto.FlightDTO>>> GetByRouteAsync(
            [FromQuery] string location,
            [FromQuery] string routeType,
            [FromQuery] DateTime? date)
        {
            IEnumerable<Flight> flights = await flightRepository.GetFlightsByRouteAsync(location, routeType, date);
            var dtos = new List<AirportApp.ClassLibrary.Entity.Dto.FlightDTO>();
            foreach (var flight in flights)
            {
                dtos.Add(new AirportApp.ClassLibrary.Entity.Dto.FlightDTO(
                    flight.Id,
                    flight.RouteId,
                    flight.GateId,
                    flight.Date,
                    flight.FlightNumber,
                    flight.Route != null ? new AirportApp.ClassLibrary.Entity.Dto.RouteDTO(
                        flight.Route.Id,
                        flight.Route.RouteType,
                        flight.Route.DepartureTime,
                        flight.Route.ArrivalTime,
                        flight.Route.Capacity,
                        flight.Route.Airport != null ? new AirportApp.ClassLibrary.Entity.Dto.AirportDTO(flight.Route.Airport.Id, flight.Route.Airport.AirportCode, flight.Route.Airport.City) : null,
                        flight.Route.Company != null ? new AirportApp.ClassLibrary.Entity.Dto.CompanyDTO(flight.Route.Company.Id, flight.Route.Company.Name) : null) : null));
            }
            return Ok(dtos);
        }

        [HttpGet("{flightId}/occupied-seat-count")]
        public async Task<ActionResult<int>> GetOccupiedSeatCountAsync(int flightId)
        {
            int count = await flightRepository.GetOccupiedSeatCountAsync(flightId);
            return Ok(count);
        }
    }
}
