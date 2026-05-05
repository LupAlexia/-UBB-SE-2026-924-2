using AirportApp.Src.ViewModel;
using AirportApp.ClassLibrary.Entity.Domain.Employee;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service;
using AirportApp.Src.Service.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirportApp.Tests.Unit.Src.Service
{
    [TestClass()]
    public class EmployeeServiceTests
    {
        private IEmployeeRepository _employeeRepository;
        private IEmployeeService _employeeService;

        [TestInitialize]
        public void Setup()
        {
            _employeeRepository = Substitute.For<IEmployeeRepository>();
            _employeeService = new EmployeeService(_employeeRepository);

            var employees = new List<Employee>
            {
                new Employee(1, "Andrei Muresan", "andrei@test.com", EmployeeDepartment.ADMIN),
                new Employee(2, "Elena Radu", "elena@test.com", EmployeeDepartment.HR)
            };
            _employeeRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<Employee>)employees));
        }

        [TestMethod()]
        public async Task GetAllEmployees_WhenCalled_ReturnsAllEntities()
        {
            var resultedEmployees = await _employeeService.GetAllEmployeesAsync();

            Assert.AreEqual(2, resultedEmployees.Count); 
            await _employeeRepository.Received(1).GetAllAsync();
        }

        [TestMethod()]
        public async Task CreateNewEmployee_WithValidData_CallsRepository()
        {
            int identificationNumber = 3;
            string fullName = "Cristi Dan";
            string emailAddress = "cristi@test.com";
            string department = "SECURITY";

            await _employeeService.CreateNewEmployeeAsync(identificationNumber, fullName, emailAddress, department);

            await _employeeRepository.Received(1).CreateNewEntityAsync(Arg.Is<Employee>(employee => employee.Id == identificationNumber)); 
        }

        [TestMethod()]
        public async Task ValidateEmployeeIntegrity_WithNullEmployee_ThrowsArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
                await _employeeService.ValidateEmployeeIntegrityAsync(null!));
        }

        [TestMethod()]
        public async Task ValidateEmployeeIntegrity_WithEmptyName_ThrowsArgumentException()
        {
            var invalidEmployee = new Employee(4, "", "test@test.com", EmployeeDepartment.ADMIN);

            var exceptionThrown = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await _employeeService.ValidateEmployeeIntegrityAsync(invalidEmployee));
            StringAssert.Contains("Name cannot be null or empty", exceptionThrown.Message); 
        }

        [TestMethod()]
        public async Task ValidateEmployeeIntegrity_InvalidDepartment_ThrowsArgumentException()
        {
            var employeeToValidate = new Employee(5, "Name", "email@test.com", (EmployeeDepartment)999);

            var exceptionThrown = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await _employeeService.ValidateEmployeeIntegrityAsync(employeeToValidate)); 
            
            StringAssert.Contains("Invalid group", exceptionThrown.Message); 
        }

        [TestMethod()]
        public async Task DeleteEmployeeById_CallsRepository_IsSuccessful()
        {
            await _employeeService.DeleteEmployeeByIdAsync(1);

            await _employeeRepository.Received(1).DeleteByIdAsync(1); 
        }

        [TestMethod()]
        public async Task ValidateEmployeeIntegrity_ForDuplicateEmployee_ThrowsArgumentException()
        {
            var existingEmployee = new Employee(1, "Andrei Muresan", "andrei@test.com", EmployeeDepartment.ADMIN);
            _employeeRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<Employee>)new List<Employee> { existingEmployee })); 

            var exceptionThrown = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await _employeeService.ValidateEmployeeIntegrityAsync(existingEmployee));

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
                await _employeeService.CreateNewEmployeeAsync(identificationNumber, fullName, emailAddress, invalidDepartment));
        }

        [TestMethod()]
        public async Task GetEmployeeById_WithExistingId_ReturnsEmployeeFromRepository()
        {
            var expectedEmployee = new Employee(1, "Test Name", "test@test.com", EmployeeDepartment.ADMIN);
            _employeeRepository.GetByIdAsync(1).Returns(Task.FromResult(expectedEmployee));

            var resultedEmployee = await _employeeService.GetEmployeeByIdAsync(1);

            Assert.AreEqual(expectedEmployee, resultedEmployee);
            await _employeeRepository.Received(1).GetByIdAsync(1);
        }

        [TestMethod()]
        public async Task UpdateEmployeeById_CallsRepositoryWithCorrectData_Succeeds()
        {
            var employeeToUpdate = new Employee(1, "Updated Name", "email@test.com", EmployeeDepartment.HR);

            await _employeeService.UpdateEmployeeByIdAsync(1, employeeToUpdate);

            await _employeeRepository.Received(1).UpdateByIdAsync(1, employeeToUpdate);
        }

        [TestMethod()]
        public async Task ValidateEmployeeIntegrity_EmptyEmail_ThrowsArgumentException()
        {
            var employeeWithEmptyEmail = new Employee(1, "Name", "", EmployeeDepartment.ADMIN);

            var exceptionThrown = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await _employeeService.ValidateEmployeeIntegrityAsync(employeeWithEmptyEmail));

            StringAssert.Contains("Email cannot be null or empty", exceptionThrown.Message);
        }

        [TestMethod()]
        public async Task ValidateEmployeeIntegrity_EmptyDepartmentName_ThrowsArgumentException()
        {
            var employeeWithEmptyDept = new Employee(1, "Name", "test@test.com", (EmployeeDepartment)999);

            var exceptionThrown = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await _employeeService.ValidateEmployeeIntegrityAsync(employeeWithEmptyDept));

            StringAssert.Contains("Invalid group", exceptionThrown.Message);
        }
    }
}
