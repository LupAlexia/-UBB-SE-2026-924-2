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
public class EmployeeMappingProfileTests
{
    private IMapper mapper;
    private ILoggerFactory loggerFactory;

    [TestInitialize]
    public void Setup()
    {
        loggerFactory = Substitute.For<ILoggerFactory>();
        var configuration = new MapperConfiguration(mapperConfiguration => cfg.AddProfile<EmployeeMappingProfile>(), loggerFactory);
        mapper = configuration.CreateMapper();
    }

    [TestMethod]
    public void Map_EmployeeToEmployeeDTO_MapsNameCorrectly()
    {
        var employeeEntity = new Employee(1, "Alex", "alex@mail.com", EmployeeDepartment.HR);

        var resultedDataTransferObject = mapper.Map<EmployeeDTO>(employeeEntity);

        Assert.AreEqual(employeeEntity.RetrieveConfiguredDisplayFullNameForBot(), resultedDataTransferObject.name);
    }

    [TestMethod]
    public void Map_EmployeeToEmployeeDTO_MapsEmailCorrectly()
    {
        var employeeEntity = new Employee(1, "Alex", "alex@mail.com", EmployeeDepartment.HR);

        var resultedDataTransferObject = mapper.Map<EmployeeDTO>(employeeEntity);

        Assert.AreEqual(employeeEntity.RetrieveConfiguredEmailAddressForBotContact(), resultedDataTransferObject.email);
    }

    [TestMethod]
    public void Map_EmployeeToEmployeeDTO_ValidConfiguration()
    {
        var configuration = new MapperConfiguration(mapperConfiguration => cfg.AddProfile<EmployeeMappingProfile>(), loggerFactory);
        configuration.AssertConfigurationIsValid();
    }
}
