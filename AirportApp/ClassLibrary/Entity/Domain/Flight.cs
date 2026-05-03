using System;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    public class Flight
    {
        public int FlightId { get; set; }
        public Route? Route { get; set; }
        public Gate? Gate { get; set; }
        public DateTime Date { get; set; }
        public string? FlightNumber { get; set; }

        public Flight()
        {
        }

        public Flight(Route? route, Gate? gate, DateTime date, string? flightNumber)
        {
            Route = route;
            Gate = gate;
            Date = date;
            FlightNumber = flightNumber;
        }

        public Flight(int flightId, Route route, Gate gate, DateTime date, string flightNumber)
        {
            FlightId = flightId;
            Route = route;
            Gate = gate;
            Date = date;
            FlightNumber = flightNumber;
        }
    }
}
