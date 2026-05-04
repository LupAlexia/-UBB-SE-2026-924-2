using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    [Table("Airports")]
    public class Airport
    {
        [Key]
        [Column("Airport_Id")]
        public int Id { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)] //codes are strictly 3 characters
        [Column("Airport_Code")]
        public string AirportCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("City")]
        public string City { get; set; } = string.Empty;

        public ICollection<Gate> Gates { get; set; } = new List<Gate>();
        public Airport()
        {
        }

        public Airport(string airportCode, string city)
        {
            this.AirportCode = airportCode;
            this.City = city;
        }

        public Airport(int airportId, string airportCode, string city)
        {
            this.Id = airportId;
            this.AirportCode = airportCode;
            this.City = city;
        }
    }
}
