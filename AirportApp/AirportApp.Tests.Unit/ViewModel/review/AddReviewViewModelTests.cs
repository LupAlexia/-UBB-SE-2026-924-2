using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirportApp.Src.ViewModel.Review;
using AirportApp.Src.Service;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.Entity.Domain.Review;
using NSubstitute;




namespace CloudSpritzers1Tests.Src.ViewModel
{
    [TestClass]
    public class AddReviewViewModelTests
    {
        private AddReviewViewModel _viewModel;
        private ReviewService _reviewService;
        private IRepository<int, Review> _mockRepository;

        [TestInitialize]
        public void Setup()
        {
            
            _mockRepository = Substitute.For<IRepository<int, Review>>();
            _reviewService = new ReviewService(_mockRepository);

            _viewModel = new AddReviewViewModel(_reviewService);
        }

        [TestMethod]
        public void DutyText_WhenRatingIsZero_ReturnsNotRated()
        {
            
            _viewModel.DutyRating = 0;

            
            Assert.AreEqual("Not rated", _viewModel.DutyText);
        }

        [TestMethod]
        public void DutyText_WhenRatingIsPositive_ReturnsFormattedString()
        {
            
            _viewModel.DutyRating = 4;

           
            Assert.AreEqual("4/5", _viewModel.DutyText);
        }

        [TestMethod]
        public void SubmitReviewCommand_WhenFieldsAreEmpty_CannotExecute()
        {
            
            _viewModel.ReviewMessage = "";

           
            bool canExecute = _viewModel.SubmitReviewCommand.CanExecute(null);

           
            Assert.IsFalse(canExecute, "Command should be disabled if ratings are 0");
        }

        [TestMethod]
        public void SubmitReviewCommand_WhenAllFieldsFilled_CanExecute()
        {
           
            _viewModel.DutyRating = 5;
            _viewModel.FlightRating = 5;
            _viewModel.StaffRating = 5;
            _viewModel.CleanRating = 5;
            _viewModel.ReviewMessage = "Great experience!";

           
            bool canExecuteCommand = _viewModel.SubmitReviewCommand.CanExecute(null);

            
            Assert.IsTrue(canExecuteCommand, "Command should be enabled when all data is valid");
        }

        [TestMethod]
        public void SubmitReview_WhenUserIsNull_TriggersNotLoggedInAlert()
        {
            bool alertFired = false;
            string? alertTitle = null;

            _viewModel.AlertRequested += (sender, arguments) =>
            {
                alertFired = true;
                alertTitle = arguments.Title;
            };

            _viewModel.SubmitReviewCommand.Execute(null);

            Assert.IsTrue(alertFired, "The AlertRequested event should have been raised.");
            Assert.AreEqual("Not Logged In", alertTitle);
        }

        [TestMethod]
        public void CharCountText_WhenMessageIsUpdated_ReturnsCorrectCount()
        {
           
            _viewModel.ReviewMessage = "Hello";
           
            Assert.AreEqual("5 characters", _viewModel.CharCountText);

            _viewModel.ReviewMessage = "";
           
            Assert.AreEqual("0 characters", _viewModel.CharCountText);
        }

        [TestMethod]
        public void SubmitReview_AfterAttempt_ResetsProperties()
        {
           
            _viewModel.DutyRating = 5;
            _viewModel.ReviewMessage = "Excellent";

            
            _viewModel.SubmitReviewCommand.Execute(null);

            Assert.AreEqual("9 characters", _viewModel.CharCountText);
        }

        [TestMethod]
        public void CharCountText_WhenMessageIsNull_ReturnsZeroCharacters()
        {
            
            _viewModel.ReviewMessage = null!;

            
            Assert.AreEqual("0 characters", _viewModel.CharCountText);
        }

        [TestMethod]
        public void FlightText_WhenRatingIsZeroOrPositive_ReturnsCorrectStrings()
        {
           
            _viewModel.FlightRating = 0;
            Assert.AreEqual("Not rated", _viewModel.FlightText);

            _viewModel.FlightRating = 3;

            
            Assert.AreEqual("3/5", _viewModel.FlightText);
        }

        [TestMethod]
        public void StaffText_WhenRatingIsZeroOrPositive_ReturnsCorrectStrings()
        {
           
            _viewModel.StaffRating = 0;
            Assert.AreEqual("Not rated", _viewModel.StaffText);

            _viewModel.StaffRating = 2;

            Assert.AreEqual("2/5", _viewModel.StaffText);
        }

        [TestMethod]
        public void CleanText_WhenRatingIsZeroOrPositive_ReturnsCorrectStrings()
        {
            
            _viewModel.CleanRating = 0;
            Assert.AreEqual("Not rated", _viewModel.CleanText);

            _viewModel.CleanRating = 5;

            
            Assert.AreEqual("5/5", _viewModel.CleanText);
        }
    }
}