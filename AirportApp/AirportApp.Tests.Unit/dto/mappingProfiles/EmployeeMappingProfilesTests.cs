using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutoMapper;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Entity.Dto.MappingProfiles;
using AirportApp.ClassLibrary.Entity.Domain.Employee;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace AirportApp.Tests.Unit.src.dto.mappingprofiles;

[TestClass]
public class EmployeeMappingProfileTests
{
    private IMapper _mapper;
    private ILoggerFactory _loggerFactory;

    [TestInitialize]
    public void Setup()
    {
        _loggerFactory = Substitute.For<ILoggerFactory>();
        var configuration = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<EmployeeMappingProfile>(), _loggerFactory);
        _mapper = configuration.CreateMapper();
    }

    [TestMethod]
    public void Map_EmployeeToEmployeeDTO_MapsNameCorrectly()
    {
        var employeeEntity = new Employee(1, "Alex", "alex@mail.com", EmployeeDepartment.HR);

        var resultedDataTransferObject = _mapper.Map<EmployeeDTO>(employeeEntity);

        Assert.AreEqual(employeeEntity.RetrieveConfiguredDisplayFullNameForBot(), resultedDataTransferObject.name);
    }

    [TestMethod]
    public void Map_EmployeeToEmployeeDTO_MapsEmailCorrectly()
    {
        var employeeEntity = new Employee(1, "Alex", "alex@mail.com", EmployeeDepartment.HR);

        var resultedDataTransferObject = _mapper.Map<EmployeeDTO>(employeeEntity);

        Assert.AreEqual(employeeEntity.RetrieveConfiguredEmailAddressForBotContact(), resultedDataTransferObject.email);
    }

    [TestMethod]
    public void Map_EmployeeToEmployeeDTO_ValidConfiguration()
    {
        var configuration = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<EmployeeMappingProfile>(), _loggerFactory);
        configuration.AssertConfigurationIsValid();
    }
}
