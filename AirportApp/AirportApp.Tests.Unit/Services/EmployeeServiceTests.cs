using CloudSpritzers1.Src.Model.Employee;
using CloudSpritzers1.Src.Repository;
using CloudSpritzers1.Src.Repository.Interfaces;
using CloudSpritzers1.Src.Service;
using CloudSpritzers1.Src.Service.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudSpritzers1Tests.Src.Service
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
            _employeeRepository.GetAll().Returns(employees);
        }

        [TestMethod()]
        public void GetAllEmployees_WhenCalled_ReturnsAllEntities()
        {
            var resultedEmployees = _employeeService.GetAllEmployees();

            
            Assert.AreEqual(2, resultedEmployees.Count); 
             _employeeRepository.Received(1).GetAll();
        }

        [TestMethod()]
        public void CreateNewEmployee_WithValidData_CallsRepository()
        {
            int identificationNumber = 3;
            string fullName = "Cristi Dan";
            string emailAddress = "cristi@test.com";
            string department = "SECURITY";

            _employeeService.CreateNewEmployee(identificationNumber, fullName, emailAddress, department);

            _employeeRepository.Received(1).CreateNewEntity(Arg.Is<Employee>(employee => employee.EmployeeId == identificationNumber)); 
        }

        [TestMethod()]
        public void ValidateEmployeeIntegrity_WithNullEmployee_ThrowsArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
                _employeeService.ValidateEmployeeIntegrity(null!));
        }

        [TestMethod()]
        public void ValidateEmployeeIntegrity_WithEmptyName_ThrowsArgumentException()
        {
            var invalidEmployee = new Employee(4, "", "test@test.com", EmployeeDepartment.ADMIN);

            var exceptionThrown = Assert.ThrowsExactly<ArgumentException>(() =>
                _employeeService.ValidateEmployeeIntegrity(invalidEmployee));
            StringAssert.Contains("Name cannot be null or empty", exceptionThrown.Message); 
        }

        [TestMethod()]
        public void ValidateEmployeeIntegrity_InvalidDepartment_ThrowsArgumentException()
        {
            
            var employeeToValidate = new Employee(5, "Name", "email@test.com", (EmployeeDepartment)999);

            var exceptionThrown = Assert.ThrowsExactly<ArgumentException>(() =>
                _employeeService.ValidateEmployeeIntegrity(employeeToValidate)); 
            
            StringAssert.Contains("Invalid group", exceptionThrown.Message); 
        }

        [TestMethod()]
        public void DeleteEmployeeById_CallsRepository_IsSuccessful()
        {
            
            _employeeService.DeleteEmployeeById(1);

            _employeeRepository.Received(1).DeleteById(1); 
        }

        [TestMethod()]
        public void ValidateEmployeeIntegrity_ForDuplicateEmployee_ThrowsArgumentException()
        {
            var existingEmployee = new Employee(1, "Andrei Muresan", "andrei@test.com", EmployeeDepartment.ADMIN);
            _employeeRepository.GetAll().Returns(new List<Employee> { existingEmployee }); 

            var exceptionThrown = Assert.ThrowsExactly<ArgumentException>(() =>
                _employeeService.ValidateEmployeeIntegrity(existingEmployee));

            StringAssert.Contains("Employee already exists", exceptionThrown.Message); 
        }

        [TestMethod()]
        public void CreateNewEmployee_WithInvalidDepartmentString_ThrowsArgumentException()
        {
            int identificationNumber = 10;
            string fullName = "Cristi Dan";
            string emailAddress = "cristi@test.com";
            string invalidDepartment = "NON_EXISTENT_DEPT"; 

            Assert.ThrowsExactly<ArgumentException>(() =>
                _employeeService.CreateNewEmployee(identificationNumber, fullName, emailAddress, invalidDepartment));
        }

        [TestMethod()]
        public void GetEmployeeById_WithExistingId_ReturnsEmployeeFromRepository()
        {
            var expectedEmployee = new Employee(1, "Test Name", "test@test.com", EmployeeDepartment.ADMIN);
            _employeeRepository.GetById(1).Returns(expectedEmployee);

            var resultedEmployee = _employeeService.GetEmployeeById(1);

            Assert.AreEqual(expectedEmployee, resultedEmployee);
            _employeeRepository.Received(1).GetById(1);
        }

        [TestMethod()]
        public void UpdateEmployeeById_CallsRepositoryWithCorrectData_Succeeds()
        {
            var employeeToUpdate = new Employee(1, "Updated Name", "email@test.com", EmployeeDepartment.HR);

            _employeeService.UpdateEmployeeById(1, employeeToUpdate);

            _employeeRepository.Received(1).UpdateById(1, employeeToUpdate);
        }

        [TestMethod()]
        public void ValidateEmployeeIntegrity_EmptyEmail_ThrowsArgumentException()
        {
            var employeeWithEmptyEmail = new Employee(1, "Name", "", EmployeeDepartment.ADMIN);

            var exceptionThrown = Assert.ThrowsExactly<ArgumentException>(() =>
                _employeeService.ValidateEmployeeIntegrity(employeeWithEmptyEmail));

            StringAssert.Contains("Email cannot be null or empty", exceptionThrown.Message);
        }

        [TestMethod()]
        public void ValidateEmployeeIntegrity_EmptyDepartmentName_ThrowsArgumentException()
        {
            var employeeWithEmptyDept = new Employee(1, "Name", "test@test.com", (EmployeeDepartment)999);

            var exceptionThrown = Assert.ThrowsExactly<ArgumentException>(() =>
                _employeeService.ValidateEmployeeIntegrity(employeeWithEmptyDept));

            StringAssert.Contains("Invalid group", exceptionThrown.Message);
        }

    }
}