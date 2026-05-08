using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    [Table("FAQs")]
    public class FAQEntry
    {
        [Key]
        [Column("FAQ_Id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        [Column("Question_Text")]
        public string Question { get; set; } = string.Empty;

        [Required]
        [Column("Answer_Text", TypeName = "NVARCHAR(MAX)")]
        public string Answer { get; set; } = string.Empty;
        [Required]
        [Column("Category")]
        public FAQCategoryEnum Category { get; set; }

        [Column("View_Count")]
        public int ViewCount { get; set; }

        [Column("Helpful_Votes")]
        public int HelpfulVotesCount { get; set; }

        [Column("Not_Helpful_Votes")]
        public int NotHelpfulVotesCount { get; set; }

        // 2. Required Parameterless Constructor
        public FAQEntry()
        {
        }

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