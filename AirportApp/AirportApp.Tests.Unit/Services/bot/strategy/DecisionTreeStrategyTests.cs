using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Domain.Chats;
using AirportApp.ClassLibrary.Entity.Domain.Faq.Bot;
using AirportApp.ClassLibrary.Entity.Domain.Message;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service.Bot;
using AirportApp.Src.Service.Bot.Strategy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace AirportApp.Tests.Unit.Src.Service.Bot.Strategy
{
    [TestClass]
    public class DecisionTreeStrategyTests
    {
        private IRepository<int, FAQNode> _mockRepository = null!;
        private DecisionTreeStrategy _strategy = null!;
        private Dictionary<int, FAQNode> _fakeDatabase = null!;
        private int _restartId;
        private User _testUser;
        private Chat _testChat;

        [TestInitialize]
        public void Setup()
        {
            _mockRepository = Substitute.For<IRepository<int, FAQNode>>();
            _fakeDatabase = new Dictionary<int, FAQNode>();
            _testUser = new User(1, "John Doe", "john@test.com");
            _testChat = new Chat(1, _testUser, ChatStatus.Active);

            var options1 = ImmutableArray.Create(new FAQOption("Go to 2", 2));
            _fakeDatabase[1] = new FAQNode(1, "Root Node", options1, false);

            _fakeDatabase[2] = new FAQNode(2, "Second Node", ImmutableArray<FAQOption>.Empty, true);

            _restartId = (int)BotStandardMessages.RestartConversation;
            if (!_fakeDatabase.ContainsKey(_restartId))
            {
                _fakeDatabase[_restartId] = new FAQNode(_restartId, "Restarting...", ImmutableArray<FAQOption>.Empty, true);
            }

            _mockRepository.GetByIdAsync(Arg.Any<int>()).Returns(callInfo => Task.FromResult(_fakeDatabase[callInfo.Arg<int>()]));

            _strategy = new DecisionTreeStrategy(_mockRepository);
        }

        [TestMethod]
        public async Task ProcessIncomingUserMessage_ValidOption_AdvancesToNextNode()
        {
            var mockMessage = Substitute.For<IMessage>();
            mockMessage.Text.Returns("Go to 2");
            mockMessage.GetChat().Returns(_testChat);

            var mockBotEngine = new BotEngineIdentity(_strategy);
            var resultedSelectedNode = await _strategy.ProcessIncomingUserMessageAndDetermineNextDecisionTreeNodeAsync(mockBotEngine, mockMessage);

            Assert.AreEqual("Second Node", resultedSelectedNode.GetMessage());
        }

        [TestMethod]
        public async Task ProcessIncomingUserMessage_InvalidOption_ReturnsRestartNode()
        {
            var mockMessage = Substitute.For<IMessage>();
            mockMessage.Text.Returns("Some random gibberish");
            mockMessage.GetChat().Returns(_testChat);

            var mockBotEngine = new BotEngineIdentity(_strategy);
            var resultedSelectedNode = await _strategy.ProcessIncomingUserMessageAndDetermineNextDecisionTreeNodeAsync(mockBotEngine, mockMessage);

            Assert.AreEqual(_fakeDatabase[_restartId].QuestionText, resultedSelectedNode.GetMessage());
        }

        [TestMethod]
        public async Task ResetCurrentlyActiveConversationNode_WhenCalled_ResetsToNode1()
        {
            var mockMessage = Substitute.For<IMessage>();
            mockMessage.Text.Returns("Go to 2");
            mockMessage.GetChat().Returns(_testChat);
            
            var mockBotEngineInitial = new BotEngineIdentity(_strategy);
            await _strategy.ProcessIncomingUserMessageAndDetermineNextDecisionTreeNodeAsync(mockBotEngineInitial, mockMessage);

            await _strategy.ResetCurrentlyActiveConversationNodeToInitialStartingPointAsync();

            var mockBotEngine = new BotEngineIdentity(_strategy);
            var resultAfterReset = await _strategy.ProcessIncomingUserMessageAndDetermineNextDecisionTreeNodeAsync(mockBotEngine, mockMessage);
            Assert.AreEqual("Second Node", resultAfterReset.GetMessage());
        }
    }
}
