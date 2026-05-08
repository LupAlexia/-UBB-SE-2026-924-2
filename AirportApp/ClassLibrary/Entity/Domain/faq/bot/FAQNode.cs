namespace AirportApp.ClassLibrary.Entity.Domain.Faq.Bot
{
    public class FAQNode
    {
        public int FaqNodeId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public List<FAQOption> Options { get; set; } = new ();
        public bool IsFinalAnswer { get; set; }

        public FAQNode()
        {
        }

        public FAQNode(int faqNodeId, string questionText, IEnumerable<FAQOption> options, bool isFinalAnswer)
        {
            this.FaqNodeId = faqNodeId;
            this.QuestionText = questionText;
            this.Options = options.ToList();
            this.IsFinalAnswer = isFinalAnswer;
        }
    }
}
