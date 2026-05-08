using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service.Bot;
using AirportApp.Src.Service.Bot.Strategy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AirportApp.Tests.Unit.Src.Service.Bot.Strategy
{
    [TestClass]
    public class DecisionTreeStrategyTests
    {
        private IRepository<int, FAQNode> mockRepository = null!;
        private DecisionTreeStrategy strategy = null!;
        private Dictionary<int, FAQNode> fakeDatabase = null!;
        private int restartId;
        private User testUser;
        private Chat testChat;

        [TestInitialize]
        public void Setup()
        {
            mockRepository = Substitute.For<IRepository<int, FAQNode>>();
            fakeDatabase = new Dictionary<int, FAQNode>();
            testUser = new User(1, "John Doe", "john@test.com");
            testChat = new Chat(1, testUser, ChatStatus.Active);

            var options1 = ImmutableArray.Create(new FAQOption("Go to 2", 2));
            fakeDatabase[1] = new FAQNode(1, "Root Node", options1, false);

            fakeDatabase[2] = new FAQNode(2, "Second Node", ImmutableArray<FAQOption>.Empty, true);

            restartId = (int)BotStandardMessages.RestartConversation;
            if (!fakeDatabase.ContainsKey(restartId))
            {
                fakeDatabase[restartId] = new FAQNode(restartId, "Restarting...", ImmutableArray<FAQOption>.Empty, true);
            }

            mockRepository.GetByIdAsync(Arg.Any<int>()).Returns(callInfo => Task.FromResult(fakeDatabase[callInfo.Arg<int>()]));

            strategy = new DecisionTreeStrategy(mockRepository);
        }

        [TestMethod]
        public async Task ProcessIncomingUserMessage_ValidOption_AdvancesToNextNode()
        {
            var mockMessage = Substitute.For<IMessage>();
            mockMessage.Text.Returns("Go to 2");
            mockMessage.GetChat().Returns(testChat);

            var mockBotEngine = new BotEngineIdentity(strategy);
            var resultedSelectedNode = await strategy.ProcessIncomingUserMessageAndDetermineNextDecisionTreeNodeAsync(mockBotEngine, mockMessage);

            Assert.AreEqual("Second Node", resultedSelectedNode.GetMessage());
        }

        [TestMethod]
        public async Task ProcessIncomingUserMessage_InvalidOption_ReturnsRestartNode()
        {
            var mockMessage = Substitute.For<IMessage>();
            mockMessage.Text.Returns("Some random gibberish");
            mockMessage.GetChat().Returns(testChat);

            var mockBotEngine = new BotEngineIdentity(strategy);
            var resultedSelectedNode = await strategy.ProcessIncomingUserMessageAndDetermineNextDecisionTreeNodeAsync(mockBotEngine, mockMessage);

            Assert.AreEqual(fakeDatabase[restartId].questionText, resultedSelectedNode.GetMessage());
        }

        [TestMethod]
        public async Task ResetCurrentlyActiveConversationNode_WhenCalled_ResetsToNode1()
        {
            var mockMessage = Substitute.For<IMessage>();
            mockMessage.Text.Returns("Go to 2");
            mockMessage.GetChat().Returns(testChat);

            var mockBotEngineInitial = new BotEngineIdentity(strategy);
            await strategy.ProcessIncomingUserMessageAndDetermineNextDecisionTreeNodeAsync(mockBotEngineInitial, mockMessage);

            await strategy.ResetCurrentlyActiveConversationNodeToInitialStartingPointAsync();

            var mockBotEngine = new BotEngineIdentity(strategy);
            var resultAfterReset = await strategy.ProcessIncomingUserMessageAndDetermineNextDecisionTreeNodeAsync(mockBotEngine, mockMessage);
            Assert.AreEqual("Second Node", resultAfterReset.GetMessage());
        }
    }
}
