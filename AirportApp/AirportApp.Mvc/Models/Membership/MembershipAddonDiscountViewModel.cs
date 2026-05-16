namespace AirportApp.Mvc.Models.Membership
{
    public class MembershipAddonDiscountViewModel
    {
        public int MembershipId { get; set; }

        public string MembershipName { get; set; } = string.Empty;

        public int AddOnId { get; set; }

        public string AddOnName { get; set; } = string.Empty;

        public float DiscountPercentage { get; set; }
    }
}
