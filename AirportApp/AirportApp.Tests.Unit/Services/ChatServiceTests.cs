using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private const int TestUserId = 101;
        private const int TestChatId = 1;
        private const int AssignedChatId = 42;
        private const int NonExistentChatId = 999;
        private const string TestUserName = "Test User";
        private const string TestUserEmail = "test@test.com";
        private const string DatabaseErrorMessage = "Database error";
        private const string ChatNotFoundMessage = "Chat not found";

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
            testUser = new User(TestUserId, TestUserName, TestUserEmail);
        }

        [TestMethod]
        public async Task OpenChat_ValidUser_ReturnsActiveChat()
        {
            mockChatRepository.CreateNewEntityAsync(Arg.Any<Chat>()).Returns(Task.FromResult(TestChatId));

            var result = await chatService.OpenChatAsync(testUser);

            Assert.AreEqual(ChatStatus.Active, result.Status);
        }

        [TestMethod]
        public async Task OpenChat_ValidUser_AssignsReturnedIdToChat()
        {
            mockChatRepository.CreateNewEntityAsync(Arg.Any<Chat>()).Returns(Task.FromResult(AssignedChatId));

            var result = await chatService.OpenChatAsync(testUser);

            Assert.AreEqual(AssignedChatId, result.Id);
        }

        [TestMethod]
        public async Task OpenChat_ValidUser_AssignsCorrectUser()
        {
            mockChatRepository.CreateNewEntityAsync(Arg.Any<Chat>()).Returns(Task.FromResult(TestChatId));

            var result = await chatService.OpenChatAsync(testUser);

            Assert.AreEqual(testUser.Id, result.User.Id);
        }

        [TestMethod]
        public async Task OpenChat_RepositoryThrowsException_PropagatesWithSameMessage()
        {
            mockChatRepository.CreateNewEntityAsync(Arg.Any<Chat>())
                .Returns(callInfo => Task.FromException<int>(new Exception(DatabaseErrorMessage)));

            var ex = await Assert.ThrowsExceptionAsync<Exception>(() => chatService.OpenChatAsync(testUser));

            Assert.AreEqual(DatabaseErrorMessage, ex.Message);
        }

        [TestMethod]
        public async Task CloseChat_ExistingChat_UpdatesStatusToClosed()
        {
            var chat = new Chat(TestChatId, testUser, ChatStatus.Active);
            mockChatRepository.GetByIdAsync(TestChatId).Returns(Task.FromResult(chat));

            await chatService.CloseChatAsync(TestChatId);

            await mockChatRepository.Received(1).UpdateByIdAsync(TestChatId,
                Arg.Is<Chat>(chat => chat.Status == ChatStatus.Closed));
        }

        [TestMethod]
        public async Task CloseChat_RepositoryThrowsException_PropagatesWithSameMessage()
        {
            mockChatRepository.GetByIdAsync(NonExistentChatId)
                .Returns(callInfo => Task.FromException<Chat>(new KeyNotFoundException(ChatNotFoundMessage)));

            var ex = await Assert.ThrowsExceptionAsync<Exception>(() => chatService.CloseChatAsync(NonExistentChatId));

            Assert.AreEqual(ChatNotFoundMessage, ex.Message);
        }
    }
}