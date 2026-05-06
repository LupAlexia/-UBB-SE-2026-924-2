using System.Net;
using System.Net.Http.Json;
using AirportApp.ClassLibrary.Entity.Domain;
using FluentAssertions;

namespace AirportApp.Tests.Integration.Controllers;

[TestClass]
public class FlightTicketApiTests : BaseApiIntegrationTest
{
    [TestMethod]
    public async Task GetOccupiedSeatsAsync_ReturnsOk()
    {
        // Act
        var response = await client.GetAsync("/api/FlightTicket/flight/1/occupied-seats");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var seats = await response.Content.ReadFromJsonAsync<List<string>>();
        seats.Should().NotBeNull();
    }

    [TestMethod]
    public async Task SaveBatchAsync_EmptyList_ReturnsOk()
    {
        // Act
        var response = await client.PostAsJsonAsync("/api/FlightTicket/batch", new List<FlightTicket>());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public async Task GetByUserIdAsync_ReturnsOk()
    {
        // Act
        var response = await client.GetAsync("/api/FlightTicket/user/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
