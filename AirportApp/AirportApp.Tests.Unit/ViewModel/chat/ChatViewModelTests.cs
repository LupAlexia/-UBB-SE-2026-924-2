using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AutoMapper;
using CloudSpritzers1.Src.Dto;
using CloudSpritzers1.Src.Model;
using CloudSpritzers1.Src.Model.Chats;
using CloudSpritzers1.Src.Model.Faq.Bot;
using CloudSpritzers1.Src.Model.Message;
using CloudSpritzers1.Src.Repository;
using CloudSpritzers1.Src.Service;
using CloudSpritzers1.Src.Service.Bot;
using CloudSpritzers1.Src.Service.Bot.Strategy;
using CloudSpritzers1.Src.Service.Interfaces;
using CloudSpritzers1.Src.ViewModel.Chats;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace CloudSpritzers1Tests.Src.ViewModel.Chats
{
    [TestClass]
    public class ChatViewModelTests
    {
        // mock Repositories
        private IRepository<int, Chat> _chatRepositoryMock;
        private IRepository<int, Message> _msgRepositoryMock;
        private IBotStrategy _strategyMock;
        private IUserService _userService;
        private IMapper _mapper;

        // real Services using mocked Repositories
        private BotEngine _botEngine;
        private MessageService _messageService;
        private ChatService _chatService;

        private User _testUser;
        private Chat _testChat;

        [TestInitialize]
        public void Setup()
        {
            _chatRepositoryMock = Substitute.For<IRepository<int, Chat>>();
            _msgRepositoryMock = Substitute.For<IRepository<int, Message>>();
            _strategyMock = Substitute.For<IBotStrategy>();
            _userService = Substitute.For<IUserService>();
            _mapper = Substitute.For<IMapper>();

            _botEngine = new BotEngine(_strategyMock);
            _messageService = new MessageService(_chatRepositoryMock, _msgRepositoryMock, _botEngine);
            _chatService = new ChatService(_chatRepositoryMock);

            _testUser = new User(42, "Test User", "test@test.com");
            _testChat = new Chat(1, 42, ChatStatus.Active);

            _chatRepositoryMock.CreateNewEntity(Arg.Any<Chat>()).Returns(1);
            _chatRepositoryMock.GetById(1).Returns(_testChat);

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
            _strategyMock.ProcessIncomingUserMessageAndDetermineNextDecisionTreeNode(Arg.Any<BotEngine>(), Arg.Any<IMessage>())
                .Returns(defaultBotReply);

            _userService.GetById(42).Returns(_testUser);
        }

        private ChatViewModel CreateViewModel(List<Message> initialMessages = null)
        {
            if (initialMessages != null)
            {
                _msgRepositoryMock.GetAll().Returns(initialMessages);
            }
            else
            {
                _msgRepositoryMock.GetAll().Returns(new List<Message>());
            }

            return new ChatViewModel(_messageService, _chatService, _mapper, _userService, _testUser);
        }

        [TestMethod]
        public void Constructor_HistoryIsEmpty_LoadsFirstMessage()
        {
            _strategyMock.ClearReceivedCalls();

            var newViewModel = CreateViewModel(new List<Message>());

            _strategyMock.ReceivedWithAnyArgs(1).ProcessIncomingUserMessageAndDetermineNextDecisionTreeNode(default, default);
        }

        [TestMethod]
        public void Constructor_HistoryIsNotEmpty_DoesNotLoadFirstMessage()
        {
            var mockMsg = new Message(1, _testUser, _testChat, "Test", DateTimeOffset.UtcNow);
            _strategyMock.ClearReceivedCalls();

            var newViewModel = CreateViewModel(new List<Message> { mockMsg });

            _strategyMock.DidNotReceiveWithAnyArgs().ProcessIncomingUserMessageAndDetermineNextDecisionTreeNode(default, default);
        }

        [TestMethod]
        public void FormatUserId_WhenCalled_ReturnsCorrectlyFormattedString()
        {
            var newViewModel = CreateViewModel();

            Assert.AreEqual("User Id: 42", newViewModel.FormatUserId);
        }

        [TestMethod]
        public void CloseChat_WhenCalled_CallsChatRepositoryUpdate()
        {
            var newViewModel = CreateViewModel();

            newViewModel.CloseChat();

            _chatRepositoryMock.Received(1).UpdateById(1, Arg.Any<Chat>());
        }

        [TestMethod]
        public void HandleOptionClick_NullOption_DoesNothing()
        {
            var mockMessage = new Message(1, _testUser, _testChat, "Init", DateTimeOffset.UtcNow);
            var newViewModel = CreateViewModel(new List<Message> { mockMessage });
            _strategyMock.ClearReceivedCalls();

            newViewModel.HandleOptionClickCommand.Execute(null);

            _strategyMock.DidNotReceiveWithAnyArgs().ProcessIncomingUserMessageAndDetermineNextDecisionTreeNode(default, default);
        }

        [TestMethod]
        public void HandleOptionClick_ValidOption_SendsMessage()
        {
            var mockMessage = new Message(1, _testUser, _testChat, "Init", DateTimeOffset.UtcNow);
            var mockViewModel = CreateViewModel(new List<Message> { mockMessage });
            _msgRepositoryMock.ClearReceivedCalls();
            var selectedChatOption = new FAQOption("Test", 2);

            mockViewModel.HandleOptionClickCommand.Execute(selectedChatOption);

            _msgRepositoryMock.Received(2).CreateNewEntity(Arg.Any<Message>());
        }

        [TestMethod]
        public void HandleOptionClick_NextOptionsAvailable_AddsToCurrentOptions()
        {
            var mockMessage = new Message(1, _testUser, _testChat, "Init", DateTimeOffset.UtcNow);
            var mockViewModel = CreateViewModel(new List<Message> { mockMessage });
            var selectedChatOption = new FAQOption("Test", 2);
            var nextOption = new FAQOption("Next", 3);
            var botReply = new BotMessage.BotMessageBuilder(_testUser, _testChat, 2)
                .AddOption(nextOption)
                .Build();
            _strategyMock.ProcessIncomingUserMessageAndDetermineNextDecisionTreeNode(Arg.Any<BotEngine>(), Arg.Any<IMessage>()).Returns(botReply);
            
            mockViewModel.HandleOptionClickCommand.Execute(selectedChatOption);

            Assert.AreEqual(nextOption, mockViewModel.CurrentOptions[0]);
        }

        [TestMethod]
        public void HandleOptionClick_NextOptionsNull_AddsRestartChatOption()
        {
            var mockMessage = new Message(1, _testUser, _testChat, "Init", DateTimeOffset.UtcNow);
            var mockViewModel = CreateViewModel(new List<Message> { mockMessage });
            var selectedChatOption = new FAQOption("Test", 2);
            var botReply = new BotMessage.BotMessageBuilder(_testUser, _testChat, 2).Build();
            typeof(BotMessage).GetField("faqOptions", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(botReply, null);
            _strategyMock.ProcessIncomingUserMessageAndDetermineNextDecisionTreeNode(Arg.Any<BotEngine>(), Arg.Any<IMessage>()).Returns(botReply);

            mockViewModel.HandleOptionClickCommand.Execute(selectedChatOption);

            Assert.AreEqual("Restart Chat", mockViewModel.CurrentOptions[0].label);
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

        [TestMethod]
        public void LoadChatHistory_SetsSenderName_Correctly()
        {
            var otherUser = new User(99, "Other Name", "other@other.com");
            var mockMessage = new Message(1, otherUser, _testChat, "Test", DateTimeOffset.UtcNow);
            _userService.GetById(99).Returns(otherUser);

            var mockViewModel = CreateViewModel(new List<Message> { mockMessage });

            Assert.AreEqual("Other Name", mockViewModel.ChatHistory[0].SenderName);
        }
    }
}