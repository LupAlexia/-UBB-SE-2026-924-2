using System.Collections.Generic;

namespace AirportApp.Src.Domain
{
    public class Membership
    {
        public int MembershipId { get; set; }
        public string Name { get; set; } = string.Empty;
        public float FlightDiscountPercentage { get; set; }
        public List<MembershipAddonDiscount> AddonDiscounts { get; set; } = new List<MembershipAddonDiscount>();

        public Membership()
        {
        }

        public Membership(string name, float flightDiscountPercentage)
        {
            Name = name;
            FlightDiscountPercentage = flightDiscountPercentage;
        }

        public Membership(int membershipId, string name, float flightDiscountPercentage)
        {
            MembershipId = membershipId;
            Name = name;
            FlightDiscountPercentage = flightDiscountPercentage;
        }
    }
}
