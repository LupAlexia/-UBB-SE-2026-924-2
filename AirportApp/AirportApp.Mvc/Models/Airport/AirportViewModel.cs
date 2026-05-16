namespace AirportApp.Mvc.Models.Airport
{
    public class AirportViewModel
    {
        public int Id { get; set; }

        public string AirportCode { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public int GateCount { get; set; }
    }
}
