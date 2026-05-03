namespace AirportApp.ClassLibrary.Entity.Domain.Faq
{
    public class FAQEntry
    {
        public int Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public FAQCategoryEnum Category { get; set; }
        public int ViewCount { get; set; }
        public int HelpfulVotesCount { get; set; }
        public int NotHelpfulVotesCount { get; set; }

        // 2. Required Parameterless Constructor
        public FAQEntry() { }

        public FAQEntry(int id, string question, string answer, FAQCategoryEnum category, int viewCount, int wasHelpfulVotes, int wasNotHelpfulVotes)
        {
            Id = id;
            Question = question;
            Answer = answer;
            Category = category;
            ViewCount = viewCount;
            HelpfulVotesCount = wasHelpfulVotes;
            NotHelpfulVotesCount = wasNotHelpfulVotes;
        }

        // These methods had 0 references. Incrementing is done directly in the database

        // public void IncrementViewCount()
        // {
        //    ViewCount++;
        // }

        // public void IncrementWasHelpfulVotes()
        // {
        //    HelpfulVotesCount++;
        // }

        // public void IncrementWasNotHelpfulVotes()
        // {
        //  NotHelpfulVotesCount++;
        // }
        public override bool Equals(object? otherObject)
        {
            return otherObject is FAQEntry entry &&
                   Id == entry.Id &&
                   Question == entry.Question &&
                   Answer == entry.Answer &&
                   Category == entry.Category &&
                   ViewCount == entry.ViewCount &&
                   HelpfulVotesCount == entry.HelpfulVotesCount &&
                   NotHelpfulVotesCount == entry.NotHelpfulVotesCount;
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}