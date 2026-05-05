using AirportApp.Src.ViewModel;
using AirportApp.ClassLibrary.Entity.Domain.Chats;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.Src.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AirportApp.Tests.Unit.Src.Service
{
    [TestClass]
    public class ChatServiceTests
    {
        private IRepository<int, Chat> _mockChatRepository = null!;
        private IRepository<int, User> _mockUserRepository = null!;
        private ChatService _chatService = null!;
        private User _testUser;

        [TestInitialize]
        public void Setup()
        {
            _mockChatRepository = Substitute.For<IRepository<int, Chat>>();
            _mockUserRepository = Substitute.For<IRepository<int, User>>();
            _chatService = new ChatService(_mockChatRepository, _mockUserRepository);
            _testUser = new User(101, "Test User", "test@test.com");
            _mockUserRepository.GetByIdAsync(101).Returns(Task.FromResult(_testUser));
        }

        [TestMethod]
        public async Task OpenChat_ValidUserId_ReturnsChatWithCorrectId()
        {
            _mockChatRepository.CreateNewEntityAsync(Arg.Any<Chat>()).Returns(Task.FromResult(5));

            var resultChat = await _chatService.OpenChatAsync(101);

            Assert.AreEqual(5, resultChat.Id);
        }

        [TestMethod]
        public async Task OpenChat_ValidUserId_ReturnsChatWithCorrectUserId()
        {
            _mockChatRepository.CreateNewEntityAsync(Arg.Any<Chat>()).Returns(Task.FromResult(5));

            var resultedChat = await _chatService.OpenChatAsync(101);

            Assert.AreEqual(101, resultedChat.UserId);
        }

        [TestMethod]
        public async Task OpenChat_ValidUserId_ReturnsChatWithActiveStatus()
        {
            _mockChatRepository.CreateNewEntityAsync(Arg.Any<Chat>()).Returns(Task.FromResult(5));

            var resultedChat = await _chatService.OpenChatAsync(101);

            Assert.AreEqual(ChatStatus.Active, resultedChat.Status);
        }

        [TestMethod]
        public async Task OpenChat_RepositoryThrowsException_ThrowsException()
        {
            _mockChatRepository.CreateNewEntityAsync(Arg.Any<Chat>()).Returns(x => Task.FromException<int>(new Exception("Database error")));

            await Assert.ThrowsExceptionAsync<Exception>(async () => await _chatService.OpenChatAsync(101));
        }

        [TestMethod]
        public async Task OpenChat_RepositoryThrowsException_ThrowsCorrectErrorMessage()
        {
            _mockChatRepository.CreateNewEntityAsync(Arg.Any<Chat>()).Returns(x => Task.FromException<int>(new Exception("Database error")));

            var exceptionThrown = await Assert.ThrowsExceptionAsync<Exception>(async () => await _chatService.OpenChatAsync(101));

            Assert.AreEqual("Database error", exceptionThrown.Message);
        }

        [TestMethod]
        public async Task CloseChat_ExistingChat_CallsRepositoryUpdateWithClosedStatus()
        {
            var exampleChat = new Chat(1, _testUser, ChatStatus.Active);
            _mockChatRepository.GetByIdAsync(1).Returns(Task.FromResult(exampleChat));

            await _chatService.CloseChatAsync(1);

            await _mockChatRepository.Received(1).UpdateByIdAsync(1, Arg.Is<Chat>(updatedChatEntity => updatedChatEntity.Status == ChatStatus.Closed));
        }

        [TestMethod]
        public async Task CloseChat_RepositoryThrowsException_ThrowsException()
        {
            _mockChatRepository.GetByIdAsync(999).Returns(x => Task.FromException<Chat>(new KeyNotFoundException("Chat not found")));

            await Assert.ThrowsExceptionAsync<Exception>(async () => await _chatService.CloseChatAsync(999));
        }

        [TestMethod]
        public async Task CloseChat_RepositoryThrowsException_ThrowsCorrectErrorMessage()
        {
            _mockChatRepository.GetByIdAsync(999).Returns(x => Task.FromException<Chat>(new KeyNotFoundException("Chat not found")));

            var exceptionThrown = await Assert.ThrowsExceptionAsync<Exception>(async () => await _chatService.CloseChatAsync(999));

            Assert.AreEqual("Chat not found", exceptionThrown.Message);
        }
    }
}
