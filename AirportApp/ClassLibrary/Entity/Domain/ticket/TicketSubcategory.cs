using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportApp.ClassLibrary.Entity.Domain.Ticket
{
    public class TicketSubcategory
    {
        public int Id { get; set; }
        public string SubcategoryName { get; set; } = string.Empty;
        public int SubcategoryExternalReferenceId { get; set; }

        public int ParentCategoryId { get; set; }
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