using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Mvc.Models.FAQ
{
    public class FAQEntryViewModel
    {
        public int Id { get; set; }

        public string Question { get; set; } = string.Empty;

        public string Answer { get; set; } = string.Empty;

        public FAQCategoryEnum Category { get; set; }

        public string CategoryName => Category.ToString();

        public int ViewCount { get; set; }

        public int HelpfulVotesCount { get; set; }

        public int NotHelpfulVotesCount { get; set; }

        public int TotalVotes => HelpfulVotesCount + NotHelpfulVotesCount;

        public double HelpfulPercentage => TotalVotes > 0 ? (double)HelpfulVotesCount / TotalVotes * 100 : 0;
    }
}
