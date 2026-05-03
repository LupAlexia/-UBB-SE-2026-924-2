using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    [Table("Membership_Addon_Discounts")]
    public class MembershipAddonDiscount
    {

        [Key, Column(Order = 0)]
        [ForeignKey(nameof(Membership))]
        public int MembershipId { get; set; }
        public Membership Membership { get; set; } = null!;

        [Key, Column(Order = 1)]
        [ForeignKey(nameof(AddOn))]
        public int AddOnId { get; set; }
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
            MembershipId = membership.Id;
            AddOn = addOn;
            AddOnId = addOn.Id;
            DiscountPercentage = discountPercentage;
        }
    }
}
