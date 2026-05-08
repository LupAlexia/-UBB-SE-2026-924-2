using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    [Table("Memberships")]
    public class Membership
    {
        [Key]
        [Column("Membership_Id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0, 100)]
        [Column("Flight_Discount_Percentage")]
        public float FlightDiscountPercentage { get; set; }
        public ICollection<MembershipAddonDiscount> AddonDiscounts { get; set; } = new List<MembershipAddonDiscount>();
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
