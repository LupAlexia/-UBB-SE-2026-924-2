using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.Src.ViewModel;
using AirportApp.ClassLibrary.Service.Interfaces;
using AirportApp.Src.Service.Interfaces;

namespace AirportApp.Tests.Unit.ViewModel
{
    [TestClass]
    [DoNotParallelize]
    public class MembershipViewModelTests
    {
        private readonly Mock<IMembershipService> mockMembershipService;
        private readonly Mock<INavigationService> mockNavigationService;

        public MembershipViewModelTests()
        {
            mockMembershipService = new Mock<IMembershipService>();
            mockNavigationService = new Mock<INavigationService>();

            mockMembershipService.Setup(service => service.GetAllMembershipsAsync()).ReturnsAsync(new List<Membership>());

            // Ensure a clean session for every test execution
            UserSession.CurrentUser = null;
        }

        [TestMethod]
        public async Task Initialization_ValidServiceCall_PopulatesMembershipsCollectionAsync()
        {
            const int ExpectedMembershipId = 1;
            const string ExpectedMembershipName = "Gold";
            const int ExpectedDiscountPercentage = 10;

            var memberships = new List<Membership>
            {
                new Membership { Id = ExpectedMembershipId, Name = ExpectedMembershipName, FlightDiscountPercentage = ExpectedDiscountPercentage }
            };

            mockMembershipService.Setup(service => service.GetAllMembershipsAsync()).ReturnsAsync(memberships);

            var viewModel = new MembershipViewModel(mockMembershipService.Object, mockNavigationService.Object);
            // Assuming the ViewModel loads memberships in constructor, we might need a small delay or a public Load method
            await Task.Delay(100);

            Assert.IsTrue(viewModel.Memberships.Count > 0, "Memberships collection should be populated on initialization.");
            Assert.AreEqual(ExpectedMembershipName, viewModel.Memberships[0].Name);
        }

        [TestMethod]
        public void PurchaseCommand_UserNotLoggedIn_NavigatesToAuthenticationPage()
        {
            UserSession.CurrentUser = null;
            const int DummyMembershipId = 1;

            var viewModel = new MembershipViewModel(mockMembershipService.Object, mockNavigationService.Object);
            viewModel.PurchaseCommand.Execute(DummyMembershipId);

            mockNavigationService.Verify(service => service.NavigateTo(It.IsAny<Type>(), It.IsAny<object?>()), Times.AtLeastOnce);
        }

        [TestMethod]
        public async Task PurchaseCommand_ValidMembershipPurchase_SetsPurchaseSucceededToTrueAsync()
        {
            const int ExpectedUserId = 1;
            const int ExpectedMembershipId = 2;
            const string ExpectedSuccessMessage = "Purchase successful!";

            UserSession.CurrentUser = new Customer(ExpectedUserId, "test@test.com", "0722112233", "test_user", "hashed_password", null);

            mockMembershipService.Setup(service => service.PurchaseMembershipAsync(ExpectedUserId, ExpectedMembershipId))
                .ReturnsAsync(new MembershipPurchaseResult { Succeeded = true, Message = ExpectedSuccessMessage });

            var viewModel = new MembershipViewModel(mockMembershipService.Object, mockNavigationService.Object);
            viewModel.PurchaseCommand.Execute(ExpectedMembershipId);
            await Task.Delay(100);

            Assert.IsTrue(viewModel.PurchaseSucceeded, "PurchaseSucceeded should be true for a successful purchase.");
            Assert.AreEqual(ExpectedSuccessMessage, viewModel.PurchaseResultMessage);
        }
    }
}
