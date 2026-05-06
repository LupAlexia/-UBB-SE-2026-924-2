using System.Net;
using System.Net.Http.Json;
using AirportApp.ClassLibrary.Entity.Domain;
using FluentAssertions;

namespace AirportApp.Tests.Integration.Controllers;

[TestClass]
public class UserAndMembershipApiTests : BaseApiIntegrationTest
{
    [TestMethod]
    public async Task GetAllUsersAsync_ReturnsOk()
    {
        var response = await client.GetAsync("/api/User");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [TestMethod]
    public async Task GetMembershipByIdAsync_ReturnsOkOrNotFound()
    {
        var response = await client.GetAsync("/api/Membership/1");
        // We accept both because we don't know if ID 1 exists in the local DB without seeding
        response.StatusCode.Should().Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
    }

    [TestMethod]
    public async Task GetAllAddOnsAsync_ReturnsOk()
    {
        var response = await client.GetAsync("/api/AddOn");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [TestMethod]
    public async Task GetCustomerByEmailAsync_ReturnsOkOrNotFound()
    {
        // Fix: Use query parameter as expected by CustomerController
        var response = await client.GetAsync("/api/Customer/by-email?email=test@test.com");
        response.StatusCode.Should().Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
    }
}
