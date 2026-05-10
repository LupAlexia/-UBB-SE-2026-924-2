using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    [Table("Membership_Addon_Discounts")]
    public class MembershipAddonDiscount
    {
        public Membership Membership { get; set; } = null!;

        public AddOn AddOn { get; set; } = null!;

        [Required]
        [Range(0, 100)]
        [Column("Discount_Percentage")]
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
