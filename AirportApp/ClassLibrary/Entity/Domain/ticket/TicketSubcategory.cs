using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportApp.ClassLibrary.Entity.Domain.Ticket
{
    [Table("TicketSubcategories")]
    public class TicketSubcategory
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

        [Required]
        [Column("Parent_Category_Id")]
        public int ParentCategoryId { get; set; }

        [ForeignKey(nameof(ParentCategoryId))]
        public TicketCategory ParentCategory { get; set; } = null!;

        public TicketSubcategory() { }

        public TicketSubcategory(int subcategoryId, string subcategoryName, int externalId, TicketCategory parentCategory)
        {
            Id = subcategoryId;
            SubcategoryName = subcategoryName;
            SubcategoryExternalReferenceId = externalId;
            ParentCategory = parentCategory;
            ParentCategoryId = parentCategory.Id;
        }
    }
}