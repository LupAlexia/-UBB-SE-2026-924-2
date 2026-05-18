using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirportApp.Src.Service;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.Entity.Domain;
using NSubstitute;

namespace AirportApp.Tests.Unit.Src.Service
{
    [TestClass]
    public class ReviewServiceTests
    {
        private const int TestUserId = 1;
        private const int TestReviewId = 1;
        private const int MinRating = 1;
        private const int MaxRating = 5;
        private const int BelowMinRating = 0;
        private const int AboveMaxRating = 6;
        private const int MixedRating1 = 5;
        private const int MixedRating2 = 3;
        private const int MixedRating3 = 1;
        private const int AscendingRating1 = 1;
        private const int AscendingRating2 = 2;
        private const int AscendingRating3 = 3;
        private const int AscendingRating4 = 4;
        private const float AscendingAverageExpected = 2.5f;
        private const float AllMinAverageExpected = 1.0f;
        private const float AllMaxAverageExpected = 5.0f;
        private const float MixedAverageExpected = 3.0f;
        private const string TestUserName = "Test User";
        private const string TestUserEmail = "test@test.com";
        private const string ValidMessage = "Great experience";
        private const string EmptyMessage = "";
        private const string DuplicateMessage = "I already exist";

        private IRepository<int, Review> reviewRepository = null!;
        private ReviewService reviewService = null!;
        private User testUser = null!;

