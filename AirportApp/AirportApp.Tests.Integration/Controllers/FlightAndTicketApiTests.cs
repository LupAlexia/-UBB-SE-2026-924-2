using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace AirportApp.Tests.Integration.Controllers;

[TestClass]
public class FlightAndTicketApiTests : BaseApiIntegrationTest
{
    [TestMethod]
    public async Task GetFlightByIdAsync_ReturnsOkOrNotFound()
    {
        var response = await client.GetAsync("/api/Flight/1");
        response.StatusCode.Should().Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
    }

    [TestMethod]
    public async Task GetTicketCategoriesAsync_ReturnsOk()
    {
        var response = await client.GetAsync("/api/TicketCategory");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [TestMethod]
    public async Task GetTicketSubcategoriesAsync_ReturnsOk()
    {
        var response = await client.GetAsync("/api/TicketSubcategory");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [TestMethod]
    public async Task GetAllEmployeesAsync_ReturnsOk()
    {
        var response = await client.GetAsync("/api/Employee");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [TestMethod]
    public async Task GetTicketByIdAsync_ReturnsOkOrNotFound()
    {
        var response = await client.GetAsync("/api/Ticket/1");
        response.StatusCode.Should().Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
    }
}
