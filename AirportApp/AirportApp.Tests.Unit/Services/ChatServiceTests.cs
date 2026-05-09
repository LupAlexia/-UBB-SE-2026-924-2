using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.Src.ViewModel;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.Src.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AirportApp.Tests.Unit.Src.Service
{
    [TestClass]
    public class ChatServiceTests
    {
        private IRepository<int, Chat> mockChatRepository = null!;
        private IRepository<int, User> mockUserRepository = null!;
        private ChatService chatService = null!;
        private User testUser;

        [TestInitialize]
        public void Setup()
        {
            mockChatRepository = Substitute.For<IRepository<int, Chat>>();
            mockUserRepository = Substitute.For<IRepository<int, User>>();
            chatService = new ChatService(mockChatRepository, mockUserRepository);
            testUser = new User(101, "Test User", "test@test.com");
            mockUserRepository.GetByIdAsync(101).Returns(Task.FromResult(testUser));
        }

        [TestMethod]
        public async Task OpenChat_ValidUserId_ReturnsChatWithCorrectId()
        {
            mockChatRepository.CreateNewEntityAsync(Arg.Any<Chat>()).Returns(Task.FromResult(5));

            var resultChat = await chatService.OpenChatAsync(testUser);

            Assert.AreEqual(5, resultChat.Id);
        }

        [TestMethod]
        public async Task OpenChat_ValidUserId_ReturnsChatWithCorrectUserId()
        {
            mockChatRepository.CreateNewEntityAsync(Arg.Any<Chat>()).Returns(Task.FromResult(5));

            var resultedChat = await chatService.OpenChatAsync(testUser);

            Assert.AreEqual(101, resultedChat.User.Id);
        }

        [TestMethod]
        public async Task OpenChat_ValidUserId_ReturnsChatWithActiveStatus()
        {
            mockChatRepository.CreateNewEntityAsync(Arg.Any<Chat>()).Returns(Task.FromResult(5));

            var resultedChat = await chatService.OpenChatAsync(testUser);

            Assert.AreEqual(ChatStatus.Active, resultedChat.Status);
        }

        [TestMethod]
        public async Task OpenChat_RepositoryThrowsException_ThrowsException()
        {
            mockChatRepository.CreateNewEntityAsync(Arg.Any<Chat>()).Returns(x => Task.FromException<int>(new Exception("Database error")));

            await Assert.ThrowsExceptionAsync<Exception>(async () => await chatService.OpenChatAsync(testUser));
        }

        [TestMethod]
        public async Task OpenChat_RepositoryThrowsException_ThrowsCorrectErrorMessage()
        {
            mockChatRepository.CreateNewEntityAsync(Arg.Any<Chat>()).Returns(x => Task.FromException<int>(new Exception("Database error")));

            var exceptionThrown = await Assert.ThrowsExceptionAsync<Exception>(async () => await chatService.OpenChatAsync(testUser));

            Assert.AreEqual("Database error", exceptionThrown.Message);
        }

        [TestMethod]
        public async Task CloseChat_ExistingChat_CallsRepositoryUpdateWithClosedStatus()
        {
            var exampleChat = new Chat(1, testUser, ChatStatus.Active);
            mockChatRepository.GetByIdAsync(1).Returns(Task.FromResult(exampleChat));

            await chatService.CloseChatAsync(1);

            await mockChatRepository.Received(1).UpdateByIdAsync(1, Arg.Is<Chat>(updatedChatEntity => updatedChatEntity.Status == ChatStatus.Closed));
        }

        [TestMethod]
        public async Task CloseChat_RepositoryThrowsException_ThrowsException()
        {
            mockChatRepository.GetByIdAsync(999).Returns(x => Task.FromException<Chat>(new KeyNotFoundException("Chat not found")));

            await Assert.ThrowsExceptionAsync<Exception>(async () => await chatService.CloseChatAsync(999));
        }

        [TestMethod]
        public async Task CloseChat_RepositoryThrowsException_ThrowsCorrectErrorMessage()
        {
            mockChatRepository.GetByIdAsync(999).Returns(x => Task.FromException<Chat>(new KeyNotFoundException("Chat not found")));

            var exceptionThrown = await Assert.ThrowsExceptionAsync<Exception>(async () => await chatService.CloseChatAsync(999));

            Assert.AreEqual("Chat not found", exceptionThrown.Message);
        }
    }
}
