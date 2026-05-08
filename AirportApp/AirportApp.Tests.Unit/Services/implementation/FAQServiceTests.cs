using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirportApp.Src.Service.Implementation;
using AirportApp.ClassLibrary.Repository.Interfaces;
using NSubstitute;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Tests.Unit.Src.Service
{
    [TestClass]
    public class FAQServiceTests
    {
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
                new FAQEntry(1, "Q1", "A1", FAQCategoryEnum.CheckIn, 0, 0, 0),
                new FAQEntry(2, "Q2", "A2", FAQCategoryEnum.CheckIn, 0, 0, 0),
                new FAQEntry(3, "Q3", "A3", FAQCategoryEnum.Baggage, 0, 0, 0)
            };
            faqRepositoryMock.GetByCategoryAsync(FAQCategoryEnum.CheckIn).Returns(Task.FromResult(faqs.Where(f => f.Category == FAQCategoryEnum.CheckIn).ToList()));

            var result = (await faqService.GetByCategoryAsync(FAQCategoryEnum.CheckIn)).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(f => f.Category == FAQCategoryEnum.CheckIn));
            await faqRepositoryMock.Received(1).GetByCategoryAsync(FAQCategoryEnum.CheckIn);
        }

        [TestMethod]
        public async Task GetAll_WhenCalled_ReturnsAllFromRepository()
        {
            var faqs = new List<FAQEntry>
            {
                new FAQEntry(1, "Q1", "A1", FAQCategoryEnum.CheckIn, 0, 0, 0),
                new FAQEntry(2, "Q2", "A2", FAQCategoryEnum.Baggage, 0, 0, 0)
            };
            faqRepositoryMock.GetAllAsync().Returns(Task.FromResult((IEnumerable<FAQEntry>)faqs));

            var result = (await faqService.GetAllAsync()).ToList();

            Assert.AreEqual(2, result.Count);
            await faqRepositoryMock.Received(1).GetAllAsync();
        }

        [TestMethod]
        public async Task AddFAQEntry_WhenCalled_CallsRepository()
        {
            var faq = new FAQEntry(1, "Q1", "A1", FAQCategoryEnum.CheckIn, 0, 0, 0);
            await faqService.AddFAQEntryAsync(faq);
            await faqRepositoryMock.Received(1).CreateNewEntityAsync(faq);
        }
    }
}
