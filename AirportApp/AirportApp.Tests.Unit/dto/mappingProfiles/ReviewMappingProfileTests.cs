using AutoMapper;
using CloudSpritzers1.Src.Dto;
using CloudSpritzers1.Src.Model;
using CloudSpritzers1.Src.Model.Review;
using CloudSpritzers1.Src.Dto.MappingProfiles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace CloudSpritzers1Tests.Src.Dto.MappingProfiles
{
    [TestClass]
    public class ReviewMappingProfileTests
    {
        private IMapper _mapper;
        private User _testUser;
        private Review _testReview;

        [TestInitialize]
        public void Setup()
        {
            var configuration = new MapperConfiguration(mapperConfiguration => mapperConfiguration.AddProfile<ReviewMappingProfile>());
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

            Assert.AreEqual(_testReview.GetId(), resultDataTransferObject.reviewId);
        }

        [TestMethod]
        public void Map_ValidReview_MapsUserIdCorrectly()
        {
            var resultDataTransferObject = _mapper.Map<ReviewDTO>(_testReview);

            Assert.AreEqual(_testReview.GetUser().UserId, resultDataTransferObject.userId);
        }

        [TestMethod]
        public void Map_ValidReview_MapsUserNameCOrrectly()
        {
            var resultDataTransferObject = _mapper.Map<ReviewDTO>(_testReview);

            Assert.AreEqual("John Doe", resultDataTransferObject.userName);
        }

        [TestMethod]
        public void Map_ValidReview_MapsMessageCorrectly()
        {
            var resultDataTransferObject = _mapper.Map<ReviewDTO>(_testReview);

            Assert.AreEqual(_testReview.GetMessage(), resultDataTransferObject.message);
        }

        [TestMethod]
        public void Map_ValidReview_MapsDutyFreeRatingCorrectly()
        {
            var resultDataTransferObject = _mapper.Map<ReviewDTO>(_testReview);

            Assert.AreEqual(_testReview.GetDutyFreeRating(), resultDataTransferObject.dutyFreeRating);
        }

        [TestMethod]
        public void Map_ValidReview_MapsFlightExprienceRatingCorrectly()
        {
            var resultDataTransferObject = _mapper.Map<ReviewDTO>(_testReview);

            Assert.AreEqual(_testReview.GetFlightExperienceRating(), resultDataTransferObject.flightExperienceRating);
        }

        [TestMethod]
        public void Map_ValidReview_MapsStaffFriendlinessRatingCorrectly()
        {
            var resultDataTransferObject = _mapper.Map<ReviewDTO>(_testReview);

            Assert.AreEqual(_testReview.GetStaffFriendlinessRating(), resultDataTransferObject.staffFriendlinessRating);
        }

        [TestMethod]
        public void Map_ValidReview_MapsCleanlinessRatingCorrectly()
        {
            var resultDataTransferObject = _mapper.Map<ReviewDTO>(_testReview);

            Assert.AreEqual(_testReview.GetCleanlinessRating(), resultDataTransferObject.cleanlinessRating);
        }



        [TestMethod]
        public void Map_AllRatingsEqual_OverallRatingCalculatedCorrectly()
        {
            
            var userEntity = new User(102, "Jane Doe", "jane@example.com");
            var sourceReview = new Review(2, userEntity, "Good", 4, 4, 4, 4);

           
            var resultDataTransferObject = _mapper.Map<ReviewDTO>(sourceReview);

            Assert.AreEqual(4.0f, resultDataTransferObject.overallRating);
        }

        [TestMethod]
        public void Map_ZeroRatings_OverallRatingIsZero()
        {
            
            var userEntity = new User(103, "Bob", "bob@example.com");
            var sourceReview = new Review(3, userEntity, "Bad", 0, 0, 0, 0);

            var resultDataTransferObject = _mapper.Map<ReviewDTO>(sourceReview);

            Assert.AreEqual(0.0f, resultDataTransferObject.overallRating);
        }
    }
}
