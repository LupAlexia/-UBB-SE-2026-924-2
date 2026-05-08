using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutoMapper;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Entity.Domain;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace AirportApp.Tests.Unit.src.dto.mappingprofiles;

[TestClass]
public class ChatMappingProfileTests
{
    private IMapper mapper;
    private Chat chat;
    private User testUser;
    private ILoggerFactory loggerFactory;

    [TestInitialize]
    public void Setup()
    {
        loggerFactory = Substitute.For<ILoggerFactory>();
        var configuration = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<ChatMappingProfile>(), loggerFactory);

        mapper = configuration.CreateMapper();
        testUser = new User(10, "Test User", "test@test.com");
        chat = new Chat(1, testUser, ChatStatus.Active);
    }

    [TestMethod]
    public void Map_ChatToChatDTO_MapsChatIdCorrectly()
    {
        var resultedDataTransferObject = mapper.Map<ChatDTO>(chat);

        Assert.AreEqual(chat.Id, resultedDataTransferObject.chatId);
    }

    [TestMethod]
    public void Map_ChatToChatDTO_MapsUserIdCorrectly()
    {
        var resultedDataTransferObject = mapper.Map<ChatDTO>(chat);

        Assert.AreEqual(chat.UserId, resultedDataTransferObject.userId);
    }

    [TestMethod]
    public void Map_ChatToChatDTO_MapsStatusCorrectly()
    {
        var resultedDataTransferObject = mapper.Map<ChatDTO>(chat);

        Assert.AreEqual(chat.Status, resultedDataTransferObject.status);
    }

    [TestMethod]
    public void Map_ChatToChatDTO_MapsMessageCountCorrectly()
    {
        var resultedDataTransferObject = mapper.Map<ChatDTO>(chat);
        Assert.AreEqual(0, resultedDataTransferObject.messageCount);
    }

    [TestMethod]
    public void Map_ChatToChatDTO_ValidConfiguration()
    {
        var configuration = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<ChatMappingProfile>(), loggerFactory);

        configuration.AssertConfigurationIsValid();
    }
}
