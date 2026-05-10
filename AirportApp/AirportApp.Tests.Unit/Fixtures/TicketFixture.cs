using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Tests.Unit.Fixtures;

public static class TicketFixture
{
    public static FlightTicket CreateValidTestTicket(
        Customer user,
        Flight flight,
        string seat = "12A",
        float price = 150.0f,
        string status = "Confirmed")
    {
        return new FlightTicket
        {
            Id = 1,
            User = user,
            Flight = flight,
            Seat = seat,
            Price = price,
            Status = status,
            PassengerFirstName = "Andrei",
            PassengerLastName = "Ionescu",
            PassengerEmail = "andrei.ionescu@gmail.com",
            PassengerPhone = "0722112233"
        };
    }

    public static FlightTicket CreateConfirmedTicket(Customer user, Flight flight)
    {
        return CreateValidTestTicket(user, flight, status: "Confirmed");
    }

    public static FlightTicket CreatePendingTicket(Customer user, Flight flight)
    {
        return CreateValidTestTicket(user, flight, status: "Pending");
    }
}
