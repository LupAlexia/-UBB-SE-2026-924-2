using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.Src.Model.Faq;

namespace AirportApp.Src.Repository.Interfaces;

public interface IFAQRepository : IRepository<int, FAQEntry>
{
    List<FAQEntry> GetByCategory(FAQCategoryEnum category);
    void IncrementViewCount(int identificationNumber);
    void IncrementWasHelpfulVotes(int identificationNumber);
    void IncrementWasNotHelpfulVotes(int identificationNumber);
}
