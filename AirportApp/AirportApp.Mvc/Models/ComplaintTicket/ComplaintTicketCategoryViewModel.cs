namespace AirportApp.Mvc.Models.ComplaintTicket
{
    public class ComplaintTicketCategoryViewModel
    {
        public int Id { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public string DefaultUrgencyLevel { get; set; } = string.Empty;

        public List<ComplaintTicketSubcategoryViewModel> Subcategories { get; set; } = new List<ComplaintTicketSubcategoryViewModel>();
    }
}
