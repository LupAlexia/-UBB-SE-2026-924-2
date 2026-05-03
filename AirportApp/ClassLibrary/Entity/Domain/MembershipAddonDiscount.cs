namespace AirportApp.ClassLibrary.Entity.Domain
{
    public class MembershipAddonDiscount
    {
        public Membership Membership { get; set; } = new Membership();
        public AddOn AddOn { get; set; } = new AddOn();
        public float DiscountPercentage { get; set; }

        public MembershipAddonDiscount()
        {
        }

        public MembershipAddonDiscount(Membership membership, AddOn addOn, float discountPercentage)
        {
            Membership = membership;
            AddOn = addOn;
            DiscountPercentage = discountPercentage;
        }
    }
}
