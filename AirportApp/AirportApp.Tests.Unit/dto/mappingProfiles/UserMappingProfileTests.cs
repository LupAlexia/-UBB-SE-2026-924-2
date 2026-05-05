using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutoMapper;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Entity.Dto.MappingProfiles;
using AirportApp.ClassLibrary.Entity.Domain;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace AirportApp.Tests.Unit.src.dto.mappingprofiles;

[TestClass]
public class UserMappingProfileTests
{
    private IMapper _mapper;
    private ILoggerFactory _loggerFactory;

    [TestInitialize]
    public void Setup()
    {
        _loggerFactory = Substitute.For<ILoggerFactory>();
        var configuration = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<UserMappingProfile>(), _loggerFactory);

        _mapper = configuration.CreateMapper();
    }

    [TestMethod]
    public void Map_UserToUserDTO_MapsUserNameCorrectly()
    {
        var userEntity = new User(1, "Alex", "alex@mail.com");

        var resultedDataTransferObject = _mapper.Map<UserDTO>(userEntity);

        Assert.AreEqual(userEntity.RetrieveConfiguredDisplayFullNameForBot(), resultedDataTransferObject.name);
    }

    [TestMethod]
    public void Map_UserToUserDTO_MapsUserEmailCorrectly()
    {
        var userEntity = new User(1, "Alex", "alex@mail.com");

        var resultedDataTransferObject = _mapper.Map<UserDTO>(userEntity);

        Assert.AreEqual(userEntity.RetrieveConfiguredEmailAddressForBotContact(), resultedDataTransferObject.email);
    }

    [TestMethod]
    public void Map_UserToUserDTO_ValidConfiguration()
    {
        var configuration = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<UserMappingProfile>(), _loggerFactory);

        configuration.AssertConfigurationIsValid();
    }
}
