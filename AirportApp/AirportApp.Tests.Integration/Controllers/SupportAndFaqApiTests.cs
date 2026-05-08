using System.Net;
using System.Net.Http.Json;
using AirportApp.ClassLibrary.Entity.Domain.Faq.Bot;
using AirportApp.ClassLibrary.Entity.Domain.Chats;
using AirportApp.ClassLibrary.Entity.Domain.Message;
using FluentAssertions;

namespace AirportApp.Tests.Integration.Controllers;

[TestClass]
public class SupportAndFaqApiTests : BaseApiIntegrationTest
{
    [TestMethod]
    public async Task GetFaqRootAsync_ReturnsOk()
    {
        var response = await client.GetAsync("/api/FAQ");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [TestMethod]
    public async Task GetNextDecisionNodeAsync_InvalidChoice_ReturnsNotFound()
    {
        // DecisionTreeController.GetNextNodeAsync(parentId, choice)
        // Testing the "NotFound" logic I spotted earlier
        var response = await client.GetAsync("/api/DecisionTree/next/1/InvalidChoiceThatDoesNotExist");

        // Even if parent 1 doesn't exist, the repo returns empty list,
        // and FirstOrDefault returns null -> 404.
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [TestMethod]
    public async Task GetAllChatsAsync_ReturnsOk()
    {
        var response = await client.GetAsync("/api/Chat");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [TestMethod]
    public async Task GetChatMessagesAsync_ReturnsOk()
    {
        var response = await client.GetAsync("/api/Message/chat/1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