        [TestInitialize]
        public void Setup()
        {
            reviewRepository = Substitute.For<IRepository<int, Review>>();
            reviewService = new ReviewService(reviewRepository);
            testUser = new User(TestUserId, TestUserName, TestUserEmail);
            reviewRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<Review>)new List<Review>()));
        }

        [TestMethod]
        public async Task CalculateAverageRating_AscendingRatings_ReturnsCorrectAverage()
        {
            var review = new Review(TestReviewId, testUser, ValidMessage, AscendingRating1, AscendingRating2, AscendingRating3, AscendingRating4);
            var result = await reviewService.CalculateAverageRatingAsync(review);
            Assert.AreEqual(AscendingAverageExpected, result);
        }

        [TestMethod]
        public async Task CalculateAverageRating_AllMaxRatings_ReturnsMax()
        {
            var review = new Review(TestReviewId, testUser, ValidMessage, MaxRating, MaxRating, MaxRating, MaxRating);
            var result = await reviewService.CalculateAverageRatingAsync(review);
            Assert.AreEqual(AllMaxAverageExpected, result);
        }

        [TestMethod]
        public async Task CalculateAverageRating_AllMinRatings_ReturnsMin()
        {
            var review = new Review(TestReviewId, testUser, ValidMessage, MinRating, MinRating, MinRating, MinRating);
            var result = await reviewService.CalculateAverageRatingAsync(review);
            Assert.AreEqual(AllMinAverageExpected, result);
        }

        [TestMethod]
        public async Task CalculateAverageRating_MixedRatings_ReturnsCorrectAverage()
        {
            var review = new Review(TestReviewId, testUser, ValidMessage, MixedRating1, MixedRating2, MixedRating3, MixedRating2);
            var result = await reviewService.CalculateAverageRatingAsync(review);
            Assert.AreEqual(MixedAverageExpected, result);
        }

        [TestMethod]
        public async Task CreateReview_WithValidData_CallsRepositoryToSave()
        {
            await reviewService.CreateReviewAsync(TestReviewId, testUser, ValidMessage, MaxRating, MaxRating, MaxRating, MaxRating);
            await reviewRepository.Received(1).CreateNewEntityAsync(Arg.Any<Review>());
        }

        [TestMethod]
        public async Task CreateReview_WithInvalidRating_ThrowsAndDoesNotCallRepository()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => reviewService.CreateReviewAsync(TestReviewId, testUser, ValidMessage, BelowMinRating, MaxRating, MaxRating, MaxRating));

            await reviewRepository.DidNotReceive().CreateNewEntityAsync(Arg.Any<Review>());
        }

        [TestMethod]
        public async Task ValidateReview_DutyFreeRatingBelowMin_ThrowsArgumentException()
        {
            var review = new Review(TestReviewId, testUser, ValidMessage, BelowMinRating, MaxRating, MaxRating, MaxRating);
            var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(() => reviewService.ValidateReviewAsync(review));
            StringAssert.Contains("Duty Free Rating must be between 1 and 5", ex.Message);
        }

        [TestMethod]
        public async Task ValidateReview_CleanlinessRatingAboveMax_ThrowsArgumentException()
        {
            var review = new Review(TestReviewId, testUser, ValidMessage, MaxRating, MaxRating, MaxRating, AboveMaxRating);
            var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(() => reviewService.ValidateReviewAsync(review));
            StringAssert.Contains("Cleanliness Rating must be between 1 and 5", ex.Message);
        }

        [TestMethod]
        public async Task ValidateReview_EmptyMessage_ThrowsArgumentException()
        {
            var review = new Review(TestReviewId, testUser, EmptyMessage, MaxRating, MaxRating, MaxRating, MaxRating);
            var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(() => reviewService.ValidateReviewAsync(review));
            StringAssert.Contains("Message cannot be null or empty", ex.Message);
        }

        [TestMethod]
        public void ValidateReview_NullUser_ThrowsArgumentException()
        {
            var ex = Assert.ThrowsException<ArgumentException>(() => new Review(TestReviewId, null, ValidMessage, MaxRating, MaxRating, MaxRating, MaxRating));
            StringAssert.Contains("User cannot be null", ex.Message);
        }

        [TestMethod]
        public async Task ValidateReview_DuplicateReview_ThrowsArgumentException()
        {
            var existingReview = new Review(TestReviewId, testUser, DuplicateMessage, MaxRating, MaxRating, MaxRating, MaxRating);
            reviewRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<Review>)new List<Review> { existingReview }));

            var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(() => reviewService.ValidateReviewAsync(existingReview));
            StringAssert.Contains("Review already exists", ex.Message);
        }

        [TestMethod]
        public async Task ValidateReview_FlightExperienceRatingBelowMin_ThrowsArgumentException()
        {
            var review = new Review(TestReviewId, testUser, ValidMessage, MaxRating, BelowMinRating, MaxRating, MaxRating);
            var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(() => reviewService.ValidateReviewAsync(review));
            StringAssert.Contains("Flight Experience Rating must be between 1 and 5", ex.Message);
        }

        [TestMethod]
        public async Task ValidateReview_StaffFriendlinessRatingAboveMax_ThrowsArgumentException()
        {
            var review = new Review(TestReviewId, testUser, ValidMessage, MaxRating, MaxRating, AboveMaxRating, MaxRating);
            var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(() => reviewService.ValidateReviewAsync(review));
            StringAssert.Contains("Staff Friendliness Rating must be between 1 and 5", ex.Message);
        }

        [TestMethod]
        public async Task ValidateReview_CleanlinessRatingBelowMin_ThrowsArgumentException()
        {
            var review = new Review(TestReviewId, testUser, ValidMessage, MaxRating, MaxRating, MaxRating, BelowMinRating);
            var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(() => reviewService.ValidateReviewAsync(review));
            StringAssert.Contains("Cleanliness Rating must be between 1 and 5", ex.Message);
        }

        [TestMethod]
        public async Task ValidateReview_StaffFriendlinessBelowMin_ThrowsArgumentException()
        {
            var review = new Review(TestReviewId, testUser, ValidMessage, MaxRating, MaxRating, BelowMinRating, MaxRating);
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => reviewService.ValidateReviewAsync(review));
        }

        [TestMethod]
        public async Task ValidateReview_WithAllValidData_DoesNotThrow()
        {
            var validReview = new Review(TestReviewId, testUser, ValidMessage, MaxRating, MaxRating, MaxRating, MaxRating);
            await reviewService.ValidateReviewAsync(validReview);
        }

        [TestMethod]
        public async Task ValidateReview_AllRatingsAtMinimum_DoesNotThrow()
        {
            var review = new Review(TestReviewId, testUser, ValidMessage, MinRating, MinRating, MinRating, MinRating);
            await reviewService.ValidateReviewAsync(review);
        }
    }
}