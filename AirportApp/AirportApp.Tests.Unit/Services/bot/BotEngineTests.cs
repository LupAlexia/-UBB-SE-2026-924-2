using AirportApp.ClassLibrary.Entity.Domain.Chats;
using AirportApp.ClassLibrary.Entity.Domain.Message;
using AirportApp.ClassLibrary.Entity.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AirportApp.Tests.Unit.Src.Service.Bot
{
    [TestClass]
    public class BotEngineTests
    {
        private IBotStrategy _mockStrategy = null!;
        private BotEngineIdentity _botEngine = null!;
        private User _testUser = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockStrategy = Substitute.For<IBotStrategy>();
            _botEngine = new BotEngineIdentity(_mockStrategy);
            _testUser = new User(1, "Test", "test@test.com");
        }

        [TestMethod]
        public async Task GenerateAppropriateResponse_ValidMessage_ReturnsStrategyResult()
        {
            var mockIncomingMessage = Substitute.For<IMessage>();
            var dummyChat = new Chat(1, _testUser, ChatStatus.Active);
            var expectedResponse = new BotMessage.BotMessageBuilder(_botEngine, dummyChat, 1)
                .WithMessage("I am the mocked strategy response")
                .Build();

            _mockStrategy.ProcessIncomingUserMessageAndDetermineNextDecisionTreeNodeAsync(_botEngine, mockIncomingMessage)
                .Returns(expectedResponse);

            var resultedBotResponse = await _botEngine.GenerateAppropriateResponseBasedOnCurrentStrategyAsync(mockIncomingMessage);

            Assert.AreEqual(expectedResponse, resultedBotResponse);
        }

        [TestMethod]
        public async Task ResetBotConversationState_CallsStrategyResetMethod_ExactlyOnce()
        {
            await _botEngine.ResetBotConversationStateToInitialRootNodeAsync();

            await _mockStrategy.Received(1).ResetCurrentlyActiveConversationNodeToInitialStartingPointAsync();
        }

        [TestMethod]
        public void RetrieveConfiguredEmailAddressForBotContact_WhenCalled_ReturnsCorrectEmail()
        {
            var resultedEmail = _botEngine.RetrieveConfiguredEmailAddressForBotContact();
            Assert.AreEqual("customer-support@cloudspritzers.com", resultedEmail);
        }

        [TestMethod]
        public void RetrieveConfiguredDisplayFullNameForBot_WhenCalled_ReturnsCarlos()
        {
            var resultedFullName = _botEngine.RetrieveConfiguredDisplayFullNameForBot();
            Assert.AreEqual("Carlos", resultedFullName);
        }

        [TestMethod]
        public void RetrieveUniqueDatabaseIdentifierForBot_WhenCalled_ReturnsZero()
        {
            var resultedIdentifier = _botEngine.RetrieveUniqueDatabaseIdentifierForBot();
            Assert.AreEqual(0, resultedIdentifier);
        }
    }
}



