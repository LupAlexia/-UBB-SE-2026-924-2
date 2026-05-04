using System;
using System.Diagnostics.CodeAnalysis;
using CloudSpritzers1.Src.Model;
using CloudSpritzers1.Src.Model.Chats;
using CloudSpritzers1.Src.Model.Message;
using CloudSpritzers1.Src.Repository.Implementation;

namespace CloudSpritzers1Tests;

[TestClass]
public class ChatTests
{
    [TestMethod]
    public void Constructor_WhenCalled_InitializesPropertiesCorrectly()
    {
        int chatId = 1;
        int userId = 1;
        ChatStatus status = ChatStatus.Active;

        Chat newChat = new Chat(chatId, userId, status);

        var expectedMessageList = new List<IMessage>();
        Assert.AreEqual(chatId, newChat.ChatId);
        Assert.AreEqual(userId, newChat.UserId);
        Assert.AreEqual(status, newChat.Status);
        CollectionAssert.AreEquivalent(expectedMessageList, newChat.Messages);
    }

    [TestMethod]
    public void AddMessage_WithValidMessage_AddsMessage()
    {
        int chatId = 1;
        int userId = 1;
        ChatStatus status = ChatStatus.Active;
        Chat chat = new Chat(chatId, userId, status);
        var messageSender = new User(1, "John Doe", "johndoe.example@com");
        var newMessage = new Message(messageSender, chat, "Hello");

        chat.AddMessage(newMessage);

        Assert.AreEqual(1, chat.MessageCount());
        Assert.AreEqual(newMessage, chat.Messages[0]);
    }

    [TestMethod]
    public void AddMessage_WithNullMessage_ThrowsArgumentNullException()
    {
        int chatId = 1;
        int userId = 1;
        ChatStatus status = ChatStatus.Active;
        Chat chat = new Chat(chatId, userId, status);
        var messageSender = new User(1, "John Doe", "johndoe.example@com");

        Assert.ThrowsExactly<ArgumentNullException>(() => chat.AddMessage(null));
    }

    [TestMethod]
    public void CloseChat_WhenCalled_UpdatesChatStatusToClosed()
    {
        int chatId = 1;
        int userId = 1;
        ChatStatus status = ChatStatus.Active;
        Chat chat = new Chat(chatId, userId, status);

        chat.CloseChat();

        Assert.AreEqual(ChatStatus.Closed, chat.Status);
    }

}
