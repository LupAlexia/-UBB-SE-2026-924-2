namespace AirportApp.Src.Domain
{
    public class Airport
    {
        public int AirportId { get; set; }

        public string? AirportCode { get; set; }

        public string? City { get; set; }

        public Airport()
        {
        }

        public Airport(string airportCode, string city)
        {
            this.AirportCode = airportCode;
            this.City = city;
        }

        public Airport(int airportId, string airportCode, string city)
        {
            this.AirportId = airportId;
            this.AirportCode = airportCode;
            this.City = city;
        }
    }
}
