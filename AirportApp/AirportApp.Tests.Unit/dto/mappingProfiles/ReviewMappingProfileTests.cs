using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutoMapper;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Entity.Domain;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace AirportApp.Tests.Unit.Dto.MappingProfiles
{
    [TestClass]
    public class ReviewMappingProfileTests
    {
        private IMapper mapper = null!;
        private User testUser = null!;
        private Review testReview = null!;
        private ILoggerFactory loggerFactory = null!;

        [TestInitialize]
        public void Setup()
        {
            loggerFactory = Substitute.For<ILoggerFactory>();
            var configuration = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<ReviewMappingProfile>(), loggerFactory);
            mapper = configuration.CreateMapper();
            testUser = new User(101, "John Doe", "john@example.com");
            testReview = new Review(1, testUser, "Great flight!", 5, 4, 3, 2);
        }

        [TestMethod]
        public void Map_ValidReview_ReturnsNotNullObject()
        {
            var resultDataTransferObject = mapper.Map<ReviewDTO>(testReview);

            Assert.IsNotNull(resultDataTransferObject);
        }

        [TestMethod]
        public void Map_ValidReview_MapsReviewIdCorrectly()
        {
            var resultDataTransferObject = mapper.Map<ReviewDTO>(testReview);

            Assert.AreEqual(testReview.Id, resultDataTransferObject.reviewId);
        }

        [TestMethod]
        public void Map_ValidReview_MapsMessageCorrectly()
        {
            var resultDataTransferObject = mapper.Map<ReviewDTO>(testReview);

            Assert.AreEqual(testReview.Message, resultDataTransferObject.message);
        }

        [TestMethod]
        public void Map_ValidReview_MapsCleanlinessRatingCorrectly()
        {
            var resultDataTransferObject = mapper.Map<ReviewDTO>(testReview);

            Assert.AreEqual(testReview.CleanlinessRating, resultDataTransferObject.cleanlinessRating);
        }

        [TestMethod]
        public void Map_ValidReview_MapsDutyFreeRatingCorrectly()
        {
            var resultDataTransferObject = mapper.Map<ReviewDTO>(testReview);

            Assert.AreEqual(testReview.DutyFreeRating, resultDataTransferObject.dutyFreeRating);
        }

        [TestMethod]
        public void Map_ValidReview_MapsFlightExprienceRatingCorrectly()
        {
            var resultDataTransferObject = mapper.Map<ReviewDTO>(testReview);

            Assert.AreEqual(testReview.FlightExperienceRating, resultDataTransferObject.flightExperienceRating);
        }

        [TestMethod]
        public void Map_ValidReview_MapsStaffFriendlinessRatingCorrectly()
        {
            var resultDataTransferObject = mapper.Map<ReviewDTO>(testReview);

            Assert.AreEqual(testReview.StaffFriendlinessRating, resultDataTransferObject.staffFriendlinessRating);
        }

        [TestMethod]
        public void Map_ValidReview_MapsUserNameCorrectly()
        {
            var resultDataTransferObject = mapper.Map<ReviewDTO>(testReview);

            Assert.AreEqual(testUser.RetrieveConfiguredDisplayFullNameForBot(), resultDataTransferObject.userName);
        }

        [TestMethod]
        public void Map_AllRatingsEqual_OverallRatingCalculatedCorrectly()
        {
            var reviewWithEqualRatings = new Review(2, testUser, "Average", 3, 3, 3, 3);
            var resultDataTransferObject = mapper.Map<ReviewDTO>(reviewWithEqualRatings);

            Assert.AreEqual(3.0, resultDataTransferObject.overallRating);
        }

        [TestMethod]
        public void Map_ZeroRatings_OverallRatingIsZero()
        {
            var reviewWithZeroRatings = new Review(3, testUser, "N/A", 0, 0, 0, 0);
            var resultDataTransferObject = mapper.Map<ReviewDTO>(reviewWithZeroRatings);

            Assert.AreEqual(0.0, resultDataTransferObject.overallRating);
        }
    }
}
