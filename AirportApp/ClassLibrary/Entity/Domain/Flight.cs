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

        public Route Route { get; set; } = null!;

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
            Gate = gate;
            Date = date;
            FlightNumber = flightNumber;
        }

        public Flight(int flightId, Route route, Gate gate, DateTime date, string flightNumber)
        {
            Id = flightId;
            Route = route;
            Gate = gate;
            Date = date;
            FlightNumber = flightNumber;
        }
    }
}
