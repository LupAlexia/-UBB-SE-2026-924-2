namespace AirportApp.Src.Domain
{
    public class PriceBreakdown
    {
        public float BasePricePerPerson { get; set; }
        public float BasePriceTotal { get; set; }
        public float AddOnsTotal { get; set; }
        public float MembershipSavings { get; set; }
        public float FinalTotal { get; set; }
    }
}
