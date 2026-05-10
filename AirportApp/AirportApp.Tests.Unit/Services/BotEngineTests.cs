using AirportApp.ClassLibrary.Entity.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AirportApp.Tests.Unit.Services
{
    [TestClass]
    public class BotEngineTests
    {
        private const int TestUserId = 1;
        private const int TestChatId = 1;
        private const int TestMessageId = 1;
        private const string TestUserName = "Test";
        private const string TestUserEmail = "test@test.com";
        private const string MockStrategyResponse = "I am the mocked strategy response";
        private const string ShortReply = "reply";

        private IBotStrategy mockStrategy = null!;
        private BotEngineIdentity botEngine = null!;
        private User testUser = null!;

        [TestInitialize]
        public void Setup()
        {
            mockStrategy = Substitute.For<IBotStrategy>();
            botEngine = new BotEngineIdentity(mockStrategy);
            testUser = new User(TestUserId, TestUserName, TestUserEmail);
        }

        [TestMethod]
        public async Task GenerateResponse_ValidMessage_ReturnsStrategyResult()
        {
            var mockMessage = Substitute.For<IMessage>();
            var dummyChat = new Chat(TestChatId, testUser, ChatStatus.Active);
            var expectedResponse = new BotMessage.BotMessageBuilder(botEngine, dummyChat, TestMessageId)
                .WithMessage(MockStrategyResponse)
                .Build();

            mockStrategy.ProcessIncomingUserMessageAndDetermineNextDecisionTreeNodeAsync(botEngine, mockMessage)
                .Returns(expectedResponse);

            var result = await botEngine.GenerateAppropriateResponseBasedOnCurrentStrategyAsync(mockMessage);

            Assert.AreEqual(expectedResponse, result);
        }

        [TestMethod]
        public async Task GenerateResponse_DelegatesToStrategy_ExactlyOnce()
        {
            var mockMessage = Substitute.For<IMessage>();
            var dummyChat = new Chat(TestChatId, testUser, ChatStatus.Active);
            var response = new BotMessage.BotMessageBuilder(botEngine, dummyChat, TestMessageId)
                .WithMessage(ShortReply)
                .Build();

            mockStrategy.ProcessIncomingUserMessageAndDetermineNextDecisionTreeNodeAsync(botEngine, mockMessage)
                .Returns(response);

            await botEngine.GenerateAppropriateResponseBasedOnCurrentStrategyAsync(mockMessage);

            await mockStrategy.Received(1).ProcessIncomingUserMessageAndDetermineNextDecisionTreeNodeAsync(botEngine, mockMessage);
        }

        [TestMethod]
        public async Task ResetBotConversationState_DelegatesToStrategy_ExactlyOnce()
        {
            await botEngine.ResetBotConversationStateToInitialRootNodeAsync();

            await mockStrategy.Received(1).ResetCurrentlyActiveConversationNodeToInitialStartingPointAsync();
        }
    }
}