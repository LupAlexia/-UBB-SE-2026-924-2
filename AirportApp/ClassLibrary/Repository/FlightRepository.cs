using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.DataAccess;

namespace AirportApp.ClassLibrary.Repository
{
    public class FlightRepository : IFlightRepository
    {
        private readonly AirportDbContext databaseContext;

        public FlightRepository(AirportDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public async Task<Flight?> GetFlightByIdAsync(int flightIdentifier)
        {
            return await this.databaseContext.Flights
                .Include(flight => flight.Route)
                    .ThenInclude(route => route.Company)
                .Include(flight => flight.Route)
                    .ThenInclude(route => route.Airport)
                .Include(flight => flight.Gate)
                .FirstOrDefaultAsync(flight => flight.Id == flightIdentifier);
        }

        public async Task<IEnumerable<Flight>> GetFlightsByRouteAsync(string location, string routeType, DateTime? date)
        {
            var query = this.databaseContext.Flights
                .Include(flight => flight.Route)
                    .ThenInclude(route => route.Company)
                .Include(flight => flight.Route)
                    .ThenInclude(route => route.Airport)
                .Include(flight => flight.Gate)
                .AsQueryable();

            if (date.HasValue)
            {
                query = query.Where(flight => flight.Date.Date == date.Value.Date);
            }

            query = query.Where(flight => flight.Route.RouteType == routeType);

            query = query.Where(flight =>
                flight.Route.Airport.City == location ||
                flight.Route.Airport.AirportCode == location);

            return await query.ToListAsync();
        }

        public async Task<int> GetOccupiedSeatCountAsync(int flightId)
        {
            return await this.databaseContext.FlightTickets
                .Where(flightTicket => flightTicket.Status != "canceled" && flightTicket.Status != "Cancelled")
                .Where(flightTicket => flightTicket.Flight.Id == flightId)
                .CountAsync();
        }
    }
}
