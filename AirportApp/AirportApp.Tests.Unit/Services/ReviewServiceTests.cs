using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.Src.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirportApp.Src.Service;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.Entity.Domain.Review;
using AirportApp.ClassLibrary.Entity.Domain;
using NSubstitute;

namespace AirportApp.Tests.Unit.Src.Service
{
    [TestClass]
    public class ReviewServiceTests
    {
        private IRepository<int, Review> reviewRepository = null!;
        private ReviewService reviewService = null!;
        private User testUser = null!;

        [TestInitialize]
        public void Setup()
        {
            reviewRepository = Substitute.For<IRepository<int, Review>>();
            reviewService = new ReviewService(reviewRepository);
            testUser = new User(1, "Test User", "test@test.com");
            reviewRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<Review>)new List<Review>()));
        }

        [TestMethod]
        public void CalculateAverageRating_WhenCalled_ReturnsCorrectMath()
        {
            var review = new Review(1, testUser, "Test", 1, 2, 3, 4); // Sum = 10

            var resultedReviewAverageRating = reviewService.CalculateAverageRating(review);

            Assert.AreEqual(2.5f, resultedReviewAverageRating);
        }

        [TestMethod]
        public async Task CreateReview_WithValidData_CallsRepositoryToSave()
        {
            await reviewService.CreateReviewAsync(1, testUser, "Great flight", 5, 5, 5, 5);
            await reviewRepository.Received(1).CreateNewEntityAsync(Arg.Any<Review>());
        }

        [TestMethod]
        public async Task ValidateReview_RatingBelowMin_ThrowsArgumentException()
        {
            var review = new Review(1, testUser, "Too low", 0, 5, 5, 5);

            var exceptionThrown = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await reviewService.ValidateReviewAsync(review));

            StringAssert.Contains("Duty Free Rating must be between 1 and 5", exceptionThrown.Message);
        }

        [TestMethod]
        public async Task ValidateReview_RatingAboveMax_ThrowsArgumentException()
        {
            var review = new Review(1, testUser, "Too high", 5, 5, 5, 6);

            var exceptionThrown = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await reviewService.ValidateReviewAsync(review));

            StringAssert.Contains("Cleanliness Rating must be between 1 and 5", exceptionThrown.Message);
        }

        [TestMethod]
        public async Task ValidateReview_EmptyMessage_ThrowsArgumentException()
        {
            var review = new Review(1, testUser, string.Empty, 5, 5, 5, 5);

            var exceptionThrown = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await reviewService.ValidateReviewAsync(review));

            StringAssert.Contains("Message cannot be null or empty", exceptionThrown.Message);
        }

        [TestMethod]
        public void ValidateReview_NullUser_ThrowsArgumentException()
        {
            var exceptionThrown = Assert.ThrowsException<ArgumentException>(() =>
                new Review(1, null, "No user", 5, 5, 5, 5));

            StringAssert.Contains("User cannot be null", exceptionThrown.Message);
        }

        [TestMethod]
        public async Task ValidateReview_DuplicateReview_ThrowsArgumentException()
        {
            var existingReview = new Review(1, testUser, "I already exist", 5, 5, 5, 5);

            reviewRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<Review>)new List<Review> { existingReview }));

            var exceptionThrown = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await reviewService.ValidateReviewAsync(existingReview));
            StringAssert.Contains("Review already exists", exceptionThrown.Message);
        }

        [TestMethod]
        public async Task GetById_ValidId_ReturnsReviewFromRepository()
        {
            var expectedReview = new Review(1, testUser, "Great", 5, 5, 5, 5);
            reviewRepository.GetByIdAsync(1).Returns(Task.FromResult(expectedReview));

            var resultedReview = await reviewService.GetByIdAsync(1);

            Assert.AreEqual(expectedReview, resultedReview);
            await reviewRepository.Received(1).GetByIdAsync(1);
        }

        [TestMethod]
        public async Task UpdateById_WhenCalled_CallsRepositoryUpdate()
        {
            var updatedReview = new Review(1, testUser, "Updated Message", 4, 4, 4, 4);
            await reviewService.UpdateByIdAsync(1, updatedReview);
            await reviewRepository.Received(1).UpdateByIdAsync(1, updatedReview);
        }

        [TestMethod]
        public async Task DeleteById_WhenCalled_CallsRepositoryDelete()
        {
            await reviewService.DeleteByIdAsync(10);
            await reviewRepository.Received(1).DeleteByIdAsync(10);
        }

        [TestMethod]
        public async Task GetAll_WhenCalled_ReturnsListOfReviews()
        {
            var reviews = new List<Review>
            {
                new Review(1, testUser, "R1", 5, 5, 5, 5),
                new Review(2, testUser, "R2", 4, 4, 4, 4)
            };
            reviewRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<Review>)reviews));

            var resultedReviewList = (await reviewService.GetAllAsync()).ToList();

            Assert.AreEqual(2, resultedReviewList.Count);
            await reviewRepository.Received(1).GetAllAsync();
        }

        [TestMethod]
        public async Task ValidateReview_FlightExperienceRatingInvalid_ThrowsArgumentException()
        {
            var review = new Review(1, testUser, "Test", 5, 0, 5, 5);

            var exceptionThrown = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await reviewService.ValidateReviewAsync(review));

            StringAssert.Contains("Flight Experience Rating must be between 1 and 5", exceptionThrown.Message);
        }

        [TestMethod]
        public async Task ValidateReview_StaffFriendlinessRatingInvalid_ThrowsArgumentException()
        {
            var review = new Review(1, testUser, "Test", 5, 5, 6, 5);

            var exceptionThrown = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await reviewService.ValidateReviewAsync(review));

            StringAssert.Contains("Staff Friendliness Rating must be between 1 and 5", exceptionThrown.Message);
        }

        [TestMethod]
        public async Task ValidateReview_CleanlinessRatingInvalid_ThrowsArgumentException()
        {
            var review = new Review(1, testUser, "Test", 5, 5, 5, 0);

            var exceptionThrown = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await reviewService.ValidateReviewAsync(review));

            StringAssert.Contains("Cleanliness Rating must be between 1 and 5", exceptionThrown.Message);
        }

        [TestMethod]
        public async Task ValidateReview_WithAllValidData_DoesNotThrowAndMovesToNextStep()
        {
            var validReview = new Review(1, testUser, "Everything was perfect!", 5, 5, 5, 5);

            await reviewService.ValidateReviewAsync(validReview);
        }

        [TestMethod]
        public async Task ValidateReview_StaffFriendlinessBelowMin_ThrowsArgumentException()
        {
            var review = new Review(1, testUser, "Test", 5, 5, 0, 5);

            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await reviewService.ValidateReviewAsync(review));
        }
    }
}
