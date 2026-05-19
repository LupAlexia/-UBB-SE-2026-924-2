using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.Src.ViewModel;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using AirportApp.ClassLibrary.Service.Interfaces;

namespace AirportApp.Tests.Unit.Src.Service
{
    [TestClass]
    public class MessageServiceTests
    {
        private IRepository<int, Chat> mockChatRepository = null!;
        private IMessageRepository mockMessageRepository = null!;
        private IDecisionTreeService mockDecisionTreeService = null!;
        private IBotStrategy mockStrategy = null!;
        private BotEngineIdentity realBotEngine = null!;
        private MessageService messageService = null!;
        private User testUser = null!;

        private class TestSender : Sender
        {
            public override string RetrieveConfiguredDisplayFullNameForBot() => "Test User";
            public override string RetrieveConfiguredEmailAddressForBotContact() => "user@test.com";
            public override int RetrieveUniqueDatabaseIdentifierForBot() => 1;
        }
        private TestSender testSender = null!;

        [TestInitialize]
        public void Setup()
        {
            mockChatRepository = Substitute.For<IRepository<int, Chat>>();
            mockMessageRepository = Substitute.For<IMessageRepository>();
            mockDecisionTreeService = Substitute.For<IDecisionTreeService>();
            mockStrategy = Substitute.For<IBotStrategy>();
            realBotEngine = new BotEngineIdentity(mockStrategy);
            messageService = new MessageService(mockChatRepository, mockMessageRepository, mockDecisionTreeService, realBotEngine);
            testUser = new User(1, "Test", "test@test.com");
            testSender = new TestSender();
        }

