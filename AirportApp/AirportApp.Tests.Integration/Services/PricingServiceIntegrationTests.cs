using FluentAssertions;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service;
using AirportApp.Tests.Unit.Fixtures;
using AirportApp.ClassLibrary.DataAccess;

namespace AirportApp.Tests.Integration.Services;

[TestClass]
public class PricingServiceIntegrationTests : BaseIntegrationTest
{
    private const int UniqueCodeStartIndex = 0;
    private const int UniqueCodeLength = 4;
    private const string MihaiEmail = "mihai.popescu";
    private const string MihaiUsername = "MihaiPopescu";
    private const string MihaiPassword = "Parolalamos123!";
    private const string MihaiPhone = "0722112233";
    private const float TicketPrice1 = 100.0f;
    private const float TicketPrice2 = 100.0f;
    private const int MembershipId = 1;
    private const string MembershipName = "Premium";
    private readonly IPricingService pricingService;
    private readonly IMembershipRepository membershipRepository;
    private readonly ICustomerRepository userRepository;

    public PricingServiceIntegrationTests()
    {
        var dbContext = CreateDbContext();
        membershipRepository = new MembershipRepository(dbContext);
        userRepository = new CustomerRepository(dbContext, membershipRepository);
        pricingService = new PricingService();
    }

    [TestMethod]
    public async Task PricingCalculation_WithMembership_WorksEndToEnd()
    {
        var memberships = await membershipRepository.GetAllMembershipsAsync();
        var membership = memberships.First();
        membership.AddonDiscounts = (await membershipRepository.GetAddonDiscountsAsync(membership.Id)).ToList();

        var uniqueCode = Guid.NewGuid().ToString().Substring(UniqueCodeStartIndex, UniqueCodeLength);
        var user = new Customer
        {
            Email = $"{MihaiEmail}_{uniqueCode}@gmail.com",
            Username = $"{MihaiUsername}_{uniqueCode}",
            Phone = MihaiPhone,
            PasswordHash = MihaiPassword,
            Membership = membership,
        };
        await userRepository.AddUserAsync(user);
        var databaseUser = await userRepository.GetByEmailAsync(user.Email);
        databaseUser!.Membership = membership;

        var flight = FlightFixture.CreateValidTestFlight();
        var ticket1 = new FlightTicket { Price = TicketPrice1, SelectedAddOns = new List<AddOn>() };
        var ticket2 = new FlightTicket { Price = TicketPrice2, SelectedAddOns = new List<AddOn>() };
        var tickets = new List<FlightTicket> { ticket1, ticket2 };

        var breakdown = pricingService.CalculatePriceBreakdown(flight, databaseUser, tickets);

        breakdown.BasePriceTotal.Should().BeGreaterThan(0);
        breakdown.FinalTotal.Should().BeGreaterThan(0);
        breakdown.MembershipSavings.Should().BeGreaterThanOrEqualTo(0);
    }
}
