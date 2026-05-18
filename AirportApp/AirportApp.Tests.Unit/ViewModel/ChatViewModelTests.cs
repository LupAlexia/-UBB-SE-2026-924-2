using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.Src.ViewModel;
using AutoMapper;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service;
using AirportApp.Src.Service.Bot;
using AirportApp.Src.Service.Bot.Strategy;
using AirportApp.ClassLibrary.Service.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AirportApp.Tests.Unit.ViewModel
{
    [TestClass]
    public class ChatViewModelTests
    {
        private IRepository<int, Chat> chatRepositoryMock;
        private IMessageRepository messageRepositoryMock;
        private IRepository<int, User> userRepositoryMock;
        private IDecisionTreeService faqRepositoryMock;
        private IBotStrategy strategyMock;
        private IUserService userService;
        private IMapper mapper;

        private BotEngineIdentity botEngine;
        private MessageService messageService;
        private ChatService chatService;

        private User testUser;
        private Chat testChat;

        [TestInitialize]
        public void Setup()
        {
            chatRepositoryMock = Substitute.For<IRepository<int, Chat>>();
            messageRepositoryMock = Substitute.For<IMessageRepository>();
            userRepositoryMock = Substitute.For<IRepository<int, User>>();
            faqRepositoryMock = Substitute.For<IDecisionTreeService>();
            strategyMock = Substitute.For<IBotStrategy>();
            userService = Substitute.For<IUserService>();
            mapper = Substitute.For<IMapper>();
            botEngine = new BotEngineIdentity(strategyMock);
            messageService = new MessageService(chatRepositoryMock, messageRepositoryMock, faqRepositoryMock, botEngine);
            chatService = new ChatService(chatRepositoryMock, userRepositoryMock);

            testUser = new User(42, "Test User", "test@test.com");
            testChat = new Chat(1, testUser, ChatStatus.Active);

            chatRepositoryMock.CreateNewEntityAsync(Arg.Any<Chat>()).Returns(Task.FromResult(1));
            chatRepositoryMock.GetByIdAsync(1).Returns(Task.FromResult(testChat));
            userRepositoryMock.GetByIdAsync(42).Returns(Task.FromResult(testUser));

            mapper.Map<MessageDTO>(Arg.Any<IMessage>()).Returns(callInfo =>
            {
                var messageEntity = (IMessage)callInfo[0];
                var dataTransferObject = new MessageDTO();
                var messageSender = messageEntity.GetSender();
                if (messageSender != null)
                {
                    dataTransferObject.Sender = messageSender;
                }
                return dataTransferObject;
            });

            userService.GetByIdAsync(42).Returns(Task.FromResult(testUser));

            // Mock FAQ node for RestartChat
            var faqNode1 = new FAQNode(1, "Test Question", ImmutableArray<FAQOption>.Empty, false);
            faqRepositoryMock.GetNodeByIdAsync(1).Returns(Task.FromResult(faqNode1));
        }

        private ChatViewModel CreateViewModel(List<Message>? initialMessages = null)
        {
            if (initialMessages != null)
            {
                messageRepositoryMock.GetAllAsync().Returns(Task.FromResult((IEnumerable<Message>)initialMessages));
            }
            else
            {
                messageRepositoryMock.GetAllAsync().Returns(Task.FromResult(Enumerable.Empty<Message>()));
            }

            return new ChatViewModel(messageService, chatService, mapper, userService, faqRepositoryMock, testUser);
        }

        [TestMethod]
        public async Task Constructor_HistoryIsEmpty_LoadsFirstMessage()
        {
            faqRepositoryMock.ClearReceivedCalls();

            var newViewModel = CreateViewModel(new List<Message>());
            // Give some time for background tasks if any, though constructor starts them
            await Task.Delay(100);

            await faqRepositoryMock.Received(1).GetNodeByIdAsync(1);
        }

        [TestMethod]
        public void FormatUserId_WhenCalled_ReturnsCorrectlyFormattedString()
        {
            var newViewModel = CreateViewModel();

            Assert.AreEqual("User Id: 42", newViewModel.FormatUserId);
        }

        [TestMethod]
        public async Task CloseChat_WhenCalled_CallsChatRepositoryUpdate()
        {
            var newViewModel = CreateViewModel();

            await newViewModel.CloseChatAsync();

            await chatRepositoryMock.Received(1).UpdateByIdAsync(1, Arg.Any<Chat>());
        }

        [TestMethod]
        public void HandleOptionClick_NullOption_DoesNothing()
        {
            var mockMessage = new Message(1, testUser, testChat, "Init", DateTimeOffset.UtcNow);
            var newViewModel = CreateViewModel(new List<Message> { mockMessage });
            strategyMock.ClearReceivedCalls();

            newViewModel.HandleOptionClickCommand.Execute(null);

            strategyMock.DidNotReceiveWithAnyArgs().ProcessIncomingUserMessageAndDetermineNextDecisionTreeNodeAsync(default, default);
        }

        [TestMethod]
        public async Task HandleOptionClick_ValidOption_SendsMessage()
        {
            var mockMessage = new Message(1, testUser, testChat, "Init", DateTimeOffset.UtcNow);
            var mockViewModel = CreateViewModel(new List<Message> { mockMessage });
            messageRepositoryMock.ClearReceivedCalls();
            var nextNode = new FAQNode(2, "Next Question", ImmutableArray<FAQOption>.Empty, false);
            var selectedChatOption = new FAQOption("Test", nextNode);
            faqRepositoryMock.GetNodeByIdAsync(2).Returns(Task.FromResult(nextNode));

            mockViewModel.HandleOptionClickCommand.Execute(selectedChatOption);
            await Task.Delay(100);

            await messageRepositoryMock.Received(2).CreateNewEntityAsync(Arg.Any<Message>());
        }

        [TestMethod]
        public void LoadChatHistory_OutgoingMessage_SetsIsOutgoingTrue()
        {
            var mockMessage = new Message(1, testUser, testChat, "Test", DateTimeOffset.UtcNow);

            var mockViewModel = CreateViewModel(new List<Message> { mockMessage });

            Assert.IsTrue(mockViewModel.ChatHistory[0].IsOutgoing);
        }

        [TestMethod]
        public void LoadChatHistory_IncomingMessage_SetsIsOutgoingFalse()
        {
            var otherUser = new User(99, "Other", "other@other.com");
            var mockMessage = new Message(1, otherUser, testChat, "Test", DateTimeOffset.UtcNow);

            var mockViewModel = CreateViewModel(new List<Message> { mockMessage });

            Assert.IsFalse(mockViewModel.ChatHistory[0].IsOutgoing);
        }
    }
}
