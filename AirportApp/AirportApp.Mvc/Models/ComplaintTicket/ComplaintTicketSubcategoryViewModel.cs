namespace AirportApp.Mvc.Models.ComplaintTicket
{
    public class ComplaintTicketSubcategoryViewModel
    {
        public int Id { get; set; }

        public string SubcategoryName { get; set; } = string.Empty;

        public int ParentCategoryId { get; set; }

        public string ParentCategoryName { get; set; } = string.Empty;
    }
}
