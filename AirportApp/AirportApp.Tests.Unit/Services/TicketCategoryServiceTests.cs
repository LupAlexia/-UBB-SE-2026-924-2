using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudSpritzers1.Src.Service;
using CloudSpritzers1.Src.Repository.Interfaces;
using CloudSpritzers1.Src.Model.Ticket;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;

namespace CloudSpritzers1Tests.Src.Service
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
        public void GetCategoryById_WhenCalled_ReturnsCategoryFromRepository()
        {
           
            var expectedCategory = new TicketCategory(1, "Technical", TicketUrgencyLevelEnum.HIGH);
            _categoryRepositoryMock.GetById(1).Returns(expectedCategory);          
            
            var resultedCategory = _categoryService.GetCategoryById(1);

            Assert.AreEqual(expectedCategory.CategoryName, resultedCategory.CategoryName);
            _categoryRepositoryMock.Received(1).GetById(1);
        }

        [TestMethod]
        public void GetAllCategories_WhenCalled_ReturnsAllCategoriesFromRepository()
        {

            var categories = new List<TicketCategory>
            {
                new TicketCategory(1, "IT", TicketUrgencyLevelEnum.MEDIUM),
                new TicketCategory(2, "HR", TicketUrgencyLevelEnum.LOW)
            };
            _categoryRepositoryMock.GetAll().Returns(categories);

            var resultedCategories = _categoryService.GetAllCategories().ToList();

            Assert.AreEqual(2, resultedCategories.Count);
            _categoryRepositoryMock.Received(1).GetAll();
        }

        [TestMethod]
        public void GetCategoryById_WhenRepositoryThrows_ServicePropagatesException()
        {

            _categoryRepositoryMock.GetById(Arg.Any<int>()).Returns(capturedArgs => { throw new KeyNotFoundException(); });

            Assert.ThrowsExactly<KeyNotFoundException>(() => _categoryService.GetCategoryById(999));
        }
    }
}