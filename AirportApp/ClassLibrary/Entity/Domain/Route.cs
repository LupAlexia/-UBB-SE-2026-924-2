using System;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    public class Route
    {
        public int Id { get; set; }

        public Company Company { get; set; } = null!;
        public int CompanyId { get; set; }


        public Airport? Airport { get; set; } = null!;
        public int AirportId { get; set; }

        public string RouteType { get; set; } = string.Empty;

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
            Id = routeId;
            Company = company;
            CompanyId = company.Id;
            Airport = airport;
            AirportId = airport.Id;
            RouteType = routeType;
            DepartureTime = departureTime;
            ArrivalTime = arrivalTime;
            Capacity = capacity;
        }
    }
}
