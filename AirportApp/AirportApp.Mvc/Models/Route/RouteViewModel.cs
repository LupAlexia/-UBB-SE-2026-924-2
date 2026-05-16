namespace AirportApp.Mvc.Models.Route
{
    public class RouteViewModel
    {
        public int Id { get; set; }

        public string RouteType { get; set; } = string.Empty;

        public DateTime DepartureTime { get; set; }

        public DateTime ArrivalTime { get; set; }

        public int Capacity { get; set; }

        public int CompanyId { get; set; }

        public string CompanyName { get; set; } = string.Empty;

        public int AirportId { get; set; }

        public string AirportCode { get; set; } = string.Empty;
    }
}
