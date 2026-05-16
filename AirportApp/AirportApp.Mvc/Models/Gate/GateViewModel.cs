namespace AirportApp.Mvc.Models.Gate
{
    public class GateViewModel
    {
        public int Id { get; set; }

        public string GateName { get; set; } = string.Empty;

        public int AirportId { get; set; }

        public string AirportCode { get; set; } = string.Empty;
    }
}
