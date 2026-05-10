using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository
{
    public class FAQRepository : IFAQRepository
    {
        private readonly AirportDbContext databaseContext;

        public FAQRepository(AirportDbContext databaseContext)
        {
            this.databaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
        }

        public async Task<FAQEntry> GetByIdAsync(int askedQuestionId)
        {
            var faqEntry = await this.databaseContext.Faqs.FirstOrDefaultAsync(faqEntry => faqEntry.Id == askedQuestionId);
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

            this.databaseContext.Faqs.Add(questionEntity);
            await this.databaseContext.SaveChangesAsync();
            return questionEntity.Id;
        }

        public async Task UpdateByIdAsync(int identificationNumber, FAQEntry questionEntity)
        {
            if (questionEntity == null)
            {
                throw new ArgumentNullException(nameof(questionEntity), "FAQ entry cannot be null.");
            }

            var faqEntry = await this.databaseContext.Faqs.FirstOrDefaultAsync(faqEntry => faqEntry.Id == identificationNumber);
            if (faqEntry != null)
            {
                faqEntry.Question = questionEntity.Question;
                faqEntry.Answer = questionEntity.Answer;
                faqEntry.Category = questionEntity.Category;
                faqEntry.ViewCount = questionEntity.ViewCount;
                faqEntry.HelpfulVotesCount = questionEntity.HelpfulVotesCount;
                faqEntry.NotHelpfulVotesCount = questionEntity.NotHelpfulVotesCount;
                await this.databaseContext.SaveChangesAsync();
            }
        }

        public async Task DeleteByIdAsync(int identificationNumber)
        {
            var faqEntry = await this.databaseContext.Faqs.FirstOrDefaultAsync(faqEntity => faqEntity.Id == identificationNumber);
            if (faqEntry != null)
            {
                this.databaseContext.Faqs.Remove(faqEntry);
                await this.databaseContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<FAQEntry>> GetAllAsync()
        {
            return await this.databaseContext.Faqs.ToListAsync();
        }

        public async Task<List<FAQEntry>> GetByCategoryAsync(FAQCategoryEnum category)
        {
            if (category == FAQCategoryEnum.All)
            {
                return await this.databaseContext.Faqs.ToListAsync();
            }

            return await this.databaseContext.Faqs
                .Where(faqEntity => faqEntity.Category == category)
                .ToListAsync();
        }

        public async Task IncrementViewCountAsync(int identificationNumber)
        {
            var faqEntry = await this.databaseContext.Faqs.FirstOrDefaultAsync(faqEntity => faqEntity.Id == identificationNumber);
            if (faqEntry != null)
            {
                faqEntry.ViewCount++;
                await this.databaseContext.SaveChangesAsync();
            }
        }

        public async Task IncrementWasHelpfulVotesAsync(int identificationNumber)
        {
            var faqEntry = await this.databaseContext.Faqs.FirstOrDefaultAsync(faqEntity => faqEntity.Id == identificationNumber);
            if (faqEntry != null)
            {
                faqEntry.HelpfulVotesCount++;
                await this.databaseContext.SaveChangesAsync();
            }
        }

        public async Task IncrementWasNotHelpfulVotesAsync(int identificationNumber)
        {
            var faqEntry = await this.databaseContext.Faqs.FirstOrDefaultAsync(faqEntity => faqEntity.Id == identificationNumber);
            if (faqEntry != null)
            {
                faqEntry.NotHelpfulVotesCount++;
                await this.databaseContext.SaveChangesAsync();
            }
        }
    }
}