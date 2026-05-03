using System;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    public class Route
    {
        public int RouteId { get; set; }

        public Company? Company { get; set; }

        public Airport? Airport { get; set; }

        public string? RouteType { get; set; }

        public DateTime DepartureTime { get; set; }

        public DateTime ArrivalTime { get; set; }

        public int Capacity { get; set; }

        public Route()
        {
        }

        public Route(Company company, Airport airport, string routeType, DateTime departureTime, DateTime arrivalTime, int capacity)
        {
            Company = company;
            Airport = airport;
            RouteType = routeType;
            DepartureTime = departureTime;
            ArrivalTime = arrivalTime;
            Capacity = capacity;
        }

        public Route(int routeId, Company company, Airport airport, string routeType, DateTime departureTime, DateTime arrivalTime, int capacity)
        {
            RouteId = routeId;
            Company = company;
            Airport = airport;
            RouteType = routeType;
            DepartureTime = departureTime;
            ArrivalTime = arrivalTime;
            Capacity = capacity;
        }
    }
}
