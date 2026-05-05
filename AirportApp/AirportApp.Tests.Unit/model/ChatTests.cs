using System;
using System.Collections.Generic;
using System.Linq;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Domain.Chats;
using AirportApp.ClassLibrary.Entity.Domain.Message;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AirportApp.Tests.Unit;

[TestClass]
public class ChatTests
{
    private User _testUser;

    [TestInitialize]
    public void Setup()
    {
        _testUser = new User(1, "John Doe", "johndoe.example@com");
    }

    [TestMethod]
    public void Constructor_WhenCalled_InitializesPropertiesCorrectly()
    {
        int chatId = 1;
        ChatStatus status = ChatStatus.Active;

        Chat newChat = new Chat(chatId, _testUser, status);

        Assert.AreEqual(chatId, newChat.Id);
        Assert.AreEqual(_testUser.UserId, newChat.UserId);
        Assert.AreEqual(status, newChat.Status);
        Assert.AreEqual(0, newChat.Messages.Count);
    }

    [TestMethod]
    public void AddMessage_WithValidMessage_AddsMessage()
    {
        Chat chat = new Chat(1, _testUser, ChatStatus.Active);
        var newMessage = new Message(1, _testUser, chat, "Hello", DateTimeOffset.UtcNow);

        chat.AddMessage(newMessage);

        Assert.AreEqual(1, chat.MessageCount());
        Assert.AreEqual(newMessage, chat.Messages.First());
    }

    [TestMethod]
    public void AddMessage_WithNullMessage_ThrowsArgumentNullException()
    {
        Chat chat = new Chat(1, _testUser, ChatStatus.Active);

        Assert.ThrowsException<ArgumentNullException>(() => chat.AddMessage(null!));
    }

    [TestMethod]
    public void CloseChat_WhenCalled_UpdatesChatStatusToClosed()
    {
        Chat chat = new Chat(1, _testUser, ChatStatus.Active);

        chat.CloseChat();

        Assert.AreEqual(ChatStatus.Closed, chat.Status);
    }
}
