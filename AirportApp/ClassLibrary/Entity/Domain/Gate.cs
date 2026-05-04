using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    [Table("Gates")]
    public class Gate
    {
        [Key]
        [Column("Gate_Id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(10)]
        [Column("Gate_Name")]
        public string GateName { get; set; } = string.Empty;

        [Required]
        [Column("Airport_Id")]
        public int AirportId { get; set; }

        [ForeignKey(nameof(AirportId))]
        public Airport Airport { get; set; } = null!;

        public Gate()
        {
        }

        public Gate(string gateName)
        {
            this.GateName = gateName;
        }

        public Gate(int gateId, string gateName)
        {
            this.Id = gateId;
            this.GateName = gateName;
        }

        public Gate(int id, string gateName, Airport airport)
        {
            Id = id;
            GateName = gateName;
            Airport = airport;
            AirportId = airport.Id;
        }
    }
}
