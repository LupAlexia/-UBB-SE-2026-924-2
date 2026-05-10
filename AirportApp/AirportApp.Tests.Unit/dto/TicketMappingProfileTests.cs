using System;
using AutoMapper;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Entity.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace AirportApp.Tests.Unit
{
    [TestClass]
    public class TicketMappingProfileTests
    {
        private IMapper mapper;
        private User testUser;
        private ComplaintTicketCategory testCategory;
        private ComplaintTicketSubcategory testSubcategory;
        private ComplaintTicket testTicket;
        private ILoggerFactory loggerFactory;

        [TestInitialize]
        public void Setup()
        {
            loggerFactory = Substitute.For<ILoggerFactory>();
            var configuration = new MapperConfiguration(mapperConfiguration => cfg.AddProfile<TicketMappingProfile>(), loggerFactory);
            mapper = configuration.CreateMapper();

            testUser = new User(101, "Jane Doe", "jane@example.com");
            testCategory = new ComplaintTicketCategory(1, "Billing", ComplaintTicketUrgencyLevelEnum.HIGH);
            testSubcategory = new ComplaintTicketSubcategory(10, "Refund", 99, testCategory);

            testTicket = new ComplaintTicket(
                5,
                testUser,
                ComplaintTicketStatusEnum.OPEN,
                testCategory,
                testSubcategory,
                "Refund Request",
                "I want a refund for my delayed flight.",
                new DateTime(2026, 1, 1),
                ComplaintTicketUrgencyLevelEnum.HIGH);
        }

        [TestMethod]
        public void Map_ValidTicket_ReturnsNotNullObject()
        {
            var resultDataTransferObject = mapper.Map<TicketDTO>(testTicket);

            Assert.IsNotNull(resultDataTransferObject);
        }

        [TestMethod]
        public void Map_ValidTicket_MapsTicketIdCorrectly()
        {
            var resultDataTransferObject = mapper.Map<TicketDTO>(testTicket);

            Assert.AreEqual(testTicket.Id, resultDataTransferObject.ticketId);
        }

        [TestMethod]
        public void Map_ValidTicket_MapsCreatorAccountIdCorrectly()
        {
            var resultDataTransferObject = mapper.Map<TicketDTO>(testTicket);

            Assert.AreEqual(testTicket.Creator.Id, resultDataTransferObject.creatorAccountId);
        }

        [TestMethod]
        public void Map_ValidTicket_MapsCreatorEmailAddressCorrectly()
        {
            var resultDataTransferObject = mapper.Map<TicketDTO>(testTicket);

            Assert.AreEqual(testTicket.Creator.RetrieveConfiguredEmailAddressForBotContact(), resultDataTransferObject.creatorEmailAddress);
        }

        [TestMethod]
        public void Map_ValidTicket_MapsUrgencyLevelCorrectly()
        {
            var resultDataTransferObject = mapper.Map<TicketDTO>(testTicket);

            Assert.AreEqual(testTicket.UrgencyLevel, resultDataTransferObject.urgencyLevel);
        }

        [TestMethod]
        public void Map_ValidTicket_MapsCurrentStatusCorrectly()
        {
            var resultDataTransferObject = mapper.Map<TicketDTO>(testTicket);

            Assert.AreEqual(testTicket.CurrentStatus, resultDataTransferObject.currentStatus);
        }

        [TestMethod]
        public void Map_ValidTicket_MapsCategoryIdCorrectly()
        {
            var resultDataTransferObject = mapper.Map<TicketDTO>(testTicket);

            Assert.AreEqual(testTicket.Category.Id, resultDataTransferObject.categoryId);
        }

        [TestMethod]
        public void Map_ValidTicket_MapsCategoryNameCorrectly()
        {
            var resultDataTransferObject = mapper.Map<TicketDTO>(testTicket);

            Assert.AreEqual(testTicket.Category.CategoryName, resultDataTransferObject.categoryName);
        }

        [TestMethod]
        public void Map_ValidTicket_MapsSubcategoryIdCorrectly()
        {
            var resultDataTransferObject = mapper.Map<TicketDTO>(testTicket);

            Assert.AreEqual(testTicket.Subcategory.Id, resultDataTransferObject.subcategoryId);
        }

        [TestMethod]
        public void Map_ValidTicket_MapsSubcategoryNameCorrectly()
        {
            var resultDataTransferObject = mapper.Map<TicketDTO>(testTicket);

            Assert.AreEqual(testTicket.Subcategory.SubcategoryName, resultDataTransferObject.subcategoryName);
        }

        [TestMethod]
        public void Map_ValidTicket_MapsSubjectCorrectly()
        {
            var resultDataTransferObject = mapper.Map<TicketDTO>(testTicket);

            Assert.AreEqual(testTicket.Subject, resultDataTransferObject.subject);
        }

        [TestMethod]
        public void Map_ValidTicket_MapsDescriptionCorrectly()
        {
            var resultDataTransferObject = mapper.Map<TicketDTO>(testTicket);

            Assert.AreEqual(testTicket.Description, resultDataTransferObject.description);
        }

        [TestMethod]
        public void Map_ValidTicket_MapsCreationTimestampCorrectly()
        {
            var resultDataTransferObject = mapper.Map<TicketDTO>(testTicket);

            Assert.AreEqual(testTicket.CreationTimestamp, resultDataTransferObject.creationTimestamp);
        }
    }
}
