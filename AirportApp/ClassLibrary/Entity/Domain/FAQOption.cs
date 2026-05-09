using System.ComponentModel.DataAnnotations.Schema;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    public class FAQOption
    {
        public int NodeId { get; set; }
        public string Label { get; set; } = string.Empty;
        // They might not have a next option, meaning the end of the chat
        public FAQNode? NextOption { get; set; }

        public FAQOption()
        {
        }

        public FAQOption(string label, int nextOptionId)
        {
            this.Label = label;
            this.NextOption = nextOptionId == 0 ? null : new FAQNode { NodeId = nextOptionId };
        }

        public FAQOption(string label, FAQNode? nextOption)
        {
            this.Label = label;
            this.NextOption = nextOption;
        }
    }
}
