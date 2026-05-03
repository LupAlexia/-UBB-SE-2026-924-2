using System;
using System.Collections.Generic;
using System.Linq;
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

        public FAQEntry GetById(int askedQuestionId)
        {
            var faqEntry = this.dataBaseContext.faqs.FirstOrDefault(f => f.Id == askedQuestionId);
            if (faqEntry == null)
            {
                throw new KeyNotFoundException($"FAQ with id {askedQuestionId} was not found.");
            }

            return faqEntry;
        }

        public int CreateNewEntity(FAQEntry questionEntity)
        {
            if (questionEntity == null)
            {
                throw new ArgumentNullException(nameof(questionEntity), "FAQ entry cannot be null.");
            }

            this.dataBaseContext.faqs.Add(questionEntity);
            this.dataBaseContext.SaveChanges();
            return questionEntity.Id;
        }

        public void UpdateById(int identificationNumber, FAQEntry questionEntity)
        {
            if (questionEntity == null)
            {
                throw new ArgumentNullException(nameof(questionEntity), "FAQ entry cannot be null.");
            }

            var faqEntry = this.dataBaseContext.faqs.FirstOrDefault(f => f.Id == identificationNumber);
            if (faqEntry != null)
            {
                faqEntry.Question = questionEntity.Question;
                faqEntry.Answer = questionEntity.Answer;
                faqEntry.Category = questionEntity.Category;
                faqEntry.ViewCount = questionEntity.ViewCount;
                faqEntry.HelpfulVotesCount = questionEntity.HelpfulVotesCount;
                faqEntry.NotHelpfulVotesCount = questionEntity.NotHelpfulVotesCount;
                this.dataBaseContext.SaveChanges();
            }
        }

        public void DeleteById(int identificationNumber)
        {
            var faqEntry = this.dataBaseContext.faqs.FirstOrDefault(f => f.Id == identificationNumber);
            if (faqEntry != null)
            {
                this.dataBaseContext.faqs.Remove(faqEntry);
                this.dataBaseContext.SaveChanges();
            }
        }

        public IEnumerable<FAQEntry> GetAll()
        {
            return this.dataBaseContext.faqs.ToList();
        }

        public List<FAQEntry> GetByCategory(FAQCategoryEnum category)
        {
            if (category == FAQCategoryEnum.All)
            {
                return this.dataBaseContext.faqs.ToList();
            }

            return this.dataBaseContext.faqs
                .Where(f => f.Category == category)
                .ToList();
        }

        public void IncrementViewCount(int identificationNumber)
        {
            var faqEntry = this.dataBaseContext.faqs.FirstOrDefault(f => f.Id == identificationNumber);
            if (faqEntry != null)
            {
                faqEntry.ViewCount++;
                this.dataBaseContext.SaveChanges();
            }
        }

        public void IncrementWasHelpfulVotes(int identificationNumber)
        {
            var faqEntry = this.dataBaseContext.faqs.FirstOrDefault(f => f.Id == identificationNumber);
            if (faqEntry != null)
            {
                faqEntry.HelpfulVotesCount++;
                this.dataBaseContext.SaveChanges();
            }
        }

        public void IncrementWasNotHelpfulVotes(int identificationNumber)
        {
            var faqEntry = this.dataBaseContext.faqs.FirstOrDefault(f => f.Id == identificationNumber);
            if (faqEntry != null)
            {
                faqEntry.NotHelpfulVotesCount++;
                this.dataBaseContext.SaveChanges();
            }
        }
    }
}
}