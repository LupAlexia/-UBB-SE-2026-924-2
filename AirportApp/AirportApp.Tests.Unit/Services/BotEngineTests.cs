using AirportApp.ClassLibrary.Entity.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AirportApp.Tests.Unit.Services
{
    [TestClass]
    public class BotEngineTests
    {
        private IBotStrategy mockStrategy = null!;
        private BotEngineIdentity botEngine = null!;
        private User testUser = null!;

        [TestInitialize]
        public void Setup()
        {
            mockStrategy = Substitute.For<IBotStrategy>();
            botEngine = new BotEngineIdentity(mockStrategy);
            testUser = new User(1, "Test", "test@test.com");
        }

        [TestMethod]
        public async Task GenerateAppropriateResponse_ValidMessage_ReturnsStrategyResult()
        {
            var mockIncomingMessage = Substitute.For<IMessage>();
            var dummyChat = new Chat(1, testUser, ChatStatus.Active);
            var expectedResponse = new BotMessage.BotMessageBuilder(botEngine, dummyChat, 1)
                .WithMessage("I am the mocked strategy response")
                .Build();

            mockStrategy.ProcessIncomingUserMessageAndDetermineNextDecisionTreeNodeAsync(botEngine, mockIncomingMessage)
                .Returns(expectedResponse);

            var resultedBotResponse = await botEngine.GenerateAppropriateResponseBasedOnCurrentStrategyAsync(mockIncomingMessage);

            Assert.AreEqual(expectedResponse, resultedBotResponse);
        }

        [TestMethod]
        public async Task ResetBotConversationState_CallsStrategyResetMethod_ExactlyOnce()
        {
            await botEngine.ResetBotConversationStateToInitialRootNodeAsync();

            await mockStrategy.Received(1).ResetCurrentlyActiveConversationNodeToInitialStartingPointAsync();
        }

        [TestMethod]
        public void RetrieveConfiguredEmailAddressForBotContact_WhenCalled_ReturnsCorrectEmail()
        {
            var resultedEmail = botEngine.RetrieveConfiguredEmailAddressForBotContact();
            Assert.AreEqual("customer-support@cloudspritzers.com", resultedEmail);
        }

        [TestMethod]
        public void RetrieveConfiguredDisplayFullNameForBot_WhenCalled_ReturnsCarlos()
        {
            var resultedFullName = botEngine.RetrieveConfiguredDisplayFullNameForBot();
            Assert.AreEqual("Carlos", resultedFullName);
        }

        [TestMethod]
        public void RetrieveUniqueDatabaseIdentifierForBot_WhenCalled_ReturnsZero()
        {
            var resultedIdentifier = botEngine.RetrieveUniqueDatabaseIdentifierForBot();
            Assert.AreEqual(0, resultedIdentifier);
        }
    }
}



