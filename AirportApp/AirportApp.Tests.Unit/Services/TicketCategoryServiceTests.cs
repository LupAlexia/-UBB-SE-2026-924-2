using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.Src.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirportApp.Src.Service;
using AirportApp.ClassLibrary.Repository.Interfaces;
using NSubstitute;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Tests.Unit.Src.Service
{
    [TestClass]
    public class TicketCategoryServiceTests
    {
        private ITicketCategoryRepository categoryRepositoryMock;
        private ComplaintTicketCategoryService categoryService;

        [TestInitialize]
        public void Setup()
        {
            categoryRepositoryMock = Substitute.For<ITicketCategoryRepository>();
            categoryService = new ComplaintTicketCategoryService(categoryRepositoryMock);
        }

        [TestMethod]
        public async Task GetCategoryById_WhenCalled_ReturnsCategoryFromRepository()
        {
            var expectedCategory = new ComplaintTicketCategory(1, "Technical", ComplaintTicketUrgencyLevelEnum.HIGH);
            categoryRepositoryMock.GetByIdAsync(1).Returns(Task.FromResult(expectedCategory));

            var resultedCategory = await categoryService.GetCategoryByIdAsync(1);

            Assert.IsNotNull(resultedCategory);
            Assert.AreEqual(expectedCategory.CategoryName, resultedCategory.CategoryName);
            await categoryRepositoryMock.Received(1).GetByIdAsync(1);
        }

        [TestMethod]
        public async Task GetAllCategories_WhenCalled_ReturnsAllCategoriesFromRepository()
        {
            var categories = new List<ComplaintTicketCategory>
            {
                new ComplaintTicketCategory(1, "IT", ComplaintTicketUrgencyLevelEnum.MEDIUM),
                new ComplaintTicketCategory(2, "HR", ComplaintTicketUrgencyLevelEnum.LOW)
            };
            categoryRepositoryMock.GetAllAsync().Returns(Task.FromResult((IEnumerable<ComplaintTicketCategory>)categories));

            var resultedCategories = (await categoryService.GetAllCategoriesAsync()).ToList();

            Assert.AreEqual(2, resultedCategories.Count);
            await categoryRepositoryMock.Received(1).GetAllAsync();
        }

        [TestMethod]
        public async Task GetCategoryById_WhenRepositoryThrows_ServicePropagatesException()
        {
            categoryRepositoryMock.GetByIdAsync(Arg.Any<int>()).Returns(x => Task.FromException<ComplaintTicketCategory>(new KeyNotFoundException()));

            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () => await categoryService.GetCategoryByIdAsync(999));
        }
    }
}
