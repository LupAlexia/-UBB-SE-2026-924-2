using System;
using System.Collections.Generic;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interfaces
{
    public interface IFlightRepository
    {
        IEnumerable<Flight> GetFlightsByRoute(string location, string routeType, DateTime? date);

        Flight? GetFlightById(int id);

        int GetOccupiedSeatCount(int flightId);
    }
}
