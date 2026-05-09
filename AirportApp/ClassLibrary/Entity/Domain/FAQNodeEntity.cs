using System.Collections.Generic;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    public class FAQNodeEntity
    {
        public int NodeId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public bool IsFinalAnswer { get; set; }

        public ICollection<FAQOptionEntity> Options { get; set; } = new List<FAQOptionEntity>();
    }
}
