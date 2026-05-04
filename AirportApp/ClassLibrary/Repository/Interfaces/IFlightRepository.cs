using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interfaces
{
    public interface IFlightRepository
    {
        Task<IEnumerable<Flight>> GetFlightsByRouteAsync(string location, string routeType, DateTime? date);

        Task<Flight?> GetFlightByIdAsync(int id);

        Task<int> GetOccupiedSeatCountAsync(int flightId);
    }
}
