namespace AirportApp.ClassLibrary.Entity.Domain
{
    public class Gate
    {
        public int GateId { get; set; }

        public string? GateName { get; set; }

        public Gate()
        {
        }

        public Gate(string gateName)
        {
            this.GateName = gateName;
        }

        public Gate(int gateId, string gateName)
        {
            this.GateId = gateId;
            this.GateName = gateName;
        }
    }
}
