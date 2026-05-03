namespace AirportApp.ClassLibrary.Entity.Domain
{
    public class Gate
    {
        public int Id { get; set; }

        public string GateName { get; set; } = string.Empty;

        public int AirportId { get; set; }
        public Airport Airport { get; set; } = null!;

        public Gate()
        {
        }

        public Gate(string gateName)
        {
            this.GateName = gateName;
        }

        public Gate(int gateId, string gateName)
        {
            this.Id = gateId;
            this.GateName = gateName;
        }

        public Gate(int id, string gateName, Airport airport)
        {
            Id = id;
            GateName = gateName;
            Airport = airport;
            AirportId = airport.Id;
        }
    }
}
