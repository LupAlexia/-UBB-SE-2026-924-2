using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Service.Interfaces;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Src.Service.Implementation
{
    public class FAQService : IFAQService
    {
        private readonly IFAQRepository faqRepository;

        public FAQService(IFAQRepository faqRepository)
        {
            this.faqRepository = faqRepository;
        }

        public async Task<List<FAQEntry>> GetAllAsync()
        {
            return (await faqRepository.GetAllAsync()).ToList();
        }

        public async Task<List<FAQEntry>> GetByCategoryAsync(FAQCategoryEnum category)
        {
            return await faqRepository.GetByCategoryAsync(category);
        }

        public async Task AddFAQEntryAsync(FAQEntry newElem)
        {
            await faqRepository.CreateNewEntityAsync(newElem);
        }

        public async Task EditFAQEntryAsync(FAQEntry tempEntry, int faqEntryId)
        {
            await faqRepository.UpdateByIdAsync(faqEntryId, tempEntry);
        }

        public async Task DeleteFAQEntryAsync(int entryId)
        {
            await faqRepository.DeleteByIdAsync(entryId);
        }

        public async Task IncrementViewCountAsync(FAQEntry entry)
        {
            await faqRepository.IncrementViewCountAsync(entry.Id);
        }

        public async Task IncrementWasHelpfulVotesAsync(FAQEntry entry)
        {
            await faqRepository.IncrementWasHelpfulVotesAsync(entry.Id);
        }

        public async Task IncrementWasNotHelpfulVotesAsync(FAQEntry entry)
        {
            await faqRepository.IncrementWasNotHelpfulVotesAsync(entry.Id);
        }

        public async Task<List<FAQEntry>> FilterFAQEntryAsync(FAQCategoryEnum category, string searchQuery)
        {
            IEnumerable<FAQEntry> frequentlyAskedQuestions;

            if (category != FAQCategoryEnum.All)
            {
                frequentlyAskedQuestions = await this.GetByCategoryAsync(category);
            }
            else
            {
                frequentlyAskedQuestions = await this.GetAllAsync();
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