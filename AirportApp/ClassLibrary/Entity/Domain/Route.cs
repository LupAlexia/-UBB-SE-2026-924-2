using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    [Table("Routes")]
    public class Route
    {
        [Key]
        [Column("Route_Id")]
        public int Id { get; set; }
        public Company Company { get; set; } = null!;

        public Airport Airport { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        [Column("Route_Type")]
        public string RouteType { get; set; } = string.Empty;

        [Required]
        [Column("Departure_Time")]
        public DateTime DepartureTime { get; set; }

        [Required]
        [Column("Arrival_Time")]
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
            Airport = airport;
            RouteType = routeType;
            DepartureTime = departureTime;
            ArrivalTime = arrivalTime;
            Capacity = capacity;
        }
    }
}
