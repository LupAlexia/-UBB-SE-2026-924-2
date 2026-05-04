using CloudSpritzers1.Src.Model.Chats;
using CloudSpritzers1.Src.Model.Faq.Bot;
using CloudSpritzers1.Src.Model.Message;
using CloudSpritzers1.Src.Repository;
using CloudSpritzers1.Src.Service.Bot;
using CloudSpritzers1.Src.Service.Bot.Strategy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace CloudSpritzers1Tests.Src.Service.Bot.Strategy
{
    [TestClass]
    public class DecisionTreeStrategyTests
    {
        private IRepository<int, FAQNode> _mockRepository = null!;
        private DecisionTreeStrategy _strategy = null!;
        private Dictionary<int, FAQNode> _fakeDatabase = null!;
        private int _restartId;

        [TestInitialize]
        public void Setup()
        {
            _mockRepository = Substitute.For<IRepository<int, FAQNode>>();
            _fakeDatabase = new Dictionary<int, FAQNode>();

            var options1 = ImmutableArray.Create(new FAQOption("Go to 2", 2));
            _fakeDatabase[1] = new FAQNode(1, "Root Node", options1, false);

            _fakeDatabase[2] = new FAQNode(2, "Second Node", ImmutableArray<FAQOption>.Empty, true);

            _restartId = (int)BotStandardMessages.RESTART_CONVERSATION;
            if (!_fakeDatabase.ContainsKey(_restartId))
            {
                _fakeDatabase[_restartId] = new FAQNode(_restartId, "Restarting...", ImmutableArray<FAQOption>.Empty, true);
            }

            _mockRepository.GetById(Arg.Any<int>()).Returns(callInfo => _fakeDatabase[callInfo.Arg<int>()]);

            _strategy = new DecisionTreeStrategy(_mockRepository);
        }

        [TestMethod]
        public void ProcessIncomingUserMessage_ValidOption_AdvancesToNextNode()
        {
            var mockMessage = Substitute.For<IMessage>();
            mockMessage.GetMessage().Returns("Go to 2");
            mockMessage.GetChat().Returns(new Chat(1, 1, ChatStatus.Active));

            var resultedSelectedNode = _strategy.ProcessIncomingUserMessageAndDetermineNextDecisionTreeNode(null!, mockMessage);

            Assert.AreEqual("Second Node", resultedSelectedNode.GetMessage());
        }

        [TestMethod]
        public void ProcessIncomingUserMessage_InvalidOption_ReturnsRestartNode()
        {
            var mockMessage = Substitute.For<IMessage>();
            mockMessage.GetMessage().Returns("Some random gibberish");
            mockMessage.GetChat().Returns(new Chat(1, 1, ChatStatus.Active));

            var resultedSelectedNode = _strategy.ProcessIncomingUserMessageAndDetermineNextDecisionTreeNode(null!, mockMessage);

            Assert.AreEqual(_fakeDatabase[_restartId].questionText, resultedSelectedNode.GetMessage());
        }

        [TestMethod]
        public void ResetCurrentlyActiveConversationNode_WhenCalled_ResetsToNode1()
        {
            var mockMessage = Substitute.For<IMessage>();
            mockMessage.GetMessage().Returns("Go to 2");
            mockMessage.GetChat().Returns(new Chat(1, 1, ChatStatus.Active));
            _strategy.ProcessIncomingUserMessageAndDetermineNextDecisionTreeNode(null!, mockMessage);

            _strategy.ResetCurrentlyActiveConversationNodeToInitialStartingPoint();

            var resultAfterReset = _strategy.ProcessIncomingUserMessageAndDetermineNextDecisionTreeNode(null!, mockMessage);
            Assert.AreEqual("Second Node", resultAfterReset.GetMessage());
        }
    }
}