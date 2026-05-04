using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Faq;

namespace AirportApp.Src.Service.Interfaces
{
    public interface IFAQService
    {
        List<FAQEntry> GetAll();
        List<FAQEntry> GetByCategory(FAQCategoryEnum category);
        void AddFAQEntry(FAQEntry newElem);
        void EditFAQEntry(FAQEntry tempEntry, int faqEntryId);
        void DeleteFAQEntry(int entryId);
        void IncrementViewCount(FAQEntry entry);
        void IncrementWasHelpfulVotes(FAQEntry entry);
        void IncrementWasNotHelpfulVotes(FAQEntry entry);

        List<FAQEntry> FilterFAQEntry(FAQCategoryEnum category, string searchQury);
    }
}
