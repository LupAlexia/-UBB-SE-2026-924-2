using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutoMapper;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Entity.Dto.MappingProfiles;
using AirportApp.ClassLibrary.Entity.Domain.Message;
using AirportApp.ClassLibrary.Entity.Domain.Chats;
using AirportApp.ClassLibrary.Entity.Domain;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace AirportApp.Tests.Unit.Src.Dto.MappingProfiles
{
    [TestClass]
    public class MessageMappingProfileTests
    {
        private IMapper autoMapperInstance;
        private User testUser;
        private Chat testChat;
        private Message testMessage;
        private ILoggerFactory loggerFactory;

        [TestInitialize]
        public void Setup()
        {
            loggerFactory = Substitute.For<ILoggerFactory>();
            var configuration = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<MessageMappingProfile>(), loggerFactory);
            autoMapperInstance = configuration.CreateMapper();
            testUser = new User(1, "Alex", "alex@mail.com");
            testChat = new Chat(10, testUser, ChatStatus.Active);
            testMessage = new Message(testUser, testChat, "Hello");
            testMessage.Sender = testUser; // Add this line
        }

        [TestMethod]
        public void Configuration_IsValid_PassesValidation()
        {
            var configuration = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<MessageMappingProfile>(), loggerFactory);

            configuration.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void Map_ValidMessage_MapsMessageTextCorrectly()
        {
            var resultMessageDataTransferObject = autoMapperInstance.Map<MessageDTO>(testMessage);

            Assert.AreEqual("Hello", resultMessageDataTransferObject.MessageText);
        }

        [TestMethod]
        public void Map_ValidMessage_MapsChatIdentifierCorrectly()
        {
            var resultMessageDataTransferObject = autoMapperInstance.Map<MessageDTO>(testMessage);

            Assert.AreEqual(testChat.Id, resultMessageDataTransferObject.ChatId);
        }

        [TestMethod]
        public void Map_ValidMessage_MapsSenderIdentifierCorrectly()
        {
            var resultMessageDataTransferObject = autoMapperInstance.Map<MessageDTO>(testMessage);

            Assert.AreEqual(testUser.UserId, resultMessageDataTransferObject.Sender.RetrieveUniqueDatabaseIdentifierForBot());
        }

        [TestMethod]
        public void Map_ValidMessage_MapsSenderNameCorrectly()
        {
            var resultMessageDataTransferObject = autoMapperInstance.Map<MessageDTO>(testMessage);

            Assert.AreEqual(testUser.RetrieveConfiguredDisplayFullNameForBot(), resultMessageDataTransferObject.Sender.RetrieveConfiguredDisplayFullNameForBot());
        }

        [TestMethod]
        public void Map_ValidMessage_MapsUninitializedTimestampCorrectly()
        {
            var resultMessageDataTransferObject = autoMapperInstance.Map<MessageDTO>(testMessage);

            Assert.IsTrue(resultMessageDataTransferObject.Timestamp != default);
        }

        [TestMethod]
        public void Map_ValidMessage_MapsFrequentlyAskedQuestionsOptionsCorrectly()
        {
            var resultMessageDataTransferObject = autoMapperInstance.Map<MessageDTO>(testMessage);

            Assert.IsNotNull(resultMessageDataTransferObject.FaqOptions);
        }
    }
}
