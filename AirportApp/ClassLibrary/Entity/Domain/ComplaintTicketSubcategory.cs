using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    [Table("TicketSubcategories")]
    public class ComplaintTicketSubcategory
    {
        [Key]
        [Column("Subcategory_Id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("Subcategory_Name")]
        public string SubcategoryName { get; set; } = string.Empty;

        [Column("External_Reference_Id")]
        public int SubcategoryExternalReferenceId { get; set; }

        public ComplaintTicketCategory ParentCategory { get; set; } = null!;

        public ComplaintTicketSubcategory()
        {
        }

        public ComplaintTicketSubcategory(int subcategoryId, string subcategoryName, int externalId, ComplaintTicketCategory parentCategory)
        {
            Id = subcategoryId;
            SubcategoryName = subcategoryName;
            SubcategoryExternalReferenceId = externalId;
            ParentCategory = parentCategory;
        }
    }
}