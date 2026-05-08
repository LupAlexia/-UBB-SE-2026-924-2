using AirportApp.Src.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirportApp.Src.ViewModel.Review;
using AirportApp.Src.Service;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.Entity.Domain.Review;
using NSubstitute;

namespace AirportApp.Tests.Unit.Src.ViewModel
{
    [TestClass]
    public class AddReviewViewModelTests
    {
        private AddReviewViewModel viewModel;
        private ReviewService reviewService;
        private IRepository<int, Review> mockRepository;

        [TestInitialize]
        public void Setup()
        {
            mockRepository = Substitute.For<IRepository<int, Review>>();
            reviewService = new ReviewService(mockRepository);

            viewModel = new AddReviewViewModel(reviewService);
        }

        [TestMethod]
        public void DutyText_WhenRatingIsZero_ReturnsNotRated()
        {
            viewModel.DutyRating = 0;

            Assert.AreEqual("Not rated", viewModel.DutyText);
        }

        [TestMethod]
        public void DutyText_WhenRatingIsPositive_ReturnsFormattedString()
        {
            viewModel.DutyRating = 4;

            Assert.AreEqual("4/5", viewModel.DutyText);
        }

        [TestMethod]
        public void SubmitReviewCommand_WhenFieldsAreEmpty_CannotExecute()
        {
            viewModel.ReviewMessage = string.Empty;

            bool canExecute = viewModel.SubmitReviewCommand.CanExecute(null);

            Assert.IsFalse(canExecute, "Command should be disabled if ratings are 0");
        }

        [TestMethod]
        public void SubmitReviewCommand_WhenAllFieldsFilled_CanExecute()
        {
            viewModel.DutyRating = 5;
            viewModel.FlightRating = 5;
            viewModel.StaffRating = 5;
            viewModel.CleanRating = 5;
            viewModel.ReviewMessage = "Great experience!";

            bool canExecuteCommand = viewModel.SubmitReviewCommand.CanExecute(null);

            Assert.IsTrue(canExecuteCommand, "Command should be enabled when all data is valid");
        }

        [TestMethod]
        public void SubmitReview_WhenUserIsNull_TriggersNotLoggedInAlert()
        {
            bool alertFired = false;
            string? alertTitle = null;

            viewModel.AlertRequested += (sender, arguments) =>
            {
                alertFired = true;
                alertTitle = arguments.Title;
            };

            viewModel.SubmitReviewCommand.Execute(null);

            Assert.IsTrue(alertFired, "The AlertRequested event should have been raised.");
            Assert.AreEqual("Not Logged In", alertTitle);
        }

        [TestMethod]
        public void CharCountText_WhenMessageIsUpdated_ReturnsCorrectCount()
        {
            viewModel.ReviewMessage = "Hello";
            Assert.AreEqual("5 characters", viewModel.CharCountText);

            viewModel.ReviewMessage = string.Empty;

            Assert.AreEqual("0 characters", viewModel.CharCountText);
        }

        [TestMethod]
        public void SubmitReview_AfterAttempt_ResetsProperties()
        {
            viewModel.DutyRating = 5;
            viewModel.ReviewMessage = "Excellent";

            viewModel.SubmitReviewCommand.Execute(null);

            Assert.AreEqual("9 characters", viewModel.CharCountText);
        }

        [TestMethod]
        public void CharCountText_WhenMessageIsNull_ReturnsZeroCharacters()
        {
            viewModel.ReviewMessage = null!;

            Assert.AreEqual("0 characters", viewModel.CharCountText);
        }

        [TestMethod]
        public void FlightText_WhenRatingIsZeroOrPositive_ReturnsCorrectStrings()
        {
            viewModel.FlightRating = 0;
            Assert.AreEqual("Not rated", viewModel.FlightText);

            viewModel.FlightRating = 3;

            Assert.AreEqual("3/5", viewModel.FlightText);
        }

        [TestMethod]
        public void StaffText_WhenRatingIsZeroOrPositive_ReturnsCorrectStrings()
        {
            viewModel.StaffRating = 0;
            Assert.AreEqual("Not rated", viewModel.StaffText);

            viewModel.StaffRating = 2;

            Assert.AreEqual("2/5", viewModel.StaffText);
        }

        [TestMethod]
        public void CleanText_WhenRatingIsZeroOrPositive_ReturnsCorrectStrings()
        {
            viewModel.CleanRating = 0;
            Assert.AreEqual("Not rated", viewModel.CleanText);

            viewModel.CleanRating = 5;

            Assert.AreEqual("5/5", viewModel.CleanText);
        }
    }
}



