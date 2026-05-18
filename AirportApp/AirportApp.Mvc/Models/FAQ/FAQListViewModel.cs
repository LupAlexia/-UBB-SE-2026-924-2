using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Mvc.Models.FAQ
{
    public class FAQListViewModel
    {
        public List<FAQEntryViewModel> FAQEntries { get; set; } = new List<FAQEntryViewModel>();

        public FAQCategoryEnum? SelectedCategory { get; set; }

        public string? SearchQuery { get; set; }
    }
}
