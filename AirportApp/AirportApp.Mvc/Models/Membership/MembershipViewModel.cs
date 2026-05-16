namespace AirportApp.Mvc.Models.Membership
{
    public class MembershipViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public float FlightDiscountPercentage { get; set; }

        public List<MembershipAddonDiscountViewModel> AddonDiscounts { get; set; } = new List<MembershipAddonDiscountViewModel>();
    }
}
