using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutoMapper;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Entity.Dto.MappingProfiles;
using AirportApp.ClassLibrary.Entity.Domain.Review;
using AirportApp.ClassLibrary.Entity.Domain;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;

namespace AirportApp.Tests.Unit.dto.mappingProfiles
{
    [TestClass]
    public class ReviewMappingProfileTests
    {
        private IMapper _mapper = null!;
        private User _testUser = null!;
        private Review _testReview = null!;
        private ILoggerFactory _loggerFactory = null!;

        [TestInitialize]
        public void Setup()
        {
            _loggerFactory = Substitute.For<ILoggerFactory>();
            var configuration = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<ReviewMappingProfile>(), _loggerFactory);
            _mapper = configuration.CreateMapper();
            _testUser = new User(101, "John Doe", "john@example.com");
            _testReview = new Review(1, _testUser, "Great flight!", 5, 4, 3, 2);
        }

        [TestMethod]
        public void Map_ValidReview_ReturnsNotNullObject()
        {
            var resultDataTransferObject = _mapper.Map<ReviewDTO>(_testReview);

            Assert.IsNotNull(resultDataTransferObject);
        }

        [TestMethod]
        public void Map_ValidReview_MapsReviewIdCorrectly()
        {
            var resultDataTransferObject = _mapper.Map<ReviewDTO>(_testReview);

            Assert.AreEqual(_testReview.Id, resultDataTransferObject.reviewId);
        }

        [TestMethod]
        public void Map_ValidReview_MapsMessageCorrectly()
        {
            var resultDataTransferObject = _mapper.Map<ReviewDTO>(_testReview);

            Assert.AreEqual(_testReview.Message, resultDataTransferObject.message);
        }

        [TestMethod]
        public void Map_ValidReview_MapsCleanlinessRatingCorrectly()
        {
            var resultDataTransferObject = _mapper.Map<ReviewDTO>(_testReview);

            Assert.AreEqual(_testReview.CleanlinessRating, resultDataTransferObject.cleanlinessRating);
        }

        [TestMethod]
        public void Map_ValidReview_MapsDutyFreeRatingCorrectly()
        {
            var resultDataTransferObject = _mapper.Map<ReviewDTO>(_testReview);

            Assert.AreEqual(_testReview.DutyFreeRating, resultDataTransferObject.dutyFreeRating);
        }

        [TestMethod]
        public void Map_ValidReview_MapsFlightExprienceRatingCorrectly()
        {
            var resultDataTransferObject = _mapper.Map<ReviewDTO>(_testReview);

            Assert.AreEqual(_testReview.FlightExperienceRating, resultDataTransferObject.flightExperienceRating);
        }

        [TestMethod]
        public void Map_ValidReview_MapsStaffFriendlinessRatingCorrectly()
        {
            var resultDataTransferObject = _mapper.Map<ReviewDTO>(_testReview);

            Assert.AreEqual(_testReview.StaffFriendlinessRating, resultDataTransferObject.staffFriendlinessRating);
        }

        [TestMethod]
        public void Map_ValidReview_MapsUserNameCorrectly()
        {
            var resultDataTransferObject = _mapper.Map<ReviewDTO>(_testReview);

            Assert.AreEqual(_testUser.RetrieveConfiguredDisplayFullNameForBot(), resultDataTransferObject.userName);
        }

        [TestMethod]
        public void Map_AllRatingsEqual_OverallRatingCalculatedCorrectly()
        {
            var reviewWithEqualRatings = new Review(2, _testUser, "Average", 3, 3, 3, 3);
            var resultDataTransferObject = _mapper.Map<ReviewDTO>(reviewWithEqualRatings);

            Assert.AreEqual(3.0, resultDataTransferObject.overallRating);
        }

        [TestMethod]
        public void Map_ZeroRatings_OverallRatingIsZero()
        {
            var reviewWithZeroRatings = new Review(3, _testUser, "N/A", 0, 0, 0, 0);
            var resultDataTransferObject = _mapper.Map<ReviewDTO>(reviewWithZeroRatings);

            Assert.AreEqual(0.0, resultDataTransferObject.overallRating);
        }
    }
}