        [TestMethod]
        public void Constructor_WithNullChatRepo_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new MessageService(null!, mockMessageRepository, mockDecisionTreeService, realBotEngine));
        }

        [TestMethod]
        public void Constructor_WithNullMessageRepo_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new MessageService(mockChatRepository, null!, mockDecisionTreeService, realBotEngine));
        }

        [TestMethod]
        public void Constructor_WithNullBotEngine_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new MessageService(mockChatRepository, mockMessageRepository, mockDecisionTreeService, null!));
        }

        [TestMethod]
        public void Constructor_WithNullDecisionTreeService_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new MessageService(mockChatRepository, mockMessageRepository, null!, realBotEngine));
        }

        [TestMethod]
        public async Task SendMessage_WithNullOption_ThrowsArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => messageService.SendMessageAsync(1, testSender, null!));
        }

        [TestMethod]
        public async Task SendMessage_WithInactiveChat_ThrowsInvalidOperationException()
        {
            var closedChat = new Chat(1, testUser, ChatStatus.Closed);
            mockChatRepository.GetByIdAsync(1).Returns(closedChat);
            var nextNode = new FAQNode(2, "Next", ImmutableArray<FAQOption>.Empty, false);
            var selectedChatOption = new FAQOption("Hello", nextNode);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => messageService.SendMessageAsync(1, testSender, selectedChatOption));
        }

        [TestMethod]
        public async Task SendMessage_WithInactiveChat_ThrowsCorrectMessage()
        {
            var closedChat = new Chat(1, testUser, ChatStatus.Closed);
            mockChatRepository.GetByIdAsync(1).Returns(closedChat);
            var nextNode = new FAQNode(2, "Next", ImmutableArray<FAQOption>.Empty, false);
            var selectedChatOption = new FAQOption("Hello", nextNode);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => messageService.SendMessageAsync(1, testSender, selectedChatOption));
            Assert.AreEqual("Chat 1 is not active.", ex.Message);
        }

        [TestMethod]
        public async Task SendMessage_WithValidInput_ReturnsCorrectBotReplyMessage()
        {
            var activeChat = new Chat(1, testUser, ChatStatus.Active);
            mockChatRepository.GetByIdAsync(1).Returns(activeChat);
            var nextNode = new FAQNode(2, "Next", ImmutableArray<FAQOption>.Empty, false);
            var selectedChatOption = new FAQOption("Help me", nextNode);
            mockDecisionTreeService.GetNodeByIdAsync(2).Returns(nextNode);

            var resultedChatMessage = await messageService.SendMessageAsync(1, testSender, selectedChatOption);

            Assert.AreEqual("Next", resultedChatMessage.GetMessage());
        }

        [TestMethod]
        public async Task SendMessage_WithValidInput_PersistsBothUserAndBotMessages()
        {
            var activeChat = new Chat(1, testUser, ChatStatus.Active);
            mockChatRepository.GetByIdAsync(1).Returns(activeChat);
            var nextNode = new FAQNode(2, "Next", ImmutableArray<FAQOption>.Empty, false);
            var selectedChatOption = new FAQOption("Help me", nextNode);
            mockDecisionTreeService.GetNodeByIdAsync(2).Returns(nextNode);

            await messageService.SendMessageAsync(1, testSender, selectedChatOption);

            await mockMessageRepository.Received(2).CreateNewEntityAsync(Arg.Any<Message>());
        }

        [TestMethod]
        public async Task SendMessage_WithOptionId1_ResetsBotStrategy()
        {
            var activeChat = new Chat(1, testUser, ChatStatus.Active);
            mockChatRepository.GetByIdAsync(1).Returns(activeChat);
            var restartNode = new FAQNode(1, "Restart", ImmutableArray<FAQOption>.Empty, true);
            var restartOption = new FAQOption("Restart", restartNode);
            mockDecisionTreeService.GetNodeByIdAsync(1).Returns(restartNode);

            await messageService.SendMessageAsync(1, testSender, restartOption);

            await mockDecisionTreeService.Received(1).GetNodeByIdAsync(1);
        }

        [TestMethod]
        public async Task GetMessage_ForWrongChat_ThrowsInvalidOperationException()
        {
            var wrongChat = new Chat(99, testUser, ChatStatus.Active);
            var chatMessage = new Message(testSender, wrongChat, "Text");
            mockMessageRepository.GetByIdAsync(5).Returns(chatMessage);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => messageService.GetMessageAsync(1, 5));
        }

        [TestMethod]
        public async Task GetMessage_ForWrongChat_ThrowsCorrectErrorMessage()
        {
            var wrongChat = new Chat(99, testUser, ChatStatus.Active);
            var chatMessage = new Message(testSender, wrongChat, "Text");
            mockMessageRepository.GetByIdAsync(5).Returns(chatMessage);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => messageService.GetMessageAsync(1, 5));
            Assert.AreEqual("Message 5 does not belong to chat 1.", ex.Message);
        }

        [TestMethod]
        public async Task GetMessage_WithValidMessage_ReturnsMessageText()
        {
            var correctChat = new Chat(1, testUser, ChatStatus.Active);
            var chatMessage = new Message(testSender, correctChat, "Correct Text");
            mockMessageRepository.GetByIdAsync(5).Returns(chatMessage);

            var resultedMessage = await messageService.GetMessageAsync(1, 5);

            Assert.AreEqual("Correct Text", resultedMessage.Text);
        }

        [TestMethod]
        public async Task GetAllMessages_WhenCalled_ReturnsCorrectCount()
        {
            var firstChat = new Chat(1, testUser, ChatStatus.Active);
            mockChatRepository.GetByIdAsync(1).Returns(firstChat);
            var firstMessage = new Message(1, testSender, firstChat, "A", DateTimeOffset.UtcNow);
            var secondMessage = new Message(2, testSender, firstChat, "B", DateTimeOffset.UtcNow);
            mockMessageRepository.GetAllAsync().Returns(new List<Message> { firstMessage, secondMessage });

            var resultedMessages = (await messageService.GetAllMessagesAsync(1)).ToList();

            Assert.AreEqual(2, resultedMessages.Count);
        }

        [TestMethod]
        public async Task GetAllMessages_WhenCalled_ReturnsMessagesOrderedByTimestampAscending()
        {
            var firstChat = new Chat(1, testUser, ChatStatus.Active);
            mockChatRepository.GetByIdAsync(1).Returns(firstChat);
            var earlierMessage = new Message(1, testSender, firstChat, "Earlier", DateTimeOffset.UtcNow.AddMinutes(-10));
            var laterMessage = new Message(2, testSender, firstChat, "Later", DateTimeOffset.UtcNow);
            mockMessageRepository.GetAllAsync().Returns(new List<Message> { laterMessage, earlierMessage });

            var resultedMessages = (await messageService.GetAllMessagesAsync(1)).ToList();

            Assert.AreEqual("Earlier", resultedMessages[0].GetMessage());
        }

        [TestMethod]
        public async Task GetAllMessages_WhenCalled_FiltersOutOtherChats()
        {
            var firstChat = new Chat(1, testUser, ChatStatus.Active);
            var secondChat = new Chat(2, testUser, ChatStatus.Active);
            mockChatRepository.GetByIdAsync(1).Returns(firstChat);
            var firstMessage = new Message(1, testSender, firstChat, "Keep", DateTimeOffset.UtcNow);
            var secondMessage = new Message(2, testSender, secondChat, "Discard", DateTimeOffset.UtcNow);
            mockMessageRepository.GetAllAsync().Returns(new List<Message> { firstMessage, secondMessage });

            var resultedMessages = (await messageService.GetAllMessagesAsync(1)).ToList();

            Assert.AreEqual("Keep", resultedMessages[0].GetMessage());
        }
    }
}



