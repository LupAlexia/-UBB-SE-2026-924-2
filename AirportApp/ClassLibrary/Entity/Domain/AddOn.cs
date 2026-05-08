using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    [Table("AddOns")]
    public class AddOn
    {
        [Key]
        [Column("AddOn_Id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column("Base_Price")]
        public float BasePrice { get; set; }

        public ICollection<FlightTicket> Tickets { get; set; } = new List<FlightTicket>();
        public AddOn()
        {
        }

        public AddOn(string name, float basePrice)
        {
            Name = name;
            BasePrice = basePrice;
        }

        public AddOn(int addOnId, string name, float basePrice)
        {
            Id = addOnId;
            Name = name;
            BasePrice = basePrice;
        }
    }
}
