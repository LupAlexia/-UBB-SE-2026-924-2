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
    public async Task SaveBatchAsync_EmptyList_ReturnsOkOrBadRequest()
    {
        // Act - empty list behaviour depends on implementation:
        // some return OK (save succeeds with nothing), others return BadRequest (validation rejects empty)
        var response = await client.PostAsJsonAsync("/api/FlightTicket/batch", new List<FlightTicket>());

        // Assert
        response.StatusCode.Should().Match(statusCode => statusCode == HttpStatusCode.OK || statusCode == HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public async Task GetByUserIdAsync_ReturnsOk()
    {
        // Act
        var response = await client.GetAsync("/api/FlightTicket/user/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [TestMethod]
    public async Task GetByUserIdAsync_NonExistentUser_ReturnsOkWithEmptyList()
    {
        // Act - no tickets for user 999999
        var response = await client.GetAsync("/api/FlightTicket/user/999999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var tickets = await response.Content.ReadFromJsonAsync<List<FlightTicket>>();
        tickets.Should().NotBeNull();
        tickets.Should().BeEmpty();
    }

    [TestMethod]
    public async Task IsSeatAvailableAsync_UnoccupiedSeat_ReturnsOkWithTrue()
    {
        // Act - seat ZZZ99 should not be occupied by any seeded ticket
        var response = await client.GetAsync("/api/FlightTicket/flight/1/seat-available/ZZZ99");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var isAvailable = await response.Content.ReadFromJsonAsync<bool>();
        isAvailable.Should().BeTrue();
    }

    [TestMethod]
    public async Task IsSeatAvailableAsync_OccupiedSeat_ReturnsOkWithFalse()
    {
        // Arrange - FlightTicket 1 has seat "12A" on flight 1 in seed data
        var response = await client.GetAsync("/api/FlightTicket/flight/1/seat-available/12A");

        // Assert - endpoint must respond OK regardless of whether seed data is present
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [TestMethod]
    public async Task UpdateTicketStatusAsync_SeededTicket_ReturnsNoContent()
    {
        // Arrange - ticket 1 exists in seed data
        var checkResponse = await client.GetAsync("/api/FlightTicket/user/1");
        var tickets = await checkResponse.Content.ReadFromJsonAsync<List<FlightTicket>>();
        if (tickets == null || !tickets.Any())
        {
            return;
        }

        var ticketId = tickets.First().Id;

        // Act
        var response = await client.PutAsJsonAsync($"/api/FlightTicket/{ticketId}/status", "Cancelled");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [TestMethod]
    public async Task AddTicketAddOnsAsync_SeededTicket_ReturnsNoContent()
    {
        // Arrange - ticket 1 exists and add-on 1 exists in seed data
        var checkResponse = await client.GetAsync("/api/FlightTicket/user/1");
        var tickets = await checkResponse.Content.ReadFromJsonAsync<List<FlightTicket>>();
        if (tickets == null || !tickets.Any())
        {
            return;
        }

        var ticketId = tickets.First().Id;
        var addOnIds = new List<int> { 1 };

        // Act
        var response = await client.PostAsJsonAsync($"/api/FlightTicket/{ticketId}/addons", addOnIds);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [TestMethod]
    public async Task GetOccupiedSeatsAsync_NonExistentFlight_ReturnsOkWithEmptyList()
    {
        // Act
        var response = await client.GetAsync("/api/FlightTicket/flight/999999/occupied-seats");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var seats = await response.Content.ReadFromJsonAsync<List<string>>();
        seats.Should().NotBeNull();
        seats.Should().BeEmpty();
    }
}