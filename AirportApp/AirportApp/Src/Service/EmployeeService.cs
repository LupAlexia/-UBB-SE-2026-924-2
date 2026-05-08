using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Employee;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service.Interfaces;

namespace AirportApp.Src.Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            this.employeeRepository = employeeRepository;
        }

        public async Task<Employee> GetEmployeeByIdAsync(int identificationNumber)
        {
            return await employeeRepository.GetByIdAsync(identificationNumber);
        }

        public async Task<int> AddEmployeeAsync(Employee employeeEntity)
        {
            return await employeeRepository.CreateNewEntityAsync(employeeEntity);
        }

        public async Task UpdateEmployeeByIdAsync(int identificationNumber, Employee employeeEntity)
        {
            await employeeRepository.UpdateByIdAsync(identificationNumber, employeeEntity);
        }

        public async Task DeleteEmployeeByIdAsync(int identificationNumber)
        {
            await employeeRepository.DeleteByIdAsync(identificationNumber);
        }

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            return (await employeeRepository.GetAllAsync()).ToList();
        }

        public async Task CreateNewEmployeeAsync(int identificationNumber, string fullName, string emailAddress, string departmentName)
        {
            EmployeeDepartment departmentEnum = (EmployeeDepartment)Enum.Parse(typeof(EmployeeDepartment), departmentName);
            Employee newEmployee = new Employee(identificationNumber, fullName, emailAddress, departmentEnum);
            await ValidateEmployeeIntegrityAsync(newEmployee);
            await AddEmployeeAsync(newEmployee);
        }

        public async Task ValidateEmployeeIntegrityAsync(Employee employeeEntity)
        {
            ArgumentNullException.ThrowIfNull(employeeEntity);

            if ((await this.GetAllEmployeesAsync()).Contains(employeeEntity))
            {
                throw new ArgumentException("Employee already exists");
            }
            if (string.IsNullOrEmpty(employeeEntity.RetrieveConfiguredDisplayFullNameForBot()))
            {
                throw new ArgumentException("Name cannot be null or empty");
            }
            if (string.IsNullOrEmpty(employeeEntity.RetrieveConfiguredEmailAddressForBotContact()))
            {
                throw new ArgumentException("Email cannot be null or empty");
            }
            if (!Enum.IsDefined(typeof(EmployeeDepartment), employeeEntity.GetDepartmentName()))
            {
                throw new ArgumentException("Invalid group");
            }
        }
    }
}
