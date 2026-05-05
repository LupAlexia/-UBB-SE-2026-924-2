using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutoMapper;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Entity.Dto.MappingProfiles;
using AirportApp.ClassLibrary.Entity.Domain.Message;
using AirportApp.ClassLibrary.Entity.Domain.Chats;
using AirportApp.ClassLibrary.Entity.Domain;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;

namespace AirportApp.Tests.Unit.Src.Dto.MappingProfiles
{
    [TestClass]
    public class MessageMappingProfileTests
    {
        private IMapper _autoMapperInstance;
        private User _testUser;
        private Chat _testChat;
        private Message _testMessage;
        private ILoggerFactory _loggerFactory;

        [TestInitialize]
        public void Setup()
        {
            _loggerFactory = Substitute.For<ILoggerFactory>();
            var configuration = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<MessageMappingProfile>(), _loggerFactory);
            _autoMapperInstance = configuration.CreateMapper();

            _testUser = new User(1, "Alex", "alex@mail.com");
            _testChat = new Chat(10, _testUser, ChatStatus.Active);
            _testMessage = new Message(_testUser, _testChat, "Hello");
        }

        [TestMethod]
        public void Configuration_IsValid_PassesValidation()
        {
            var configuration = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<MessageMappingProfile>(), _loggerFactory);

            configuration.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void Map_ValidMessage_MapsMessageTextCorrectly()
        {
            var resultMessageDataTransferObject = _autoMapperInstance.Map<MessageDTO>(_testMessage);

            Assert.AreEqual("Hello", resultMessageDataTransferObject.MessageText);
        }

        [TestMethod]
        public void Map_ValidMessage_MapsChatIdentifierCorrectly()
        {
            var resultMessageDataTransferObject = _autoMapperInstance.Map<MessageDTO>(_testMessage);

            Assert.AreEqual(_testChat.Id, resultMessageDataTransferObject.ChatId);
        }

        [TestMethod]
        public void Map_ValidMessage_MapsSenderIdentifierCorrectly()
        {
            var resultMessageDataTransferObject = _autoMapperInstance.Map<MessageDTO>(_testMessage);

            Assert.AreEqual(_testUser.RetrieveUniqueDatabaseIdentifierForBot(), resultMessageDataTransferObject.SenderId);
        }

        [TestMethod]
        public void Map_ValidMessage_MapsSenderNameCorrectly()
        {
            var resultMessageDataTransferObject = _autoMapperInstance.Map<MessageDTO>(_testMessage);

            Assert.AreEqual(_testUser.RetrieveConfiguredDisplayFullNameForBot(), resultMessageDataTransferObject.SenderName);
        }

        [TestMethod]
        public void Map_ValidMessage_MapsUninitializedTimestampCorrectly()
        {
            var resultMessageDataTransferObject = _autoMapperInstance.Map<MessageDTO>(_testMessage);

            Assert.IsTrue(resultMessageDataTransferObject.Timestamp != default);
        }

        [TestMethod]
        public void Map_ValidMessage_MapsFrequentlyAskedQuestionsOptionsCorrectly()
        {
            var resultMessageDataTransferObject = _autoMapperInstance.Map<MessageDTO>(_testMessage);

            Assert.IsNotNull(resultMessageDataTransferObject.FaqOptions);
        }
    }
}
