using System;
using System.Collections.Generic;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Src.Service
{
    public interface IFlightSearchService
    {
        IEnumerable<Flight> SearchFlights(string location, bool isDeparture, DateTime? date, int? passengers);
        int? ParsePassengerCount(string input);
    }
}
