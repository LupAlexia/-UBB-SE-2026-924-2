using AirportApp.Src.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirportApp.Src.Service;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirportApp.Tests.Unit.Src.Service
{
    [TestClass]
    public class TicketCategoryServiceTests
    {
        private ITicketCategoryRepository _categoryRepositoryMock;
        private TicketCategoryService _categoryService;

        [TestInitialize]
        public void Setup()
        {
            _categoryRepositoryMock = Substitute.For<ITicketCategoryRepository>();
            _categoryService = new TicketCategoryService(_categoryRepositoryMock);
        }

        [TestMethod]
        public async Task GetCategoryById_WhenCalled_ReturnsCategoryFromRepository()
        {
            var expectedCategory = new TicketCategory(1, "Technical", TicketUrgencyLevelEnum.HIGH);
            _categoryRepositoryMock.GetByIdAsync(1).Returns(Task.FromResult(expectedCategory));          
            
            var resultedCategory = await _categoryService.GetCategoryByIdAsync(1);

            Assert.IsNotNull(resultedCategory);
            Assert.AreEqual(expectedCategory.CategoryName, resultedCategory.CategoryName);
            await _categoryRepositoryMock.Received(1).GetByIdAsync(1);
        }

        [TestMethod]
        public async Task GetAllCategories_WhenCalled_ReturnsAllCategoriesFromRepository()
        {
            var categories = new List<TicketCategory>
            {
                new TicketCategory(1, "IT", TicketUrgencyLevelEnum.MEDIUM),
                new TicketCategory(2, "HR", TicketUrgencyLevelEnum.LOW)
            };
            _categoryRepositoryMock.GetAllAsync().Returns(Task.FromResult((IEnumerable<TicketCategory>)categories));

            var resultedCategories = (await _categoryService.GetAllCategoriesAsync()).ToList();

            Assert.AreEqual(2, resultedCategories.Count);
            await _categoryRepositoryMock.Received(1).GetAllAsync();
        }

        [TestMethod]
        public async Task GetCategoryById_WhenRepositoryThrows_ServicePropagatesException()
        {
            _categoryRepositoryMock.GetByIdAsync(Arg.Any<int>()).Returns(x => Task.FromException<TicketCategory>(new KeyNotFoundException()));

            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () => await _categoryService.GetCategoryByIdAsync(999));
        }
    }
}
