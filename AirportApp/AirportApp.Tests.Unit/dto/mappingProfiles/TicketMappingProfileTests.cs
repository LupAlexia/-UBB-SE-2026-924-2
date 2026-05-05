using AutoMapper;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Entity.Dto.MappingProfiles;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;

namespace AirportApp.Tests.Unit.Src.Dto.MappingProfiles
{
    [TestClass]
    public class TicketMappingProfileTests
    {
        private IMapper _mapper;
        private User _testUser;
        private TicketCategory _testCategory;
        private TicketSubcategory _testSubcategory;
        private Ticket _testTicket;
        private ILoggerFactory _loggerFactory;

        [TestInitialize]
        public void Setup()
        {
            _loggerFactory = Substitute.For<ILoggerFactory>();
            var configuration = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<TicketMappingProfile>(), _loggerFactory);
            _mapper = configuration.CreateMapper();

            _testUser = new User(101, "Jane Doe", "jane@example.com");
            _testCategory = new TicketCategory(1, "Billing", TicketUrgencyLevelEnum.HIGH);
            _testSubcategory = new TicketSubcategory(10, "Refund", 99, _testCategory);

            _testTicket = new Ticket(
                5,
                _testUser,
                TicketStatusEnum.OPEN,
                _testCategory,
                _testSubcategory,
                "Refund Request",
                "I want a refund for my delayed flight.",
                new DateTime(2026, 1, 1),
                TicketUrgencyLevelEnum.HIGH
            );
        }

        [TestMethod]
        public void Map_ValidTicket_ReturnsNotNullObject()
        {
            var resultDataTransferObject = _mapper.Map<TicketDTO>(_testTicket);

            Assert.IsNotNull(resultDataTransferObject);
        }

        [TestMethod]
        public void Map_ValidTicket_MapsTicketIdCorrectly()
        {
            var resultDataTransferObject = _mapper.Map<TicketDTO>(_testTicket);

            Assert.AreEqual(_testTicket.Id, resultDataTransferObject.ticketId);
        }

        [TestMethod]
        public void Map_ValidTicket_MapsCreatorAccountIdCorrectly()
        {
            var resultDataTransferObject = _mapper.Map<TicketDTO>(_testTicket);

            Assert.AreEqual(_testTicket.Creator.Id, resultDataTransferObject.creatorAccountId);
        }

        [TestMethod]
        public void Map_ValidTicket_MapsCreatorEmailAddressCorrectly()
        {
            var resultDataTransferObject = _mapper.Map<TicketDTO>(_testTicket);

            Assert.AreEqual(_testTicket.Creator.RetrieveConfiguredEmailAddressForBotContact(), resultDataTransferObject.creatorEmailAddress);
        }

        [TestMethod]
        public void Map_ValidTicket_MapsUrgencyLevelCorrectly()
        {
            var resultDataTransferObject = _mapper.Map<TicketDTO>(_testTicket);

            Assert.AreEqual(_testTicket.UrgencyLevel, resultDataTransferObject.urgencyLevel);
        }

        [TestMethod]
        public void Map_ValidTicket_MapsCurrentStatusCorrectly()
        {
            var resultDataTransferObject = _mapper.Map<TicketDTO>(_testTicket);

            Assert.AreEqual(_testTicket.CurrentStatus, resultDataTransferObject.currentStatus);
        }

        [TestMethod]
        public void Map_ValidTicket_MapsCategoryIdCorrectly()
        {
            var resultDataTransferObject = _mapper.Map<TicketDTO>(_testTicket);

            Assert.AreEqual(_testTicket.Category.Id, resultDataTransferObject.categoryId);
        }

        [TestMethod]
        public void Map_ValidTicket_MapsCategoryNameCorrectly()
        {
            var resultDataTransferObject = _mapper.Map<TicketDTO>(_testTicket);

            Assert.AreEqual(_testTicket.Category.CategoryName, resultDataTransferObject.categoryName);
        }

        [TestMethod]
        public void Map_ValidTicket_MapsSubcategoryIdCorrectly()
        {
            var resultDataTransferObject = _mapper.Map<TicketDTO>(_testTicket);

            Assert.AreEqual(_testTicket.Subcategory.Id, resultDataTransferObject.subcategoryId);
        }

        [TestMethod]
        public void Map_ValidTicket_MapsSubcategoryNameCorrectly()
        {
            var resultDataTransferObject = _mapper.Map<TicketDTO>(_testTicket);

            Assert.AreEqual(_testTicket.Subcategory.SubcategoryName, resultDataTransferObject.subcategoryName);
        }

        [TestMethod]
        public void Map_ValidTicket_MapsSubjectCorrectly()
        {
            var resultDataTransferObject = _mapper.Map<TicketDTO>(_testTicket);

            Assert.AreEqual(_testTicket.Subject, resultDataTransferObject.subject);
        }

        [TestMethod]
        public void Map_ValidTicket_MapsDescriptionCorrectly()
        {
            var resultDataTransferObject = _mapper.Map<TicketDTO>(_testTicket);

            Assert.AreEqual(_testTicket.Description, resultDataTransferObject.description);
        }

        [TestMethod]
        public void Map_ValidTicket_MapsCreationTimestampCorrectly()
        {
            var resultDataTransferObject = _mapper.Map<TicketDTO>(_testTicket);

            Assert.AreEqual(_testTicket.CreationTimestamp, resultDataTransferObject.creationTimestamp);
        }
    }
}
