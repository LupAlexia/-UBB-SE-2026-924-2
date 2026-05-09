using System.Collections.Generic;
using System.Linq;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    public class FAQNode
    {
        public int NodeId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public ICollection<FAQOption> Options { get; set; } = new List<FAQOption>();
        public bool IsFinalAnswer { get; set; }

        public FAQNode()
        {
        }

        public FAQNode(int nodeId, string questionText, IEnumerable<FAQOption> options, bool isFinalAnswer)
        {
            this.NodeId = nodeId;
            this.QuestionText = questionText;
            this.Options = options.ToList();
            this.IsFinalAnswer = isFinalAnswer;
        }
    }
}
