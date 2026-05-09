using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.Src.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirportApp.Src.Service;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.Entity.Dto;
using AutoMapper;
using NSubstitute;
using Microsoft.Extensions.Logging;

using ReviewEntity = AirportApp.ClassLibrary.Entity.Domain.Review;
using UserEntity = AirportApp.ClassLibrary.Entity.Domain.User;

namespace AirportApp.Tests.Unit.ViewModel
{
    [TestClass]
    public class AllReviewsViewModelTests
    {
        private AllReviewsViewModel viewModel;
        private IRepository<int, ReviewEntity> mockRepository;
        private IMapper mapper;
        private UserEntity testUser;
        private ILoggerFactory loggerFactory;

        [TestInitialize]
        public void Setup()
        {
            loggerFactory = Substitute.For<ILoggerFactory>();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile<ReviewMappingProfile>(), loggerFactory);
            mapper = configuration.CreateMapper();

            mockRepository = Substitute.For<IRepository<int, ReviewEntity>>();
            var reviewService = new ReviewService(mockRepository);

            testUser = new UserEntity(1, "Test", "test@test.com");

            viewModel = new AllReviewsViewModel(reviewService, mapper);
        }

        [TestMethod]
        public async Task LoadData_WhenNoReviewsExist_DoesNotCalculateAverages()
        {
            mockRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<ReviewEntity>)new List<ReviewEntity>()));

            await viewModel.LoadDataAsync();

            Assert.AreEqual(0, viewModel.Reviews.Count);
            Assert.AreEqual(0, viewModel.TotalReviews);
        }

        [TestMethod]
        public async Task LoadData_WhenReviewsExist_CalculatesAveragesAndFormatsStrings()
        {
            var reviews = new List<ReviewEntity>
            {
                new ReviewEntity(1, testUser, "Good", 5, 4, 3, 2),
                new ReviewEntity(2, testUser, "Bad", 1, 2, 3, 4)
            };
            mockRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<ReviewEntity>)reviews));

            await viewModel.LoadDataAsync();

            Assert.AreEqual(3.0, viewModel.AverageDutyFree);
            Assert.AreEqual("3.0", viewModel.FormattedAverageDutyFree);
            Assert.AreEqual("3.0", viewModel.FormattedAverageFlightExperience);
            Assert.AreEqual(2, viewModel.TotalReviews);
        }

        [TestMethod]
        public async Task LoadData_WhenReviewsExist_PopulatesCollectionWithMappedDTOs()
        {
            var reviews = new List<ReviewEntity>
            {
                new ReviewEntity(1, testUser, "Excellent service", 5, 5, 5, 5)
            };
            mockRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<ReviewEntity>)reviews));

            await viewModel.LoadDataAsync();

            Assert.AreEqual(1, viewModel.Reviews.Count);
            Assert.AreEqual("Excellent service", viewModel.Reviews[0].message);
            Assert.IsInstanceOfType(viewModel.Reviews[0], typeof(ReviewDTO));
        }

        [TestMethod]
        public async Task LoadData_WhenServiceReturnsNull_ReturnsEarlyWithoutError()
        {
            mockRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<ReviewEntity>)null!));

            await viewModel.LoadDataAsync();

            Assert.AreEqual(0, viewModel.Reviews.Count);
        }
    }
}
