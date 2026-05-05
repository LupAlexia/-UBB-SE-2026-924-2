using AirportApp.Src.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Domain.Chats;
using AirportApp.ClassLibrary.Entity.Domain.Faq.Bot;
using AirportApp.ClassLibrary.Entity.Domain.Message;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service;
using AirportApp.Src.Service.Bot;
using AirportApp.Src.Service.Bot.Strategy;
using AirportApp.Src.Service.Interfaces;
using AirportApp.Src.ViewModel.Chats;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AirportApp.Tests.Unit.Src.ViewModel.Chats
{
    [TestClass]
    public class ChatViewModelTests
    {
        private IRepository<int, Chat> _chatRepositoryMock;
        private IRepository<int, Message> _msgRepositoryMock;
        private IRepository<int, User> _userRepositoryMock;
        private IBotStrategy _strategyMock;
        private IUserService _userService;
        private IMapper _mapper;

        private BotEngineIdentity _botEngine;
        private MessageService _messageService;
        private ChatService _chatService;

        private User _testUser;
        private Chat _testChat;

        [TestInitialize]
        public void Setup()
        {
            _chatRepositoryMock = Substitute.For<IRepository<int, Chat>>();
            _msgRepositoryMock = Substitute.For<IRepository<int, Message>>();
            _userRepositoryMock = Substitute.For<IRepository<int, User>>();
            _strategyMock = Substitute.For<IBotStrategy>();
            _userService = Substitute.For<IUserService>();
            _mapper = Substitute.For<IMapper>();

            _botEngine = new BotEngineIdentity(_strategyMock);
            _messageService = new MessageService(_chatRepositoryMock, _msgRepositoryMock, _botEngine);
            _chatService = new ChatService(_chatRepositoryMock, _userRepositoryMock);

            _testUser = new User(42, "Test User", "test@test.com");
            _testChat = new Chat(1, _testUser, ChatStatus.Active);

            _chatRepositoryMock.CreateNewEntityAsync(Arg.Any<Chat>()).Returns(Task.FromResult(1));
            _chatRepositoryMock.GetByIdAsync(1).Returns(Task.FromResult(_testChat));
            _userRepositoryMock.GetByIdAsync(42).Returns(Task.FromResult(_testUser));

            _mapper.Map<MessageDTO>(Arg.Any<IMessage>()).Returns(callInfo =>
            {
                var messageEntity = (IMessage)callInfo[0];
                var dataTransferObject = new MessageDTO();
                var messageSender = messageEntity.GetSender();
                if (messageSender != null)
                {
                    dataTransferObject.SenderId = messageSender.RetrieveUniqueDatabaseIdentifierForBot();
                }
                return dataTransferObject;
            });

            var defaultBotReply = new BotMessage.BotMessageBuilder(_testUser, _testChat, 2).Build();
            _strategyMock.ProcessIncomingUserMessageAndDetermineNextDecisionTreeNodeAsync(Arg.Any<BotEngineIdentity>(), Arg.Any<IMessage>())
                .Returns(Task.FromResult(defaultBotReply));

            _userService.GetByIdAsync(42).Returns(Task.FromResult(_testUser));
        }

        private ChatViewModel CreateViewModel(List<Message> initialMessages = null)
        {
            if (initialMessages != null)
            {
                _msgRepositoryMock.GetAllAsync().Returns(Task.FromResult((IEnumerable<Message>)initialMessages));
            }
            else
            {
                _msgRepositoryMock.GetAllAsync().Returns(Task.FromResult(Enumerable.Empty<Message>()));
            }

            return new ChatViewModel(_messageService, _chatService, _mapper, _userService, _testUser);
        }

        [TestMethod]
        public async Task Constructor_HistoryIsEmpty_LoadsFirstMessage()
        {
            _strategyMock.ClearReceivedCalls();

            var newViewModel = CreateViewModel(new List<Message>());
            // Give some time for background tasks if any, though constructor starts them
            await Task.Delay(100); 

            await _strategyMock.ReceivedWithAnyArgs(1).ProcessIncomingUserMessageAndDetermineNextDecisionTreeNodeAsync(Arg.Any<BotEngineIdentity>(), Arg.Any<IMessage>());
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

            await _chatRepositoryMock.Received(1).UpdateByIdAsync(1, Arg.Any<Chat>());
        }

        [TestMethod]
        public void HandleOptionClick_NullOption_DoesNothing()
        {
            var mockMessage = new Message(1, _testUser, _testChat, "Init", DateTimeOffset.UtcNow);
            var newViewModel = CreateViewModel(new List<Message> { mockMessage });
            _strategyMock.ClearReceivedCalls();

            newViewModel.HandleOptionClickCommand.Execute(null);

            _strategyMock.DidNotReceiveWithAnyArgs().ProcessIncomingUserMessageAndDetermineNextDecisionTreeNodeAsync(default, default);
        }

        [TestMethod]
        public async Task HandleOptionClick_ValidOption_SendsMessage()
        {
            var mockMessage = new Message(1, _testUser, _testChat, "Init", DateTimeOffset.UtcNow);
            var mockViewModel = CreateViewModel(new List<Message> { mockMessage });
            _msgRepositoryMock.ClearReceivedCalls();
            var selectedChatOption = new FAQOption("Test", 2);

            mockViewModel.HandleOptionClickCommand.Execute(selectedChatOption);
            await Task.Delay(100);

            await _msgRepositoryMock.Received(2).CreateNewEntityAsync(Arg.Any<Message>());
        }

        [TestMethod]
        public void LoadChatHistory_OutgoingMessage_SetsIsOutgoingTrue()
        {
            var mockMessage = new Message(1, _testUser, _testChat, "Test", DateTimeOffset.UtcNow);

            var mockViewModel = CreateViewModel(new List<Message> { mockMessage });

            Assert.IsTrue(mockViewModel.ChatHistory[0].IsOutgoing);
        }

        [TestMethod]
        public void LoadChatHistory_IncomingMessage_SetsIsOutgoingFalse()
        {
            var otherUser = new User(99, "Other", "other@other.com");
            var mockMessage = new Message(1, otherUser, _testChat, "Test", DateTimeOffset.UtcNow);

            var mockViewModel = CreateViewModel(new List<Message> { mockMessage });

            Assert.IsFalse(mockViewModel.ChatHistory[0].IsOutgoing);
        }
    }
}
