using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    [Table("FlightTickets")]
    public class FlightTicket
    {
        [Key]
        [Column("Ticket_Id")]
        public int Id { get; set; }

        [Required]
        [Column("User_Id")]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public Customer User { get; set; } = null!;

        [Required]
        [Column("Flight_Id")]
        public int FlightId { get; set; }

        [ForeignKey(nameof(FlightId))]
        public Flight Flight { get; set; } = null!;

        [Required]
        [MaxLength(10)]
        [Column("Seat")]
        public string Seat { get; set; } = string.Empty;

        [Required]
        [Column("Price")]
        public float Price { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("Status")]
        public string Status { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        [Column("Passenger_First_Name")]
        public string PassengerFirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        [Column("Passenger_Last_Name")]
        public string PassengerLastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        [Column("Passenger_Email")]
        public string PassengerEmail { get; set; } = string.Empty;

        [MaxLength(20)]
        [Column("Passenger_Phone")]
        public string PassengerPhone { get; set; } = string.Empty;
        public ICollection<AddOn> SelectedAddOns { get; set; } = new List<AddOn>();
        public FlightTicket()
        {
        }

        public FlightTicket(Customer user, Flight flight, string seat, float price, string status, string passengerFirstName, string passengerLastName, string passengerEmail, string passengerPhone)
        {
            User = user;
            UserId = user.Id;
            Flight = flight;
            FlightId = flight.Id;
            Seat = seat;
            Price = price;
            Status = status;
            PassengerFirstName = passengerFirstName;
            PassengerLastName = passengerLastName;
            PassengerEmail = passengerEmail;
            PassengerPhone = passengerPhone;
        }

        public FlightTicket(int ticketId, Customer user, Flight flight, string seat, float price, string status, string passengerFirstName, string passengerLastName, string passengerEmail, string passengerPhone)
        {
            Id = ticketId;
            User = user;
            UserId = user.Id;
            Flight = flight;
            FlightId = flight.Id;
            Seat = seat;
            Price = price;
            Status = status;
            PassengerFirstName = passengerFirstName;
            PassengerLastName = passengerLastName;
            PassengerEmail = passengerEmail;
            PassengerPhone = passengerPhone;
        }
    }
}