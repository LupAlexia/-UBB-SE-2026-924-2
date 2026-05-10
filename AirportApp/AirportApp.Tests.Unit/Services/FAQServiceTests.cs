using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirportApp.Src.Service.Implementation;
using AirportApp.ClassLibrary.Repository.Interfaces;
using NSubstitute;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Tests.Unit.Services
{
    [TestClass]
    public class FAQServiceTests
    {
        private const int FaqId1 = 1;
        private const int FaqId2 = 2;
        private const int FaqId3 = 3;
        private const int ZeroViewCount = 0;
        private const string Question1 = "Q1";
        private const string Question2 = "Q2";
        private const string Question3 = "Q3";
        private const string Answer1 = "A1";
        private const string Answer2 = "A2";
        private const string Answer3 = "A3";
        private const string SearchQueryMatchingQuestion = "Q1";
        private const string SearchQueryMatchingAnswer = "A2";
        private const string SearchQueryNoMatch = "zzznomatch";
        private const string SearchQueryCaseInsensitive = "q1";

        private IFAQRepository faqRepositoryMock;
        private FAQService faqService;

        [TestInitialize]
        public void Setup()
        {
            faqRepositoryMock = Substitute.For<IFAQRepository>();
            faqService = new FAQService(faqRepositoryMock);
        }

        [TestMethod]
        public async Task GetByCategory_WhenCalled_ReturnsFilteredFAQs()
        {
            var faqs = new List<FAQEntry>
            {
                new FAQEntry(FaqId1, Question1, Answer1, FAQCategoryEnum.CheckIn, ZeroViewCount, ZeroViewCount, ZeroViewCount),
                new FAQEntry(FaqId2, Question2, Answer2, FAQCategoryEnum.CheckIn, ZeroViewCount, ZeroViewCount, ZeroViewCount)
            };
            faqRepositoryMock.GetByCategoryAsync(FAQCategoryEnum.CheckIn).Returns(Task.FromResult(faqs));

            var result = (await faqService.GetByCategoryAsync(FAQCategoryEnum.CheckIn)).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(f => f.Category == FAQCategoryEnum.CheckIn));
            await faqRepositoryMock.Received(1).GetByCategoryAsync(FAQCategoryEnum.CheckIn);
        }

        [TestMethod]
        public async Task FilterFAQEntry_WithSpecificCategory_FiltersAndSearches()
        {
            var faqs = new List<FAQEntry>
            {
                new FAQEntry(FaqId1, Question1, Answer1, FAQCategoryEnum.CheckIn, ZeroViewCount, ZeroViewCount, ZeroViewCount),
                new FAQEntry(FaqId2, Question2, Answer2, FAQCategoryEnum.Baggage, ZeroViewCount, ZeroViewCount, ZeroViewCount)
            };
            faqRepositoryMock.GetByCategoryAsync(FAQCategoryEnum.CheckIn).Returns(Task.FromResult(
                faqs.Where(f => f.Category == FAQCategoryEnum.CheckIn).ToList()));

            var result = await faqService.FilterFAQEntryAsync(FAQCategoryEnum.CheckIn, string.Empty);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(Question1, result[0].Question);
        }

        [TestMethod]
        public async Task FilterFAQEntry_WithAllCategory_ReturnsAllEntries()
        {
            var faqs = new List<FAQEntry>
            {
                new FAQEntry(FaqId1, Question1, Answer1, FAQCategoryEnum.CheckIn, ZeroViewCount, ZeroViewCount, ZeroViewCount),
                new FAQEntry(FaqId2, Question2, Answer2, FAQCategoryEnum.Baggage, ZeroViewCount, ZeroViewCount, ZeroViewCount),
                new FAQEntry(FaqId3, Question3, Answer3, FAQCategoryEnum.CheckIn, ZeroViewCount, ZeroViewCount, ZeroViewCount)
            };
            faqRepositoryMock.GetAllAsync().Returns(Task.FromResult((IEnumerable<FAQEntry>)faqs));

            var result = await faqService.FilterFAQEntryAsync(FAQCategoryEnum.All, string.Empty);

            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public async Task FilterFAQEntry_WithSearchQuery_FiltersOnQuestion()
        {
            var faqs = new List<FAQEntry>
            {
                new FAQEntry(FaqId1, Question1, Answer1, FAQCategoryEnum.CheckIn, ZeroViewCount, ZeroViewCount, ZeroViewCount),
                new FAQEntry(FaqId2, Question2, Answer2, FAQCategoryEnum.CheckIn, ZeroViewCount, ZeroViewCount, ZeroViewCount)
            };
            faqRepositoryMock.GetAllAsync().Returns(Task.FromResult((IEnumerable<FAQEntry>)faqs));

            var result = await faqService.FilterFAQEntryAsync(FAQCategoryEnum.All, SearchQueryMatchingQuestion);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(Question1, result[0].Question);
        }

        [TestMethod]
        public async Task FilterFAQEntry_WithSearchQuery_FiltersOnAnswer()
        {
            var faqs = new List<FAQEntry>
            {
                new FAQEntry(FaqId1, Question1, Answer1, FAQCategoryEnum.CheckIn, ZeroViewCount, ZeroViewCount, ZeroViewCount),
                new FAQEntry(FaqId2, Question2, Answer2, FAQCategoryEnum.CheckIn, ZeroViewCount, ZeroViewCount, ZeroViewCount)
            };
            faqRepositoryMock.GetAllAsync().Returns(Task.FromResult((IEnumerable<FAQEntry>)faqs));

            var result = await faqService.FilterFAQEntryAsync(FAQCategoryEnum.All, SearchQueryMatchingAnswer);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(Question2, result[0].Question);
        }

        [TestMethod]
        public async Task FilterFAQEntry_SearchIsCaseInsensitive()
        {
            var faqs = new List<FAQEntry>
            {
                new FAQEntry(FaqId1, Question1, Answer1, FAQCategoryEnum.CheckIn, ZeroViewCount, ZeroViewCount, ZeroViewCount)
            };
            faqRepositoryMock.GetAllAsync().Returns(Task.FromResult((IEnumerable<FAQEntry>)faqs));

            var result = await faqService.FilterFAQEntryAsync(FAQCategoryEnum.All, SearchQueryCaseInsensitive);

            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public async Task FilterFAQEntry_NoMatchingQuery_ReturnsEmpty()
        {
            var faqs = new List<FAQEntry>
            {
                new FAQEntry(FaqId1, Question1, Answer1, FAQCategoryEnum.CheckIn, ZeroViewCount, ZeroViewCount, ZeroViewCount)
            };
            faqRepositoryMock.GetAllAsync().Returns(Task.FromResult((IEnumerable<FAQEntry>)faqs));

            var result = await faqService.FilterFAQEntryAsync(FAQCategoryEnum.All, SearchQueryNoMatch);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task FilterFAQEntry_NullSearchQuery_ReturnsAllInCategory()
        {
            var faqs = new List<FAQEntry>
            {
                new FAQEntry(FaqId1, Question1, Answer1, FAQCategoryEnum.CheckIn, ZeroViewCount, ZeroViewCount, ZeroViewCount),
                new FAQEntry(FaqId2, Question2, Answer2, FAQCategoryEnum.CheckIn, ZeroViewCount, ZeroViewCount, ZeroViewCount)
            };
            faqRepositoryMock.GetAllAsync().Returns(Task.FromResult((IEnumerable<FAQEntry>)faqs));

            var result = await faqService.FilterFAQEntryAsync(FAQCategoryEnum.All, null!);

            Assert.AreEqual(2, result.Count);
        }
    }
}