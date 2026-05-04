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

        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; } = null!;

        [Required]
        [Column("Company_Id")]
        public int CompanyId { get; set; }

        [ForeignKey(nameof(AirportId))]
        public Airport Airport { get; set; } = null!;

        [Required]
        [Column("Airport_Id")]
        public int AirportId { get; set; }

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
            CompanyId = company.Id;
            Airport = airport;
            AirportId = airport.Id;
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
