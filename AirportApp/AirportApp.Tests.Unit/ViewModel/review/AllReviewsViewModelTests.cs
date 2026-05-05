using AirportApp.Src.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirportApp.Src.Service;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Entity.Dto.MappingProfiles;
using AirportApp.Src.ViewModel.Review;
using AutoMapper;
using NSubstitute;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReviewEntity = AirportApp.ClassLibrary.Entity.Domain.Review.Review;
using UserEntity = AirportApp.ClassLibrary.Entity.Domain.User;

namespace AirportApp.Src.ViewModel
{
    [TestClass]
    public class AllReviewsViewModelTests
    {
        private AllReviewsViewModel _viewModel;
        private IRepository<int, ReviewEntity> _mockRepository;
        private IMapper _mapper;
        private UserEntity _testUser;
        private ILoggerFactory _loggerFactory;

        [TestInitialize]
        public void Setup()
        {
            _loggerFactory = Substitute.For<ILoggerFactory>();
            var configuration = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<ReviewMappingProfile>(), _loggerFactory);
            _mapper = configuration.CreateMapper();

            _mockRepository = Substitute.For<IRepository<int, ReviewEntity>>();
            var reviewService = new ReviewService(_mockRepository);

            _testUser = new UserEntity(1, "Test", "test@test.com");

            _viewModel = new AllReviewsViewModel(reviewService, _mapper);
        }

        [TestMethod]
        public async Task LoadData_WhenNoReviewsExist_DoesNotCalculateAverages()
        {
            _mockRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<ReviewEntity>)new List<ReviewEntity>()));

            await _viewModel.LoadDataAsync();

            Assert.AreEqual(0, _viewModel.Reviews.Count);
            Assert.AreEqual(0, _viewModel.TotalReviews);
        }

        [TestMethod]
        public async Task LoadData_WhenReviewsExist_CalculatesAveragesAndFormatsStrings()
        {
            var reviews = new List<ReviewEntity>
            {
                new ReviewEntity(1, _testUser, "Good", 5, 4, 3, 2),
                new ReviewEntity(2, _testUser, "Bad", 1, 2, 3, 4)
            };
            _mockRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<ReviewEntity>)reviews));

            await _viewModel.LoadDataAsync();

            Assert.AreEqual(3.0, _viewModel.AverageDutyFree);
            Assert.AreEqual("3.0", _viewModel.FormattedAverageDutyFree);
            Assert.AreEqual("3.0", _viewModel.FormattedAverageFlightExperience);
            Assert.AreEqual(2, _viewModel.TotalReviews);
        }

        [TestMethod]
        public async Task LoadData_WhenReviewsExist_PopulatesCollectionWithMappedDTOs()
        {
            var reviews = new List<ReviewEntity>
            {
                new ReviewEntity(1, _testUser, "Excellent service", 5, 5, 5, 5)
            };
            _mockRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<ReviewEntity>)reviews));

            await _viewModel.LoadDataAsync();

            Assert.AreEqual(1, _viewModel.Reviews.Count);
            Assert.AreEqual("Excellent service", _viewModel.Reviews[0].message);
            Assert.IsInstanceOfType(_viewModel.Reviews[0], typeof(ReviewDTO));
        }

        [TestMethod]
        public async Task LoadData_WhenServiceReturnsNull_ReturnsEarlyWithoutError()
        {
            _mockRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<ReviewEntity>)null!));

            await _viewModel.LoadDataAsync();

            Assert.AreEqual(0, _viewModel.Reviews.Count);
        }
    }
}
