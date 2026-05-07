using System.ComponentModel.DataAnnotations.Schema;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    [NotMapped]
    public class PriceBreakdown
    {
        public float BasePricePerPerson { get; set; }
        public float BasePriceTotal { get; set; }
        public float AddOnsTotal { get; set; }
        public float MembershipSavings { get; set; }
        public float FinalTotal { get; set; }
    }
}
