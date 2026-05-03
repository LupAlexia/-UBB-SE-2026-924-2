using System.Collections.Generic;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    public class Membership
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public float FlightDiscountPercentage { get; set; }
        public List<MembershipAddonDiscount> AddonDiscounts { get; set; } = new();

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
            Id = membershipId;
            Name = name;
            FlightDiscountPercentage = flightDiscountPercentage;
        }
    }
}
