using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    [Table("Flights")]
    public class Flight
    {
        [Key]
        [Column("Flight_Id")]
        public int Id { get; set; }

        [Required]
        [Column("Route_Id")]
        public int RouteId { get; set; }

        [ForeignKey(nameof(RouteId))]
        public Route Route { get; set; } = null!;

        [Required]
        [Column("Gate_Id")]
        public int GateId { get; set; }

        [ForeignKey(nameof(GateId))]
        public Gate Gate { get; set; } = null!;

        [Required]
        [Column("Departure_Date")]
        public DateTime Date { get; set; }

        [Required]
        [MaxLength(10)]
        [Column("Flight_Number")]
        public string FlightNumber { get; set; } = string.Empty;

        public Flight()
        {
        }

        public Flight(Route route, Gate gate, DateTime date, string flightNumber)
        {
            Route = route;
            RouteId = route.Id;
            Gate = gate;
            GateId = gate.Id;
            Date = date;
            FlightNumber = flightNumber;
        }

        public Flight(int flightId, Route route, Gate gate, DateTime date, string flightNumber)
        {
            Id = flightId;
            Route = route;
            RouteId = route.Id;
            Gate = gate;
            GateId = gate.Id;
            Date = date;
            FlightNumber = flightNumber;
        }
    }
}
