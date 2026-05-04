using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutoMapper;
using CloudSpritzers1.Src.Dto;
using CloudSpritzers1.Src.Dto.MappingProfiles;
using CloudSpritzers1.Src.Model;

namespace CloudSpritzers1Tests.src.dto.mappingprofiles;

[TestClass]
public class UserMappingProfileTests
{
    private IMapper _mapper;

    [TestInitialize]
    public void Setup()
    {
        var configuration = new MapperConfiguration(mapperConfiguration => mapperConfiguration.AddProfile<UserMappingProfile>());

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
        var configuration = new MapperConfiguration(mapperConfiguration => mapperConfiguration.AddProfile<UserMappingProfile>());

        configuration.AssertConfigurationIsValid();
    }
}