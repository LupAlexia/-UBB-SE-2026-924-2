using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutoMapper;
using CloudSpritzers1.Src.Dto;
using CloudSpritzers1.Src.Dto.MappingProfiles;
using CloudSpritzers1.Src.Model.Employee;

namespace CloudSpritzers1Tests.src.dto.mappingprofiles;

[TestClass]
public class EmployeeMappingProfileTests
{
    private IMapper _mapper;

    [TestInitialize]
    public void Setup()
    {
        var configuration = new MapperConfiguration(mapperConfiguration => mapperConfiguration.AddProfile<EmployeeMappingProfile>());

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
        var configuration = new MapperConfiguration(mapperConfiguration => mapperConfiguration.AddProfile<EmployeeMappingProfile>());

        configuration.AssertConfigurationIsValid();
    }
}