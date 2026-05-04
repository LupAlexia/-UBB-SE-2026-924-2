using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Src.Service
{
    public interface IFlightSearchService
    {
        Task<IEnumerable<Flight>> SearchFlightsAsync(string location, bool isDeparture, DateTime? date, int? passengers);
        int? ParsePassengerCount(string input);
    }
}
