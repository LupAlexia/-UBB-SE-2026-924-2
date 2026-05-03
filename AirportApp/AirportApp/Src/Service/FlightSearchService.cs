using System;
using System.Collections.Generic;
using System.Linq;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.Src.Service
{
    public class FlightSearchService : IFlightSearchService
    {
        private const string DepartureRouteType = "DEP";
        private const string ArrivalRouteType = "ARR";

        private readonly IFlightRepository flightRepository;

        public FlightSearchService(IFlightRepository flightRepository)
        {
            this.flightRepository = flightRepository ?? throw new ArgumentNullException(nameof(flightRepository));
        }

        public IEnumerable<Flight> SearchFlights(string location, bool isDeparture, DateTime? date, int? passengers)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                return new List<Flight>();
            }

            string flightType = isDeparture ? DepartureRouteType : ArrivalRouteType;
            var flights = this.flightRepository.GetFlightsByRoute(location, flightType, date);

            if (passengers.HasValue && passengers.Value > 0)
            {
                flights = flights.Where(flight =>
                {
                    int occupiedSeats = this.flightRepository.GetOccupiedSeatCount(flight.Id);
                    int availableSeats = flight.Route!.Capacity - occupiedSeats;
                    return availableSeats >= passengers.Value;
                });
            }

            return flights;
        }
        public int? ParsePassengerCount(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            if (int.TryParse(input, out var parsed) && parsed > 0)
            {
                return parsed;
            }

            return 1;
        }
    }
}
