using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Faq;

namespace AirportApp.ClassLibrary.Repository.Interfaces
{
    public interface IFAQRepository : IRepository<int, FAQEntry>
    {
        Task<List<FAQEntry>> GetByCategoryAsync(FAQCategoryEnum category);

        Task IncrementViewCountAsync(int identificationNumber);

        Task IncrementWasHelpfulVotesAsync(int identificationNumber);

        Task IncrementWasNotHelpfulVotesAsync(int identificationNumber);
    }
}