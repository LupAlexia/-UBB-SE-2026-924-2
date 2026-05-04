using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutoMapper;
using CloudSpritzers1.Src.Dto;
using CloudSpritzers1.Src.Dto.MappingProfiles;
using CloudSpritzers1.Src.Model.Message;
using CloudSpritzers1.Src.Model.Chats;
using CloudSpritzers1.Src.Model;
using System;

namespace CloudSpritzers1Tests.Src.Dto.MappingProfiles
{
    [TestClass]
    public class MessageMappingProfileTests
    {
        private IMapper _autoMapperInstance;
        private User _testUser;
        private Chat _testChat;
        private Message _testMessage;

        [TestInitialize]
        public void Setup()
        {
            var configuration = new MapperConfiguration(mapperConfiguration => mapperConfiguration.AddProfile<MessageMappingProfile>());
            _autoMapperInstance = configuration.CreateMapper();

            _testUser = new User(1, "Alex", "alex@mail.com");
            _testChat = new Chat(10, 1, ChatStatus.Active);
            _testMessage = new Message(_testUser, _testChat, "Hello");
        }

        [TestMethod]
        public void Configuration_IsValid_PassesValidation()
        {
            var configuration = new MapperConfiguration(mapperConfiguration => mapperConfiguration.AddProfile<MessageMappingProfile>());

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

            Assert.AreEqual(_testChat.ChatId, resultMessageDataTransferObject.ChatId);
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