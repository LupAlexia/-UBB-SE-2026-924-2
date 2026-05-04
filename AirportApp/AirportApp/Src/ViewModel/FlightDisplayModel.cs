using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Src.ViewModel
{
    public class FlightDisplayModel
    {
        public Flight Flight { get; }
        public string FlightNumber { get; set; } = string.Empty;
        public string RouteCity { get; set; } = string.Empty;
        public string DisplayDate { get; set; } = string.Empty;
        public string DisplayPrice { get; set; } = string.Empty;

        public FlightDisplayModel(Flight flight, float basePrice)
        {
            this.Flight = flight;
            this.FlightNumber = flight.FlightNumber ?? string.Empty;
            this.RouteCity = flight.Route?.Airport?.City ?? "Unknown";
            this.DisplayDate = flight.Date.ToString("g");
            this.DisplayPrice = $"{basePrice:0.00} € / person";
        }
    }
}
