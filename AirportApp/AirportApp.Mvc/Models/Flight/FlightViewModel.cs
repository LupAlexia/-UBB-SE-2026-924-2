namespace AirportApp.Mvc.Models.Flight
{
    public class FlightViewModel
    {
        public int Id { get; set; }

        public string FlightNumber { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public int RouteId { get; set; }

        public string RouteType { get; set; } = string.Empty;

        public int GateId { get; set; }

        public string GateName { get; set; } = string.Empty;

        public string CompanyName { get; set; } = string.Empty;

        public string AirportCode { get; set; } = string.Empty;
    }
}
