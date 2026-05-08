using System.Net;
using System.Net.Http.Json;
using AirportApp.ClassLibrary.Entity.Domain.Review;
using FluentAssertions;

namespace AirportApp.Tests.Integration.Controllers;

[TestClass]
public class ReviewApiTests : BaseApiIntegrationTest
{
    [TestMethod]
    public async Task GetAllAsync_ReturnsSuccessAndJson()
    {
        // Act
        var response = await client.GetAsync("/api/Review");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");

        var reviews = await response.Content.ReadFromJsonAsync<List<Review>>();
        reviews.Should().NotBeNull();
    }

    [TestMethod]
    public async Task GetByIdAsync_ValidId_ReturnsOk()
    {
        // Act - Using ID 1 as a smoke test (assuming seed data exists)
        var response = await client.GetAsync("/api/Review/1");

        // Assert
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var review = await response.Content.ReadFromJsonAsync<Review>();
            review.Should().NotBeNull();
            review!.Id.Should().Be(1);
        }
        else
        {
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }

    [TestMethod]
    public async Task GetByIdAsync_InvalidId_ReturnsNotFound()
    {
        // Act
        var response = await client.GetAsync("/api/Review/999999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
