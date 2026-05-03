namespace AirportApp.ClassLibrary.Entity.Domain
{
    public class MembershipAddonDiscount
    {
        public int MembershipId { get; set; }
        public Membership Membership { get; set; } = null!;

        public int AddOnId { get; set; }
        public AddOn AddOn { get; set; } = null!;
        public float DiscountPercentage { get; set; }

        public MembershipAddonDiscount()
        {
        }

        public MembershipAddonDiscount(Membership membership, AddOn addOn, float discountPercentage)
        {
            Membership = membership;
            MembershipId = membership.Id;
            AddOn = addOn;
            AddOnId = addOn.Id;
            DiscountPercentage = discountPercentage;
        }
    }
}
