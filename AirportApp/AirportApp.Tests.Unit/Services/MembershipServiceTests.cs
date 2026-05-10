using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AirportApp.Tests.Unit.Services
{
    [TestClass]
    public class MembershipServiceTests
    {
        private const int TargetUserId = 1;
        private const int TargetMembershipId = 2;
        private const int BasicMembershipId = 1;
        private const float DefaultDiscountPercentage = 15.0f;
        private const string PremiumMembershipName = "Premium";
        private const string BasicMembershipName = "Basic";
        private const string SessionUserEmail = "test@test.com";
        private const string DatabaseErrorMessage = "Database connection failed";
        private const string ExpectedSuccessMessage = "Your membership purchase was completed successfully.";
        private const string ExpectedFailureMessage = "Membership purchase could not be completed. Please try again.";

        private readonly Mock<ICustomerRepository> mockUserRepository;
        private readonly Mock<IMembershipRepository> mockMembershipRepository;
        private readonly MembershipService membershipService;

        public MembershipServiceTests()
        {
            mockUserRepository = new Mock<ICustomerRepository>();
            mockMembershipRepository = new Mock<IMembershipRepository>();
            membershipService = new MembershipService(mockUserRepository.Object, mockMembershipRepository.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            UserSession.CurrentUser = null;
        }

        [TestMethod]
        public void GetAllMemberships_ValidCall_PopulatesDiscounts()
        {
            var membership = new Membership { Id = TargetMembershipId, Name = PremiumMembershipName };
            var discounts = new List<MembershipAddonDiscount> { new MembershipAddonDiscount { DiscountPercentage = DefaultDiscountPercentage } };

            mockMembershipRepository.Setup(r => r.GetAllMembershipsAsync()).ReturnsAsync(new List<Membership> { membership });
            mockMembershipRepository.Setup(r => r.GetAddonDiscountsAsync(TargetMembershipId)).ReturnsAsync(discounts);

            var result = membershipService.GetAllMembershipsAsync().Result.ToList();

            result.Should().ContainSingle();
            result.First().AddonDiscounts.Should().BeEquivalentTo(discounts);
            mockMembershipRepository.Verify(r => r.GetAddonDiscountsAsync(TargetMembershipId), Times.Once);
        }

        [TestMethod]
        public void GetAllMemberships_MultipleMemberships_FetchesDiscountsForEach()
        {
            var membership1 = new Membership { Id = BasicMembershipId, Name = BasicMembershipName };
            var membership2 = new Membership { Id = TargetMembershipId, Name = PremiumMembershipName };

            mockMembershipRepository.Setup(r => r.GetAllMembershipsAsync()).ReturnsAsync(new List<Membership> { membership1, membership2 });
            mockMembershipRepository.Setup(r => r.GetAddonDiscountsAsync(It.IsAny<int>())).ReturnsAsync(new List<MembershipAddonDiscount>());

            var result = membershipService.GetAllMembershipsAsync().Result.ToList();

            result.Should().HaveCount(2);
            mockMembershipRepository.Verify(r => r.GetAddonDiscountsAsync(BasicMembershipId), Times.Once);
            mockMembershipRepository.Verify(r => r.GetAddonDiscountsAsync(TargetMembershipId), Times.Once);
        }

        [TestMethod]
        public void GetAllMemberships_EmptyList_ReturnsEmpty()
        {
            mockMembershipRepository.Setup(r => r.GetAllMembershipsAsync()).ReturnsAsync(new List<Membership>());

            var result = membershipService.GetAllMembershipsAsync().Result.ToList();

            result.Should().BeEmpty();
            mockMembershipRepository.Verify(r => r.GetAddonDiscountsAsync(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void UpgradeUserMembership_ValidUser_UpdatesAndReturnsMembership()
        {
            var membership = new Membership { Id = TargetMembershipId, Name = PremiumMembershipName };
            var discounts = new List<MembershipAddonDiscount> { new MembershipAddonDiscount { DiscountPercentage = DefaultDiscountPercentage } };

            mockMembershipRepository.Setup(r => r.GetMembershipByIdAsync(TargetMembershipId)).ReturnsAsync(membership);
            mockMembershipRepository.Setup(r => r.GetAddonDiscountsAsync(TargetMembershipId)).ReturnsAsync(discounts);
            mockUserRepository.Setup(r => r.UpdateUserMembershipAsync(TargetUserId, TargetMembershipId)).Returns(Task.CompletedTask);

            var result = membershipService.UpgradeUserMembershipAsync(TargetUserId, TargetMembershipId).Result;

            result.Should().NotBeNull();
            result!.Name.Should().Be(PremiumMembershipName);
            result.AddonDiscounts.Should().BeEquivalentTo(discounts);
            mockUserRepository.Verify(r => r.UpdateUserMembershipAsync(TargetUserId, TargetMembershipId), Times.Once);
        }

        [TestMethod]
        public void UpgradeUserMembership_MembershipNotFound_ReturnsNull()
        {
            mockMembershipRepository.Setup(r => r.GetMembershipByIdAsync(TargetMembershipId)).ReturnsAsync((Membership?)null);
            mockUserRepository.Setup(r => r.UpdateUserMembershipAsync(TargetUserId, TargetMembershipId)).Returns(Task.CompletedTask);

            var result = membershipService.UpgradeUserMembershipAsync(TargetUserId, TargetMembershipId).Result;

            result.Should().BeNull();
        }

        [TestMethod]
        public void PurchaseMembership_ExceptionThrown_ReturnsFailureResult()
        {
            mockUserRepository.Setup(r => r.UpdateUserMembershipAsync(TargetUserId, TargetMembershipId))
                .ThrowsAsync(new Exception(DatabaseErrorMessage));

            var result = membershipService.PurchaseMembershipAsync(TargetUserId, TargetMembershipId).Result;

            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be(ExpectedFailureMessage);
        }

        [TestMethod]
        public void PurchaseMembership_NullCurrentUser_StillSucceeds()
        {
            UserSession.CurrentUser = null;
            var membership = new Membership { Id = TargetMembershipId, Name = PremiumMembershipName };

            mockMembershipRepository.Setup(r => r.GetMembershipByIdAsync(TargetMembershipId)).ReturnsAsync(membership);
            mockMembershipRepository.Setup(r => r.GetAddonDiscountsAsync(TargetMembershipId)).ReturnsAsync(new List<MembershipAddonDiscount>());
            mockUserRepository.Setup(r => r.UpdateUserMembershipAsync(TargetUserId, TargetMembershipId)).Returns(Task.CompletedTask);

            var result = membershipService.PurchaseMembershipAsync(TargetUserId, TargetMembershipId).Result;

            result.Succeeded.Should().BeTrue();
        }
    }
}