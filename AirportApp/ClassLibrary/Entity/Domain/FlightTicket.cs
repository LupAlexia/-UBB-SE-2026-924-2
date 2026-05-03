using System.Collections.Generic;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    public class FlightTicket
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public Customer User { get; set; } = null!;

        public int FlightId { get; set; }
        public Flight Flight { get; set; } = null!;
        public string Seat { get; set; } = string.Empty;
        public float Price { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PassengerFirstName { get; set; } = string.Empty;
        public string PassengerLastName { get; set; } = string.Empty;
        public string PassengerEmail { get; set; } = string.Empty;
        public string PassengerPhone { get; set; } = string.Empty;
        public List<AddOn> SelectedAddOns { get; set; } = new();

        public FlightTicket()
        {
        }

        public FlightTicket(Customer user, Flight flight, string seat, float price, string status, string passengerFirstName, string passengerLastName, string passengerEmail, string passengerPhone)
        {
            User = user;
            Flight = flight;
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