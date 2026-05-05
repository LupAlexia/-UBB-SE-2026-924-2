using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutoMapper;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Entity.Dto.MappingProfiles;
using AirportApp.ClassLibrary.Entity.Domain.Chats;
using AirportApp.ClassLibrary.Entity.Domain;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace AirportApp.Tests.Unit.src.dto.mappingprofiles;

[TestClass]
public class ChatMappingProfileTests
{
    private IMapper _mapper;
    private Chat _chat;
    private User _testUser;
    private ILoggerFactory _loggerFactory;

    [TestInitialize]
    public void Setup()
    {
        _loggerFactory = Substitute.For<ILoggerFactory>();
        var configuration = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<ChatMappingProfile>(), _loggerFactory);

        _mapper = configuration.CreateMapper();
        _testUser = new User(10, "Test User", "test@test.com");
        _chat = new Chat(1, _testUser, ChatStatus.Active);
    }

    [TestMethod]
    public void Map_ChatToChatDTO_MapsChatIdCorrectly()
    {
        var resultedDataTransferObject = _mapper.Map<ChatDTO>(_chat);

        Assert.AreEqual(_chat.Id, resultedDataTransferObject.chatId);
    }

    [TestMethod]
    public void Map_ChatToChatDTO_MapsUserIdCorrectly()
    {
        var resultedDataTransferObject = _mapper.Map<ChatDTO>(_chat);

        Assert.AreEqual(_chat.UserId, resultedDataTransferObject.userId);
    }

    [TestMethod]
    public void Map_ChatToChatDTO_MapsStatusCorrectly()
    {
        var resultedDataTransferObject = _mapper.Map<ChatDTO>(_chat);

        Assert.AreEqual(_chat.Status, resultedDataTransferObject.status);
    }

    [TestMethod]
    public void Map_ChatToChatDTO_MapsMessageCountCorrectly()
    {
        var resultedDataTransferObject = _mapper.Map<ChatDTO>(_chat);   

        Assert.AreEqual(0, resultedDataTransferObject.messageCount); 
    }

    [TestMethod]
    public void Map_ChatToChatDTO_ValidConfiguration()
    {
        var configuration = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<ChatMappingProfile>(), _loggerFactory);

        configuration.AssertConfigurationIsValid();
    }
}
