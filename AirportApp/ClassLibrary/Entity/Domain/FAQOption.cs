using System.ComponentModel.DataAnnotations.Schema;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    public class FAQOption
    {
        public int OptionId { get; set; }
        public string Label { get; set; } = string.Empty;
        // They might not have a next option, meaning the end of the chat
        public FAQNode? NextOption { get; set; }

        public FAQOption()
        {
        }

        public FAQOption(string label, FAQNode? nextOption)
        {
            this.Label = label;
            this.NextOption = nextOption;
        }
    }
}
