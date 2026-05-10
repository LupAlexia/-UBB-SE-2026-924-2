using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service;
using AirportApp.Src.Service.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Tests.Unit.Src.Service
{
    [TestClass]
    public class EmployeeServiceTests
    {
        private const int Employee1Id = 1;
        private const int Employee2Id = 2;
        private const int NewEmployeeId = 99;
        private const int CreateEmployeeId = 3;
        private const int InvalidDepartmentValue = 999;
        private const string Employee1Name = "Andrei Muresan";
        private const string Employee1Email = "andrei@test.com";
        private const string Employee2Name = "Elena Radu";
        private const string Employee2Email = "elena@test.com";
        private const string NewEmployeeName = "Brand New";
        private const string NewEmployeeEmail = "new@test.com";
        private const string CreateEmployeeName = "Cristi Dan";
        private const string CreateEmployeeEmail = "cristi@test.com";
        private const string ValidDepartmentString = "SECURITY";
        private const string InvalidDepartmentString = "NON_EXISTENT_DEPT";
        private const string EmptyNameEmail = "test@test.com";
        private const string ValidNameForEmailTest = "Name";
        private const string ValidEmailForNameTest = "email@test.com";

        private IEmployeeRepository employeeRepository;
        private IEmployeeService employeeService;

        [TestInitialize]
        public void Setup()
        {
            employeeRepository = Substitute.For<IEmployeeRepository>();
            employeeService = new EmployeeService(employeeRepository);

            var employees = new List<Employee>
            {
                new Employee(Employee1Id, Employee1Name, Employee1Email, EmployeeDepartment.ADMIN),
                new Employee(Employee2Id, Employee2Name, Employee2Email, EmployeeDepartment.HR)
            };
            employeeRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<Employee>)employees));
        }

        [TestMethod]
        public async Task GetAllEmployees_WhenCalled_ReturnsAllEntities()
        {
            var result = await employeeService.GetAllEmployeesAsync();

            Assert.AreEqual(2, result.Count);
            await employeeRepository.Received(1).GetAllAsync();
        }

        [TestMethod]
        public async Task CreateNewEmployee_WithValidData_CallsRepository()
        {
            await employeeService.CreateNewEmployeeAsync(CreateEmployeeId, CreateEmployeeName, CreateEmployeeEmail, ValidDepartmentString);

            await employeeRepository.Received(1).CreateNewEntityAsync(Arg.Is<Employee>(employee => employee.Id == CreateEmployeeId));
        }

        [TestMethod]
        public async Task CreateNewEmployee_WithInvalidDepartmentString_ThrowsArgumentException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => employeeService.CreateNewEmployeeAsync(NewEmployeeId, CreateEmployeeName, CreateEmployeeEmail, InvalidDepartmentString));
        }

        [TestMethod]
        public async Task CreateNewEmployee_WithValidData_DoesNotCallRepositoryBeforeValidation()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => employeeService.CreateNewEmployeeAsync(NewEmployeeId, CreateEmployeeName, CreateEmployeeEmail, InvalidDepartmentString));

            await employeeRepository.DidNotReceive().CreateNewEntityAsync(Arg.Any<Employee>());
        }

        [TestMethod]
        public async Task ValidateEmployeeIntegrity_WithNullEmployee_ThrowsArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => employeeService.ValidateEmployeeIntegrityAsync(null!));
        }

        [TestMethod]
        public async Task ValidateEmployeeIntegrity_WithEmptyName_ThrowsArgumentException()
        {
            var invalidEmployee = new Employee(NewEmployeeId, string.Empty, EmptyNameEmail, EmployeeDepartment.ADMIN);

            var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => employeeService.ValidateEmployeeIntegrityAsync(invalidEmployee));
            StringAssert.Contains("Name cannot be null or empty", ex.Message);
        }

        [TestMethod]
        public async Task ValidateEmployeeIntegrity_InvalidDepartment_ThrowsArgumentException()
        {
            var employee = new Employee(NewEmployeeId, ValidNameForEmailTest, ValidEmailForNameTest, (EmployeeDepartment)InvalidDepartmentValue);

            var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => employeeService.ValidateEmployeeIntegrityAsync(employee));
            StringAssert.Contains("Invalid group", ex.Message);
        }

        [TestMethod]
        public async Task ValidateEmployeeIntegrity_ForDuplicateEmployee_ThrowsArgumentException()
        {
            var existingEmployee = new Employee(Employee1Id, Employee1Name, Employee1Email, EmployeeDepartment.ADMIN);
            employeeRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<Employee>)new List<Employee> { existingEmployee }));

            var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => employeeService.ValidateEmployeeIntegrityAsync(existingEmployee));
            StringAssert.Contains("Employee already exists", ex.Message);
        }

        [TestMethod]
        public async Task ValidateEmployeeIntegrity_EmptyEmail_ThrowsArgumentException()
        {
            var employee = new Employee(NewEmployeeId, ValidNameForEmailTest, string.Empty, EmployeeDepartment.ADMIN);

            var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => employeeService.ValidateEmployeeIntegrityAsync(employee));
            StringAssert.Contains("Email cannot be null or empty", ex.Message);
        }

        [TestMethod]
        public async Task ValidateEmployeeIntegrity_ValidUniqueEmployee_DoesNotThrow()
        {
            var newEmployee = new Employee(NewEmployeeId, NewEmployeeName, NewEmployeeEmail, EmployeeDepartment.HR);

            await employeeService.ValidateEmployeeIntegrityAsync(newEmployee);
        }
    }
}