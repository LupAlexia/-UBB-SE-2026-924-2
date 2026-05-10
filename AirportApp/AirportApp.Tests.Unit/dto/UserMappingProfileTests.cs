using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutoMapper;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Entity.Domain;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace AirportApp.Tests.Unit.dto;

[TestClass]
public class UserMappingProfileTests
{
    private IMapper mapper;
    private ILoggerFactory loggerFactory;

    [TestInitialize]
    public void Setup()
    {
        loggerFactory = Substitute.For<ILoggerFactory>();
        var configuration = new MapperConfiguration(mapperConfiguration => cfg.AddProfile<UserMappingProfile>(), loggerFactory);

        mapper = configuration.CreateMapper();
    }

    [TestMethod]
    public void Map_UserToUserDTO_MapsUserNameCorrectly()
    {
        var userEntity = new User(1, "Alex", "alex@mail.com");

        var resultedDataTransferObject = mapper.Map<UserDTO>(userEntity);

        Assert.AreEqual(userEntity.RetrieveConfiguredDisplayFullNameForBot(), resultedDataTransferObject.name);
    }

    [TestMethod]
    public void Map_UserToUserDTO_MapsUserEmailCorrectly()
    {
        var userEntity = new User(1, "Alex", "alex@mail.com");

        var resultedDataTransferObject = mapper.Map<UserDTO>(userEntity);

        Assert.AreEqual(userEntity.RetrieveConfiguredEmailAddressForBotContact(), resultedDataTransferObject.email);
    }

    [TestMethod]
    public void Map_UserToUserDTO_ValidConfiguration()
    {
        var configuration = new MapperConfiguration(mapperConfiguration => cfg.AddProfile<UserMappingProfile>(), loggerFactory);

        configuration.AssertConfigurationIsValid();
    }
}
