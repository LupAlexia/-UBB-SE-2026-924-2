using System.Collections.Generic;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    public class FAQOption
    {
        public int NodeId { get; set; }
        public string Label { get; set; } = string.Empty;
        public int NextOptionId { get; set; }

        public FAQOption()
        {
        }

        public FAQOption(string label, int nextOptionId)
        {
            this.Label = label;
            this.NextOptionId = nextOptionId;
        }
    }
}
