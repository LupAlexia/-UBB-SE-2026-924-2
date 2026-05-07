using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.Src.ViewModel;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AirportApp.Tests.Unit.Src.Service
{
    [TestClass]
    public class TicketSubcategoryServiceTests
    {
        private ITicketSubcategoryRepository subcategoryRepositoryMock;
        private TicketSubcategoryService subcategoryService;
        private TicketCategory testCategory;

        [TestInitialize]
        public void Setup()
        {
            subcategoryRepositoryMock = Substitute.For<ITicketSubcategoryRepository>();
            subcategoryService = new TicketSubcategoryService(subcategoryRepositoryMock);

            testCategory = new TicketCategory(1, "IT", TicketUrgencyLevelEnum.HIGH);
        }

        [TestMethod]
        public async Task GetSubcategoryById_WhenCalled_ReturnsEntityFromRepository()
        {
            var expectedSub = new TicketSubcategory(10, "Software", 500, testCategory);
            subcategoryRepositoryMock.GetByIdAsync(10).Returns(Task.FromResult(expectedSub));

            var resultedCategory = await subcategoryService.GetSubcategoryByIdAsync(10);

            Assert.AreEqual("Software", resultedCategory.SubcategoryName);
            await subcategoryRepositoryMock.Received(1).GetByIdAsync(10);
        }

        [TestMethod]
        public async Task GetSubcategoriesByCategoryId_WhenCalled_ReturnsFilteredList()
        {
            var expectedSubcategoryList = new List<TicketSubcategory>
            {
                new TicketSubcategory(10, "Software", 500, testCategory),
                new TicketSubcategory(11, "Hardware", 501, testCategory)
            };
            subcategoryRepositoryMock.GetByCategoryIdAsync(1).Returns(Task.FromResult((IEnumerable<TicketSubcategory>)expectedSubcategoryList));

            var resultedSubcategories = (await subcategoryService.GetSubcategoriesByCategoryIdAsync(1)).ToList();

            Assert.AreEqual(2, resultedSubcategories.Count);
            Assert.AreEqual("Software", resultedSubcategories[0].SubcategoryName);
            await subcategoryRepositoryMock.Received(1).GetByCategoryIdAsync(1);
        }

        [TestMethod]
        public async Task GetSubcategoryById_WhenIdIsInvalid_PropagatesException()
        {
            subcategoryRepositoryMock.GetByIdAsync(999).Returns(x => Task.FromException<TicketSubcategory>(new KeyNotFoundException()));
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () => await subcategoryService.GetSubcategoryByIdAsync(999));
        }
    }
}
