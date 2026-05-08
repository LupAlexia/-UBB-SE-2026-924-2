using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Src.Service.Interfaces
{
    public interface IFAQService
    {
        Task<List<FAQEntry>> GetAllAsync();
        Task<List<FAQEntry>> GetByCategoryAsync(FAQCategoryEnum category);
        Task AddFAQEntryAsync(FAQEntry newElem);
        Task EditFAQEntryAsync(FAQEntry tempEntry, int faqEntryId);
        Task DeleteFAQEntryAsync(int entryId);
        Task IncrementViewCountAsync(FAQEntry entry);
        Task IncrementWasHelpfulVotesAsync(FAQEntry entry);
        Task IncrementWasNotHelpfulVotesAsync(FAQEntry entry);

        Task<List<FAQEntry>> FilterFAQEntryAsync(FAQCategoryEnum category, string searchQuery);
    }
}
