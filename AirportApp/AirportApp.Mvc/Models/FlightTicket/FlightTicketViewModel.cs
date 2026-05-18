namespace AirportApp.Mvc.Models.FlightTicket
{
    public class FlightTicketViewModel
    {
        public int Id { get; set; }

        public string Seat { get; set; } = string.Empty;

        public float Price { get; set; }

        public string Status { get; set; } = string.Empty;

        public string PassengerFirstName { get; set; } = string.Empty;

        public string PassengerLastName { get; set; } = string.Empty;

        public string PassengerEmail { get; set; } = string.Empty;

        public string PassengerPhone { get; set; } = string.Empty;

        public int FlightId { get; set; }

        public string FlightNumber { get; set; } = string.Empty;

        public int UserId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string PassengerFullName => $"{PassengerFirstName} {PassengerLastName}";
    }
}
