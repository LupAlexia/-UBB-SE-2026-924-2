using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.Entity.Domain.Faq;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.DataAccess;

namespace AirportApp.ClassLibrary.Repository
{
    public class FAQRepository : IFAQRepository
    {
        private readonly AirportDbContext dataBaseContext;

        public FAQRepository(AirportDbContext dataBaseContext)
        {
            this.dataBaseContext = dataBaseContext ?? throw new ArgumentNullException(nameof(dataBaseContext));
        }

        public async Task<FAQEntry> GetByIdAsync(int askedQuestionId)
        {
            var faqEntry = await this.dataBaseContext.faqs.FirstOrDefaultAsync(f => f.Id == askedQuestionId);
            if (faqEntry == null)
            {
                throw new KeyNotFoundException($"FAQ with id {askedQuestionId} was not found.");
            }

            return faqEntry;
        }

        public async Task<int> CreateNewEntityAsync(FAQEntry questionEntity)
        {
            if (questionEntity == null)
            {
                throw new ArgumentNullException(nameof(questionEntity), "FAQ entry cannot be null.");
            }

            this.dataBaseContext.faqs.Add(questionEntity);
            await this.dataBaseContext.SaveChangesAsync();
            return questionEntity.Id;
        }

        public async Task UpdateByIdAsync(int identificationNumber, FAQEntry questionEntity)
        {
            if (questionEntity == null)
            {
                throw new ArgumentNullException(nameof(questionEntity), "FAQ entry cannot be null.");
            }

            var faqEntry = await this.dataBaseContext.faqs.FirstOrDefaultAsync(f => f.Id == identificationNumber);
            if (faqEntry != null)
            {
                faqEntry.Question = questionEntity.Question;
                faqEntry.Answer = questionEntity.Answer;
                faqEntry.Category = questionEntity.Category;
                faqEntry.ViewCount = questionEntity.ViewCount;
                faqEntry.HelpfulVotesCount = questionEntity.HelpfulVotesCount;
                faqEntry.NotHelpfulVotesCount = questionEntity.NotHelpfulVotesCount;
                await this.dataBaseContext.SaveChangesAsync();
            }
        }

        public async Task DeleteByIdAsync(int identificationNumber)
        {
            var faqEntry = await this.dataBaseContext.faqs.FirstOrDefaultAsync(f => f.Id == identificationNumber);
            if (faqEntry != null)
            {
                this.dataBaseContext.faqs.Remove(faqEntry);
                await this.dataBaseContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<FAQEntry>> GetAllAsync()
        {
            return await this.dataBaseContext.faqs.ToListAsync();
        }

        public async Task<List<FAQEntry>> GetByCategoryAsync(FAQCategoryEnum category)
        {
            if (category == FAQCategoryEnum.All)
            {
                return await this.dataBaseContext.faqs.ToListAsync();
            }

            return await this.dataBaseContext.faqs
                .Where(f => f.Category == category)
                .ToListAsync();
        }

        public async Task IncrementViewCountAsync(int identificationNumber)
        {
            var faqEntry = await this.dataBaseContext.faqs.FirstOrDefaultAsync(f => f.Id == identificationNumber);
            if (faqEntry != null)
            {
                faqEntry.ViewCount++;
                await this.dataBaseContext.SaveChangesAsync();
            }
        }

        public async Task IncrementWasHelpfulVotesAsync(int identificationNumber)
        {
            var faqEntry = await this.dataBaseContext.faqs.FirstOrDefaultAsync(f => f.Id == identificationNumber);
            if (faqEntry != null)
            {
                faqEntry.HelpfulVotesCount++;
                await this.dataBaseContext.SaveChangesAsync();
            }
        }

        public async Task IncrementWasNotHelpfulVotesAsync(int identificationNumber)
        {
            var faqEntry = await this.dataBaseContext.faqs.FirstOrDefaultAsync(f => f.Id == identificationNumber);
            if (faqEntry != null)
            {
                faqEntry.NotHelpfulVotesCount++;
                await this.dataBaseContext.SaveChangesAsync();
            }
        }
    }
}