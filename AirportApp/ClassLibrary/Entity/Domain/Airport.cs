namespace AirportApp.ClassLibrary.Entity.Domain
{
    public class Airport
    {
        public int Id { get; set; }

        public string AirportCode { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public List<Gate> Gates { get; set; } = new();

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
            this.Id = airportId;
            this.AirportCode = airportCode;
            this.City = city;
        }
    }
}
