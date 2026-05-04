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
        private readonly AirportDbContext dataBaseContext;

        public FlightRepository(AirportDbContext dataBaseContext)
        {
            this.dataBaseContext = dataBaseContext;
        }

        public async Task<Flight?> GetFlightByIdAsync(int id)
        {
            return await this.dataBaseContext.flights
                .Include(flight => flight.Route)
                    .ThenInclude(route => route.Company)
                .Include(flight => flight.Route)
                    .ThenInclude(route => route.Airport)
                .Include(flight => flight.Gate)
                .FirstOrDefaultAsync(flight => flight.Id == id);
        }

        public async Task<IEnumerable<Flight>> GetFlightsByRouteAsync(string location, string routeType, DateTime? date)
        {
            var query = this.dataBaseContext.flights
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
            return await this.dataBaseContext.flightTickets
                .Where(flightTicket => flightTicket.Status != "canceled" && flightTicket.Status != "Cancelled")
                .Where(flightTicket => flightTicket.Flight.Id == flightId)
                .CountAsync();
        }
    }
}
