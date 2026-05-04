using System;
using System.Collections.Generic;
using System.Linq;
using AirportApp.ClassLibrary.Entity.Domain.Faq;
using AirportApp.Src.Service.Interfaces;
using Sprache;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.Src.Service.Implementation
{
	public class FAQService : IFAQService
	{
		private readonly IFAQRepository faqRepository;

		public FAQService(IFAQRepository faqRepository)
		{
			this.faqRepository = faqRepository;
		}

		public List<FAQEntry> GetAll()
		{
			return faqRepository.GetAll().ToList();
		}

        public List<FAQEntry> GetByCategory(FAQCategoryEnum category)
        {
			return faqRepository.GetByCategory(category);
        }

		public void AddFAQEntry(FAQEntry newElem)
		{
			faqRepository.CreateNewEntity(newElem);
		}

		public void EditFAQEntry(FAQEntry tempEntry, int faqEntryId)
		{
			faqRepository.UpdateById(faqEntryId, tempEntry);
		}

		public void DeleteFAQEntry(int entryId)
		{
			faqRepository.DeleteById(entryId);
		}

		public void IncrementViewCount(FAQEntry entry)
		{
			faqRepository.IncrementViewCount(entry.Id);
		}

		public void IncrementWasHelpfulVotes(FAQEntry entry)
		{
			faqRepository.IncrementWasHelpfulVotes(entry.Id);
		}

        public void IncrementWasNotHelpfulVotes(FAQEntry entry)
        {
			faqRepository.IncrementWasNotHelpfulVotes(entry.Id);
        }

		public List<FAQEntry> FilterFAQEntry(FAQCategoryEnum category, string searchQuery)
		{
			var frequentlyAskedQuestions = this.faqRepository.GetAll().AsEnumerable();
			if (category != FAQCategoryEnum.All)
			{
				frequentlyAskedQuestions = this.GetByCategory(category);
			}

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                frequentlyAskedQuestions = frequentlyAskedQuestions.Where(question =>
                    (question.Question?.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (question.Answer?.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ?? false));
            }
			return frequentlyAskedQuestions.ToList();
        }
    }
}