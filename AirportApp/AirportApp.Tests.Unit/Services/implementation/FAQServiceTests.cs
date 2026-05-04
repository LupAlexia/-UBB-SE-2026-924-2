using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudSpritzers1.Src.Service.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using CloudSpritzers1.Src.Repository.Interfaces;
using CloudSpritzers1.Src.Model.Faq;
using CloudSpritzers1.Src.Service.Interfaces;
using CloudSpritzers1.Src.Dto;
namespace CloudSpritzers1Tests.Src.Service.Implementation
{
    [TestClass()]
    public class FAQServiceTests
    {
        private IFAQRepository _faqRepository;
        private IFAQService _faqService;
        [TestInitialize]
        public void Setup()
        {
            _faqRepository = Substitute.For<IFAQRepository>();
            _faqService = new FAQService(_faqRepository);

            var frequentlyAskedQuestionsList = new List<FAQEntry>
            {
                new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0),
                new FAQEntry(2, "How much does parking cost per day?", "100 euros", FAQCategoryEnum.Parking, 200, 3, 1),
                new FAQEntry(3, "Can I bring my dog on the plane?", "Only if you buy a plane ticket for him also", FAQCategoryEnum.Baggage, 123, 34, 2),
            };
            _faqRepository.GetAll().Returns(frequentlyAskedQuestionsList);
        }

        [TestMethod()]
        public void FilterFAQEntry_WithCategoryAndQuestionSearchMatch_ReturnsFilteredEntities()
        {
            var FAQCatgoryToFilterBy = FAQCategoryEnum.All;
            var SearchQueryToFilterBy = "cars";

            var expectededFAQs = new List<FAQEntry>
            {
                new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0),
            };

            _faqRepository.GetByCategory(FAQCategoryEnum.All).Returns(expectededFAQs);
            
            var resultedFAQs = _faqService.FilterFAQEntry(FAQCatgoryToFilterBy, SearchQueryToFilterBy);
            CollectionAssert.AreEqual(expectededFAQs, resultedFAQs);
        }

        [TestMethod()]
        public void SearchMatchEntry_WithNoMatchingString_ReturnsEmptyList()
        {
            var FAQCatgoryToFilterBy = FAQCategoryEnum.All;
            var SearchQueryToFilterBy = "water";

            var expectedFAQs = new List<FAQEntry>
            {
            };

            _faqRepository.GetByCategory(FAQCatgoryToFilterBy).Returns(expectedFAQs);

            var resultedFAQs = _faqService.FilterFAQEntry(FAQCatgoryToFilterBy, SearchQueryToFilterBy);
            Assert.AreEqual(0, resultedFAQs.Count());
            CollectionAssert.AreEqual(expectedFAQs, resultedFAQs);
        }

        [TestMethod()]
        public void FilterFAQEntry_WithCategoryAndAnswerSearchMatch_ReturnsFilteredEntities()
        {
            var FAQCatgoryToFilterBy = FAQCategoryEnum.Parking;
            var SearchQueryToFilterBy = "audi";

            var expectedFAQs = new List<FAQEntry>
            {
                new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0),
            };

            _faqRepository.GetByCategory(FAQCategoryEnum.Parking).Returns(expectedFAQs);

            var resultedFAQs = _faqService.FilterFAQEntry(FAQCatgoryToFilterBy, SearchQueryToFilterBy);
            CollectionAssert.AreEqual(expectedFAQs, resultedFAQs);
        }
    }
}