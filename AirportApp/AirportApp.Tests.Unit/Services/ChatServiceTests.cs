using CloudSpritzers1.Src.Model.Chats;
using CloudSpritzers1.Src.Repository;
using CloudSpritzers1.Src.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;

namespace CloudSpritzers1Tests.Src.Service
{
    [TestClass]
    public class ChatServiceTests
    {
        private IRepository<int, Chat> _mockChatRepository = null!;
        private ChatService _chatService = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockChatRepository = Substitute.For<IRepository<int, Chat>>();
            _chatService = new ChatService(_mockChatRepository);
        }

        [TestMethod]
        public void OpenChat_ValidUserId_ReturnsChatWithCorrectId()
        {
            _mockChatRepository.CreateNewEntity(Arg.Any<Chat>()).Returns(5);

            var resultChat = _chatService.OpenChat(101);

            Assert.AreEqual(5, resultChat.ChatId);
        }

        [TestMethod]
        public void OpenChat_ValidUserId_ReturnsChatWithCorrectUserId()
        {
            _mockChatRepository.CreateNewEntity(Arg.Any<Chat>()).Returns(5);

            var resultedChat = _chatService.OpenChat(101);

            Assert.AreEqual(101, resultedChat.UserId);
        }

        [TestMethod]
        public void OpenChat_ValidUserId_ReturnsChatWithActiveStatus()
        {
            _mockChatRepository.CreateNewEntity(Arg.Any<Chat>()).Returns(5);

            var resultedChat = _chatService.OpenChat(101);

            Assert.AreEqual(ChatStatus.Active, resultedChat.Status);
        }

        [TestMethod]
        public void OpenChat_RepositoryThrowsException_ThrowsException()
        {
            _mockChatRepository.CreateNewEntity(Arg.Any<Chat>()).Returns(capturedArguments => throw new Exception("Database error"));

            Assert.ThrowsExactly<Exception>(() => _chatService.OpenChat(101));
        }

        [TestMethod]
        public void OpenChat_RepositoryThrowsException_ThrowsCorrectErrorMessage()
        {
            _mockChatRepository.CreateNewEntity(Arg.Any<Chat>()).Returns(capturedArguments => throw new Exception("Database error"));

            var exceptionThrown = Assert.ThrowsExactly<Exception>(() => _chatService.OpenChat(101));

            Assert.AreEqual("Database error", exceptionThrown.Message);
        }

        [TestMethod]
        public void CloseChat_ExistingChat_CallsRepositoryUpdateWithClosedStatus()
        {
            var exampleChat = new Chat(1, 101, ChatStatus.Active);
            _mockChatRepository.GetById(1).Returns(exampleChat);

            _chatService.CloseChat(1);

            _mockChatRepository.Received(1).UpdateById(1, Arg.Is<Chat>(updatedChatEntity => updatedChatEntity.Status == ChatStatus.Closed));
        }

        [TestMethod]
        public void CloseChat_RepositoryThrowsException_ThrowsException()
        {
            _mockChatRepository.GetById(999).Returns(capturedArguments => throw new KeyNotFoundException("Chat not found"));

            Assert.ThrowsExactly<Exception>(() => _chatService.CloseChat(999));
        }

        [TestMethod]
        public void CloseChat_RepositoryThrowsException_ThrowsCorrectErrorMessage()
        {
            _mockChatRepository.GetById(999).Returns(capturedArguments => throw new KeyNotFoundException("Chat not found"));

            var exceptionThrown = Assert.ThrowsExactly<Exception>(() => _chatService.CloseChat(999));

            Assert.AreEqual("Chat not found", exceptionThrown.Message);
        }
    }
}