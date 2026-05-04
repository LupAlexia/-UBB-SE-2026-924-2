using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudSpritzers1.Src.Service;
using CloudSpritzers1.Src.Repository.Interfaces;
using CloudSpritzers1.Src.Model.Review;
using CloudSpritzers1.Src.Model;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using CloudSpritzers1.Src.Repository;

namespace CloudSpritzers1Tests.Src.Service
{
    [TestClass]
    public class ReviewServiceTests
    {
        private IRepository<int, Review> _reviewRepository;
        private ReviewService _reviewService;
        private User _testUser;

        [TestInitialize]
        public void Setup()
        {
            // 1. Mock the repository using NSubstitute
            _reviewRepository = Substitute.For<IRepository<int, Review>>();

            // 2. Inject the mock into the service
            _reviewService = new ReviewService(_reviewRepository);

            // 3. Create a real User for test data
            _testUser = new User(1, "Test User", "test@test.com");

            // 4. Setup a default "GetAll" return to prevent null reference errors during validation
            _reviewRepository.GetAll().Returns(new List<Review>());
        }

        [TestMethod]
        public void CalculateAverageRating_WhenCalled_ReturnsCorrectMath()
        {
            var review = new Review(1, _testUser, "Test", 1, 2, 3, 4); // Sum = 10
           
            var resultedReviewAverageRating = _reviewService.CalculateAverageRating(review);

            Assert.AreEqual(2.5f, resultedReviewAverageRating);
        }

        [TestMethod]
        public void CreateReview_WithValidData_CallsRepositoryToSave()
        {
        
            _reviewService.CreateReview(1, _testUser, "Great flight", 5, 5, 5, 5);

            _reviewRepository.Received(1).CreateNewEntity(Arg.Any<Review>());
        }

        [TestMethod]
        public void ValidateReview_RatingBelowMin_ThrowsArgumentException()
        {

            var review = new Review(1, _testUser, "Too low", 0, 5, 5, 5);

           
            var exceptionThrown = Assert.ThrowsExactly<ArgumentException>(() =>
                _reviewService.ValidateReview(review));

            StringAssert.Contains("Duty Free Rating must be between 1 and 5", exceptionThrown.Message);
        }

        [TestMethod]
        public void ValidateReview_RatingAboveMax_ThrowsArgumentException()
        {
            
            var review = new Review(1, _testUser, "Too high", 5, 5, 5, 6);

            
            var exceptionThrown = Assert.ThrowsExactly<ArgumentException>(() =>
                _reviewService.ValidateReview(review));

            StringAssert.Contains("Cleanliness Rating must be between 1 and 5", exceptionThrown.Message);
        }

        [TestMethod]
        public void ValidateReview_EmptyMessage_ThrowsArgumentException()
        {
            
            var review = new Review(1, _testUser, "", 5, 5, 5, 5);

            var exceptionThrown = Assert.ThrowsExactly<ArgumentException>(() =>
                _reviewService.ValidateReview(review));

            StringAssert.Contains("Message cannot be null or empty", exceptionThrown.Message);
        }

        [TestMethod]
        public void ValidateReview_NullUser_ThrowsArgumentException()
        {
            
            var review = new Review(1, null, "No user", 5, 5, 5, 5);

            var exceptionThrown = Assert.ThrowsExactly<ArgumentException>(() =>
                _reviewService.ValidateReview(review));

            StringAssert.Contains("User cannot be null", exceptionThrown.Message);
        }

        [TestMethod]
        public void ValidateReview_DuplicateReview_ThrowsArgumentException()
        {
            var existingReview = new Review(1, _testUser, "I already exist", 5, 5, 5, 5);

            _reviewRepository.GetAll().Returns(new List<Review> { existingReview });

            var exceptionThrown = Assert.ThrowsExactly<ArgumentException>(() =>
                _reviewService.ValidateReview(existingReview));
            StringAssert.Contains("Review already exists", exceptionThrown.Message);
        }

        [TestMethod]
        public void GetById_ValidId_ReturnsReviewFromRepository()
        {
            
            var expectedReview = new Review(1, _testUser, "Great", 5, 5, 5, 5);
            _reviewRepository.GetById(1).Returns(expectedReview);

            
            var resultedReview = _reviewService.GetById(1);

            
            Assert.AreEqual(expectedReview, resultedReview);
            _reviewRepository.Received(1).GetById(1); 
        }

        [TestMethod]
        public void UpdateById_WhenCalled_CallsRepositoryUpdate()
        {
           
            var updatedReview = new Review(1, _testUser, "Updated Message", 4, 4, 4, 4);
            _reviewService.UpdateById(1, updatedReview);
            _reviewRepository.Received(1).UpdateById(1, updatedReview);
        }

        [TestMethod]
        public void DeleteById_WhenCalled_CallsRepositoryDelete()
        {
            _reviewService.DeleteById(10);
            _reviewRepository.Received(1).DeleteById(10);
        }

        [TestMethod]
        public void GetAll_WhenCalled_ReturnsListOfReviews()
        {
            
            var reviews = new List<Review>
        {
        new Review(1, _testUser, "R1", 5, 5, 5, 5),
        new Review(2, _testUser, "R2", 4, 4, 4, 4)
        };
            _reviewRepository.GetAll().Returns(reviews);

            
            var resultedReviewList = _reviewService.GetAll();

            
            Assert.AreEqual(2, resultedReviewList.Count);
            _reviewRepository.Received(1).GetAll();
        }

        [TestMethod]
        public void ValidateReview_FlightExperienceRatingInvalid_ThrowsArgumentException()
        {
            
            var review = new Review(1, _testUser, "Test", 5, 0, 5, 5);

            
            var exceptionThrown = Assert.ThrowsExactly<ArgumentException>(() =>
                _reviewService.ValidateReview(review));

            StringAssert.Contains("Flight Experience Rating must be between 1 and 5", exceptionThrown.Message);
        }

        [TestMethod]
        public void ValidateReview_StaffFriendlinessRatingInvalid_ThrowsArgumentException()
        {
            
            var review = new Review(1, _testUser, "Test", 5, 5, 6, 5);

            
            var exceptionThrown = Assert.ThrowsExactly<ArgumentException>(() =>
                _reviewService.ValidateReview(review));

            StringAssert.Contains("Staff Friendliness Rating must be between 1 and 5", exceptionThrown.Message);
        }

        [TestMethod]
        public void ValidateReview_CleanlinessRatingInvalid_ThrowsArgumentException()
        {
            
            var review = new Review(1, _testUser, "Test", 5, 5, 5, 0);

            
            var exceptionThrown = Assert.ThrowsExactly<ArgumentException>(() =>
                _reviewService.ValidateReview(review));

            StringAssert.Contains("Cleanliness Rating must be between 1 and 5", exceptionThrown.Message);
        }

        [TestMethod]
        public void ValidateReview_WithAllValidData_DoesNotThrowAndMovesToNextStep()
        {
            
            var validReview = new Review(1, _testUser, "Everything was perfect!", 5, 5, 5, 5);

            _reviewService.ValidateReview(validReview);
        }

        [TestMethod]
        public void ValidateReview_StaffFriendlinessBelowMin_ThrowsArgumentException()
        {
            
            var review = new Review(1, _testUser, "Test", 5, 5, 0, 5);

            Assert.ThrowsExactly<ArgumentException>(() => _reviewService.ValidateReview(review));
        }
    }
}