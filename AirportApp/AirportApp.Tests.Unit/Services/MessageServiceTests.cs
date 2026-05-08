using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.Src.ViewModel;
using AirportApp.ClassLibrary.Entity.Domain.Chats;
using AirportApp.ClassLibrary.Entity.Domain.Faq.Bot;
using AirportApp.ClassLibrary.Entity.Domain.Message;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AirportApp.Tests.Unit.Src.Service
{
    [TestClass]
    public class MessageServiceTests
    {
        private IRepository<int, Chat> mockChatRepository = null!;
        private IRepository<int, Message> mockMessageRepository = null!;
        private IBotStrategy mockStrategy = null!;
        private BotEngineIdentity realBotEngine = null!;
        private MessageService messageService = null!;
        private User testUser = null!;

        private class TestSender : ISender
        {
            public int RetrieveUniqueDatabaseIdentifierForBot() => 1;
            public string RetrieveConfiguredDisplayFullNameForBot() => "Test User";
            public string RetrieveConfiguredEmailAddressForBotContact() => "user@test.com";
        }
        private TestSender testSender = null!;

        [TestInitialize]
        public void Setup()
        {
            mockChatRepository = Substitute.For<IRepository<int, Chat>>();
            mockMessageRepository = Substitute.For<IRepository<int, Message>>();
            mockStrategy = Substitute.For<IBotStrategy>();
            realBotEngine = new BotEngineIdentity(mockStrategy);
            messageService = new MessageService(mockChatRepository, mockMessageRepository, realBotEngine);
            testUser = new User(1, "Test", "test@test.com");
            testSender = new TestSender();
        }

        [TestMethod]
        public void Constructor_WithNullChatRepo_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new MessageService(null!, mockMessageRepository, realBotEngine));
        }

        [TestMethod]
        public void Constructor_WithNullMessageRepo_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new MessageService(mockChatRepository, null!, realBotEngine));
        }

        [TestMethod]
        public void Constructor_WithNullBotEngine_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new MessageService(mockChatRepository, mockMessageRepository, null!));
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
            var selectedChatOption = new FAQOption("Hello", 2);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => messageService.SendMessageAsync(1, testSender, selectedChatOption));
        }

        [TestMethod]
        public async Task SendMessage_WithInactiveChat_ThrowsCorrectMessage()
        {
            var closedChat = new Chat(1, testUser, ChatStatus.Closed);
            mockChatRepository.GetByIdAsync(1).Returns(closedChat);
            var selectedChatOption = new FAQOption("Hello", 2);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => messageService.SendMessageAsync(1, testSender, selectedChatOption));
            Assert.AreEqual("Chat 1 is not active.", ex.Message);
        }

        [TestMethod]
        public async Task SendMessage_WithValidInput_ReturnsCorrectBotReplyMessage()
        {
            var activeChat = new Chat(1, testUser, ChatStatus.Active);
            mockChatRepository.GetByIdAsync(1).Returns(activeChat);
            var selectedChatOption = new FAQOption("Help me", 2);
            var expectedReply = new BotMessage.BotMessageBuilder(realBotEngine, activeChat, 2).WithMessage("I can help").Build();
            mockStrategy.ProcessIncomingUserMessageAndDetermineNextDecisionTreeNodeAsync(Arg.Any<BotEngineIdentity>(), Arg.Any<IMessage>()).Returns(expectedReply);

            var resultedChatMessage = await messageService.SendMessageAsync(1, testSender, selectedChatOption);

            Assert.AreEqual("I can help", resultedChatMessage.GetMessage());
        }

        [TestMethod]
        public async Task SendMessage_WithValidInput_PersistsBothUserAndBotMessages()
        {
            var activeChat = new Chat(1, testUser, ChatStatus.Active);
            mockChatRepository.GetByIdAsync(1).Returns(activeChat);
            var selectedChatOption = new FAQOption("Help me", 2);
            var expectedReply = new BotMessage.BotMessageBuilder(realBotEngine, activeChat, 2).WithMessage("I can help").Build();
            mockStrategy.ProcessIncomingUserMessageAndDetermineNextDecisionTreeNodeAsync(Arg.Any<BotEngineIdentity>(), Arg.Any<IMessage>()).Returns(expectedReply);

            await messageService.SendMessageAsync(1, testSender, selectedChatOption);

            await mockMessageRepository.Received(2).CreateNewEntityAsync(Arg.Any<Message>());
        }

        [TestMethod]
        public async Task SendMessage_WithOptionId1_ResetsBotStrategy()
        {
            var activeChat = new Chat(1, testUser, ChatStatus.Active);
            mockChatRepository.GetByIdAsync(1).Returns(activeChat);
            var restartOption = new FAQOption("Restart", 1);
            var expectedReply = new BotMessage.BotMessageBuilder(realBotEngine, activeChat, 1).WithMessage("Restarting").Build();
            mockStrategy.ProcessIncomingUserMessageAndDetermineNextDecisionTreeNodeAsync(Arg.Any<BotEngineIdentity>(), Arg.Any<IMessage>()).Returns(expectedReply);

            await messageService.SendMessageAsync(1, testSender, restartOption);

            await mockStrategy.Received(1).ResetCurrentlyActiveConversationNodeToInitialStartingPointAsync();
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



