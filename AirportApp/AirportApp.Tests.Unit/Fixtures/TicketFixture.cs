using TicketManager.Domain;

namespace TicketManager.Tests.Unit.Fixtures;

public static class TicketFixture
{
    public static Ticket CreateValidTestTicket()
    {
        return new Ticket
        {
            TicketId = 1,
            PassengerFirstName = "Bogdan",
            PassengerLastName = "Dragomir",
            PassengerEmail = "bogdan.d@gmail.com",
            PassengerPhone = "0755112233",
            Seat = "10C",
            Price = 120.0f,
            Status = "Active"
        };
    }
}
