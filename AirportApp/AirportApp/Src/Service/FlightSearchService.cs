using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service.Interfaces;

namespace AirportApp.Src.Service
{
    public class FlightSearchService : IFlightSearchService
    {
        private const string DepartureRouteType = "Departure";
        private const string ArrivalRouteType = "Arrival";

        private readonly IFlightRepository flightRepository;

        public FlightSearchService(IFlightRepository flightRepository)
        {
            this.flightRepository = flightRepository ?? throw new ArgumentNullException(nameof(flightRepository));
        }

        public async Task<IEnumerable<Flight>> SearchFlightsAsync(string location, bool isDeparture, DateTime? date, int? passengers)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                return new List<Flight>();
            }

            string flightType = isDeparture ? DepartureRouteType : ArrivalRouteType;
            var flights = (await this.flightRepository.GetFlightsByRouteAsync(location, flightType, date)).ToList();

            if (passengers.HasValue && passengers.Value > 0)
            {
                var filteredFlights = new List<Flight>();
                foreach (var flight in flights)
                {
                    int occupiedSeats = await this.flightRepository.GetOccupiedSeatCountAsync(flight.Id);
                    int availableSeats = flight.Route!.Capacity - occupiedSeats;
                    if (availableSeats >= passengers.Value)
                    {
                        filteredFlights.Add(flight);
                    }
                }

                return filteredFlights;
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
