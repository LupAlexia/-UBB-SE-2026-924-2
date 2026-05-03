using System.Collections.Generic;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    public class FlightTicket
    {
        public int TicketId { get; set; }
        public Customer? User { get; set; }
        public Flight? Flight { get; set; }
        public string? Seat { get; set; }
        public float Price { get; set; }
        public string? Status { get; set; }
        public string? PassengerFirstName { get; set; }
        public string? PassengerLastName { get; set; }
        public string? PassengerEmail { get; set; }
        public string? PassengerPhone { get; set; }
        public List<AddOn> SelectedAddOns { get; set; } = new List<AddOn>();

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
            TicketId = ticketId;
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
    }
}