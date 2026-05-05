using AirportApp.Src.ViewModel;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirportApp.Tests.Unit.Src.Service
{
    [TestClass]
    public class TicketSubcategoryServiceTests
    {
        private ITicketSubcategoryRepository _subcategoryRepositoryMock;
        private TicketSubcategoryService _subcategoryService;
        private TicketCategory _testCategory;

        [TestInitialize]
        public void Setup()
        {
            _subcategoryRepositoryMock = Substitute.For<ITicketSubcategoryRepository>();
            _subcategoryService = new TicketSubcategoryService(_subcategoryRepositoryMock);

            _testCategory = new TicketCategory(1, "IT", TicketUrgencyLevelEnum.HIGH);
        }

        [TestMethod]
        public async Task GetSubcategoryById_WhenCalled_ReturnsEntityFromRepository()
        {
            var expectedSub = new TicketSubcategory(10, "Software", 500, _testCategory);
            _subcategoryRepositoryMock.GetByIdAsync(10).Returns(Task.FromResult(expectedSub));

            var resultedCategory = await _subcategoryService.GetSubcategoryByIdAsync(10);

            Assert.AreEqual("Software", resultedCategory.SubcategoryName);
            await _subcategoryRepositoryMock.Received(1).GetByIdAsync(10);
        }

        [TestMethod]
        public async Task GetSubcategoriesByCategoryId_WhenCalled_ReturnsFilteredList()
        {
            var expectedSubcategoryList = new List<TicketSubcategory>
            {
                new TicketSubcategory(10, "Software", 500, _testCategory),
                new TicketSubcategory(11, "Hardware", 501, _testCategory)
            };
            _subcategoryRepositoryMock.GetByCategoryIdAsync(1).Returns(Task.FromResult((IEnumerable<TicketSubcategory>)expectedSubcategoryList));

            var resultedSubcategories = (await _subcategoryService.GetSubcategoriesByCategoryIdAsync(1)).ToList();

            Assert.AreEqual(2, resultedSubcategories.Count);
            Assert.AreEqual("Software", resultedSubcategories[0].SubcategoryName);
            await _subcategoryRepositoryMock.Received(1).GetByCategoryIdAsync(1);
        }

        [TestMethod]
        public async Task GetSubcategoryById_WhenIdIsInvalid_PropagatesException()
        {
            _subcategoryRepositoryMock.GetByIdAsync(999).Returns(x => Task.FromException<TicketSubcategory>(new KeyNotFoundException()));
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () => await _subcategoryService.GetSubcategoryByIdAsync(999));
        }
    }
}
