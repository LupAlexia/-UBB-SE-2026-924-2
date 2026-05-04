using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    [Table("Companies")]
    public class Company
    {
        [Key]
        [Column("Company_Id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("Name")]
        public string Name { get; set; } = string.Empty;

        public Company()
        {
        }

        public Company(string name)
        {
            Name = name;
        }

        public Company(int companyId, string name)
        {
            Id = companyId;
            Name = name;
        }
    }
}
