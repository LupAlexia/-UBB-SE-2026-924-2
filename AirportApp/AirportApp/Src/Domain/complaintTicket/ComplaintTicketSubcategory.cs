using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportApp.Src.Model.Ticket
{
    public class ComplaintTicketSubcategory
    {
        public int SubcategoryId { get; }
        public string SubcategoryName { get; }
        public int SubcategoryExternalReferenceId { get; }
        public ComplaintTicketCategory ParentCategory { get; }

        public ComplaintTicketSubcategory(int subcategoryId, string subcategoryName, int externalId, ComplaintTicketCategory parentCategory)
        {
            SubcategoryId = subcategoryId;
            SubcategoryName = subcategoryName;
            SubcategoryExternalReferenceId = externalId;
            ParentCategory = parentCategory;
        }
    }
}