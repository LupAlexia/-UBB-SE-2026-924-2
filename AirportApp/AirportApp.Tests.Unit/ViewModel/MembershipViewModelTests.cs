using System.Collections.Generic;
using Moq;
using Xunit;
using TicketManager.Domain;
using TicketManager.Service;
using TicketManager.ViewModel;

namespace TicketManager.Tests.Unit.ViewModel
{
    public class MembershipViewModelTests
    {
        private readonly Mock<IMembershipService> mockMembershipService;
        private readonly Mock<INavigationService> mockNavigationService;

        public MembershipViewModelTests()
        {
            mockMembershipService = new Mock<IMembershipService>();
            mockNavigationService = new Mock<INavigationService>();

            // Ensure a clean session for every test execution
            UserSession.CurrentUser = null;
        }

        [Fact]
        public void Initialization_ValidServiceCall_PopulatesMembershipsCollection()
        {
            const int ExpectedMembershipId = 1;
            const string ExpectedMembershipName = "Gold";
            const int ExpectedDiscountPercentage = 10;

            var memberships = new List<Membership>
            {
                new Membership { MembershipId = ExpectedMembershipId, Name = ExpectedMembershipName, FlightDiscountPercentage = ExpectedDiscountPercentage }
            };

            mockMembershipService.Setup(service => service.GetAllMemberships()).Returns(memberships);

            var viewModel = new MembershipViewModel(mockMembershipService.Object, mockNavigationService.Object);

            Assert.True(viewModel.Memberships.Count > 0, "Memberships collection should be populated on initialization.");
            Assert.Equal(ExpectedMembershipName, viewModel.Memberships[0].Name);
        }

        [Fact]
        public void PurchaseCommand_UserNotLoggedIn_NavigatesToAuthenticationPage()
        {
            UserSession.CurrentUser = null;
            const int DummyMembershipId = 1;

            var viewModel = new MembershipViewModel(mockMembershipService.Object, mockNavigationService.Object);
            viewModel.PurchaseCommand.Execute(DummyMembershipId);

            mockNavigationService.Verify(service => service.NavigateTo(typeof(TicketManager.View.AuthPage), It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public void PurchaseCommand_ValidMembershipPurchase_SetsPurchaseSucceededToTrue()
        {
            const int ExpectedUserId = 1;
            const int ExpectedMembershipId = 2;
            const string ExpectedSuccessMessage = "Purchase successful!";

            UserSession.CurrentUser = new User { UserId = ExpectedUserId };

            mockMembershipService.Setup(service => service.PurchaseMembership(ExpectedUserId, ExpectedMembershipId))
                .Returns(new MembershipPurchaseResult { Succeeded = true, Message = ExpectedSuccessMessage });

            var viewModel = new MembershipViewModel(mockMembershipService.Object, mockNavigationService.Object);
            viewModel.PurchaseCommand.Execute(ExpectedMembershipId);

            Assert.True(viewModel.PurchaseSucceeded, "PurchaseSucceeded should be true for a successful purchase.");
            Assert.Equal(ExpectedSuccessMessage, viewModel.PurchaseResultMessage);
        }
    }
}
