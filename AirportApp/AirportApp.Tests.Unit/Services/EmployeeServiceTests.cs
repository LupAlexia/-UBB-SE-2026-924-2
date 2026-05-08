using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.Src.ViewModel;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service;
using AirportApp.Src.Service.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Tests.Unit.Src.Service
{
    [TestClass()]
    public class EmployeeServiceTests
    {
        private IEmployeeRepository employeeRepository;
        private IEmployeeService employeeService;

        [TestInitialize]
        public void Setup()
        {
            employeeRepository = Substitute.For<IEmployeeRepository>();
            employeeService = new EmployeeService(employeeRepository);

            var employees = new List<Employee>
            {
                new Employee(1, "Andrei Muresan", "andrei@test.com", EmployeeDepartment.ADMIN),
                new Employee(2, "Elena Radu", "elena@test.com", EmployeeDepartment.HR)
            };
            employeeRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<Employee>)employees));
        }

        [TestMethod()]
        public async Task GetAllEmployees_WhenCalled_ReturnsAllEntities()
        {
            var resultedEmployees = await employeeService.GetAllEmployeesAsync();

            Assert.AreEqual(2, resultedEmployees.Count);
            await employeeRepository.Received(1).GetAllAsync();
        }

        [TestMethod()]
        public async Task CreateNewEmployee_WithValidData_CallsRepository()
        {
            int identificationNumber = 3;
            string fullName = "Cristi Dan";
            string emailAddress = "cristi@test.com";
            string department = "SECURITY";

            await employeeService.CreateNewEmployeeAsync(identificationNumber, fullName, emailAddress, department);

            await employeeRepository.Received(1).CreateNewEntityAsync(Arg.Is<Employee>(employee => employee.Id == identificationNumber));
        }

        [TestMethod()]
        public async Task ValidateEmployeeIntegrity_WithNullEmployee_ThrowsArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
                await employeeService.ValidateEmployeeIntegrityAsync(null!));
        }

        [TestMethod()]
        public async Task ValidateEmployeeIntegrity_WithEmptyName_ThrowsArgumentException()
        {
            var invalidEmployee = new Employee(4, string.Empty, "test@test.com", EmployeeDepartment.ADMIN);

            var exceptionThrown = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await employeeService.ValidateEmployeeIntegrityAsync(invalidEmployee));
            StringAssert.Contains("Name cannot be null or empty", exceptionThrown.Message);
        }

        [TestMethod()]
        public async Task ValidateEmployeeIntegrity_InvalidDepartment_ThrowsArgumentException()
        {
            var employeeToValidate = new Employee(5, "Name", "email@test.com", (EmployeeDepartment)999);

            var exceptionThrown = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await employeeService.ValidateEmployeeIntegrityAsync(employeeToValidate));

            StringAssert.Contains("Invalid group", exceptionThrown.Message);
        }

        [TestMethod()]
        public async Task DeleteEmployeeById_CallsRepository_IsSuccessful()
        {
            await employeeService.DeleteEmployeeByIdAsync(1);

            await employeeRepository.Received(1).DeleteByIdAsync(1);
        }

        [TestMethod()]
        public async Task ValidateEmployeeIntegrity_ForDuplicateEmployee_ThrowsArgumentException()
        {
            var existingEmployee = new Employee(1, "Andrei Muresan", "andrei@test.com", EmployeeDepartment.ADMIN);
            employeeRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<Employee>)new List<Employee> { existingEmployee }));

            var exceptionThrown = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await employeeService.ValidateEmployeeIntegrityAsync(existingEmployee));

            StringAssert.Contains("Employee already exists", exceptionThrown.Message);
        }

        [TestMethod()]
        public async Task CreateNewEmployee_WithInvalidDepartmentString_ThrowsArgumentException()
        {
            int identificationNumber = 10;
            string fullName = "Cristi Dan";
            string emailAddress = "cristi@test.com";
            string invalidDepartment = "NON_EXISTENT_DEPT";

            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await employeeService.CreateNewEmployeeAsync(identificationNumber, fullName, emailAddress, invalidDepartment));
        }

        [TestMethod()]
        public async Task GetEmployeeById_WithExistingId_ReturnsEmployeeFromRepository()
        {
            var expectedEmployee = new Employee(1, "Test Name", "test@test.com", EmployeeDepartment.ADMIN);
            employeeRepository.GetByIdAsync(1).Returns(Task.FromResult(expectedEmployee));

            var resultedEmployee = await employeeService.GetEmployeeByIdAsync(1);

            Assert.AreEqual(expectedEmployee, resultedEmployee);
            await employeeRepository.Received(1).GetByIdAsync(1);
        }

        [TestMethod()]
        public async Task UpdateEmployeeById_CallsRepositoryWithCorrectData_Succeeds()
        {
            var employeeToUpdate = new Employee(1, "Updated Name", "email@test.com", EmployeeDepartment.HR);

            await employeeService.UpdateEmployeeByIdAsync(1, employeeToUpdate);

            await employeeRepository.Received(1).UpdateByIdAsync(1, employeeToUpdate);
        }

        [TestMethod()]
        public async Task ValidateEmployeeIntegrity_EmptyEmail_ThrowsArgumentException()
        {
            var employeeWithEmptyEmail = new Employee(1, "Name", string.Empty, EmployeeDepartment.ADMIN);

            var exceptionThrown = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await employeeService.ValidateEmployeeIntegrityAsync(employeeWithEmptyEmail));

            StringAssert.Contains("Email cannot be null or empty", exceptionThrown.Message);
        }

        [TestMethod()]
        public async Task ValidateEmployeeIntegrity_EmptyDepartmentName_ThrowsArgumentException()
        {
            var employeeWithEmptyDept = new Employee(1, "Name", "test@test.com", (EmployeeDepartment)999);

            var exceptionThrown = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await employeeService.ValidateEmployeeIntegrityAsync(employeeWithEmptyDept));

            StringAssert.Contains("Invalid group", exceptionThrown.Message);
        }
    }
}
