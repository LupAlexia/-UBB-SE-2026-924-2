using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.Src.ViewModel;
using FluentAssertions;
using Moq;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service;
using AirportApp.Tests.Unit.Fixtures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AirportApp.Tests.Unit.Services;

[TestClass]
public class MembershipServiceTests
{
    private const int TargetUserId = 1;
    private const int TargetMembershipId = 2;
    private const float DefaultDiscountPercentage = 15.0f;
    private const string PremiumMembershipName = "Premium";
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

    [TestMethod]
    public void GetAllMemberships_ValidCall_PopulatesDiscounts()
    {
        var defaultMembership = new Membership { Id = TargetMembershipId, Name = PremiumMembershipName };
        var defaultMemberships = new List<Membership> { defaultMembership };
        var defaultDiscounts = new List<MembershipAddonDiscount>
        {
            new MembershipAddonDiscount { DiscountPercentage = DefaultDiscountPercentage }
        };

        mockMembershipRepository.Setup(repoWithMemberships => repoWithMemberships.GetAllMembershipsAsync())
            .ReturnsAsync(defaultMemberships);
        mockMembershipRepository.Setup(repoWithDiscounts => repoWithDiscounts.GetAddonDiscountsAsync(TargetMembershipId))
            .ReturnsAsync(defaultDiscounts);

        var retrievedMemberships = membershipService.GetAllMembershipsAsync().Result.ToList();

        retrievedMemberships.Should().ContainSingle();
        retrievedMemberships.First().AddonDiscounts.Should().BeEquivalentTo(defaultDiscounts);
        mockMembershipRepository.Verify(repoToVerifyDiscounts => repoToVerifyDiscounts.GetAddonDiscountsAsync(TargetMembershipId), Times.Once);
    }

    [TestMethod]
    public void UpgradeUserMembership_ValidUser_UpdatesAndReturnsMembership()
    {
        var defaultMembership = new Membership { Id = TargetMembershipId, Name = PremiumMembershipName };
        var defaultDiscounts = new List<MembershipAddonDiscount>
        {
            new MembershipAddonDiscount { DiscountPercentage = DefaultDiscountPercentage }
        };

        mockMembershipRepository.Setup(repoWithMembership => repoWithMembership.GetMembershipByIdAsync(TargetMembershipId))
            .ReturnsAsync(defaultMembership);
        mockMembershipRepository.Setup(repoWithDiscounts => repoWithDiscounts.GetAddonDiscountsAsync(TargetMembershipId))
            .ReturnsAsync(defaultDiscounts);

        var upgradedMembership = membershipService.UpgradeUserMembershipAsync(TargetUserId, TargetMembershipId).Result;

        upgradedMembership.Should().NotBeNull();
        upgradedMembership!.Name.Should().Be(PremiumMembershipName);
        upgradedMembership.AddonDiscounts.Should().BeEquivalentTo(defaultDiscounts);

        mockUserRepository.Verify(repoToVerifyUpdate => repoToVerifyUpdate.UpdateUserMembershipAsync(TargetUserId, TargetMembershipId), Times.Once);
    }

    [TestMethod]
    public void PurchaseMembership_ValidPurchase_SucceedsAndUpdatesSession()
    {
        var defaultSessionUser = new Customer { Id = TargetUserId, Email = "test@test.com" };
        UserSession.CurrentUser = defaultSessionUser;
        var defaultMembership = new Membership { Id = TargetMembershipId, Name = PremiumMembershipName };

        mockMembershipRepository.Setup(repoWithMembership => repoWithMembership.GetMembershipByIdAsync(TargetMembershipId))
            .ReturnsAsync(defaultMembership);
        mockMembershipRepository.Setup(repoWithDiscounts => repoWithDiscounts.GetAddonDiscountsAsync(TargetMembershipId))
            .ReturnsAsync(new List<MembershipAddonDiscount>());

        var purchaseResult = membershipService.PurchaseMembershipAsync(TargetUserId, TargetMembershipId).Result;

        purchaseResult.Succeeded.Should().BeTrue();
        purchaseResult.Message.Should().Be(ExpectedSuccessMessage);
        UserSession.CurrentUser.Membership.Should().Be(defaultMembership);
    }

    [TestMethod]
    public void PurchaseMembership_ExceptionThrown_ReturnsFailureResult()
    {
        mockUserRepository.Setup(repoWithException => repoWithException.UpdateUserMembershipAsync(TargetUserId, TargetMembershipId))
            .ThrowsAsync(new Exception("Database connection failed"));

        var purchaseResult = membershipService.PurchaseMembershipAsync(TargetUserId, TargetMembershipId).Result;

        purchaseResult.Succeeded.Should().BeFalse();
        purchaseResult.Message.Should().Be(ExpectedFailureMessage);
    }
}
