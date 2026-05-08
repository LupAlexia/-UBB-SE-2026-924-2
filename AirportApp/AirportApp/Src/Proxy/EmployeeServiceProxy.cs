using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Employee;
using AirportApp.Src.Service;

namespace AirportApp.Src.Proxy
{
    public class EmployeeServiceProxy : IEmployeeService 
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/employee";

        public EmployeeServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<Employee> GetEmployeeByIdAsync(int identificationNumber)
        {
            return await httpClient.GetFromJsonAsync<Employee>($"{BaseUrl}/{identificationNumber}");
        }

        public async Task<int> AddEmployeeAsync(Employee employeeEntity)
        {
            var response = await httpClient.PostAsJsonAsync(BaseUrl, employeeEntity);
            response.EnsureSuccessStatusCode();
            return employeeEntity.Id;
        }

        public async Task UpdateEmployeeByIdAsync(int identificationNumber, Employee employeeEntity)
        {
            var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{identificationNumber}", employeeEntity);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteEmployeeByIdAsync(int identificationNumber)
        {
            var response = await httpClient.DeleteAsync($"{BaseUrl}/{identificationNumber}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            return await httpClient.GetFromJsonAsync<List<Employee>>(BaseUrl);
        }

        public async Task CreateNewEmployeeAsync(int identificationNumber, string fullName, string emailAddress, string departmentName)
        {
            // logica ramane locala
            EmployeeDepartment departmentEnum = (EmployeeDepartment)Enum.Parse(typeof(EmployeeDepartment), departmentName);
            Employee newEmployee = new Employee(identificationNumber, fullName, emailAddress, departmentEnum);
            await ValidateEmployeeIntegrityAsync(newEmployee);
            await AddEmployeeAsync(newEmployee);
        }

        public async Task ValidateEmployeeIntegrityAsync(Employee employeeEntity)
        {
            // logica ramane locala, dar GetAllEmployeesAsync face call la API
            ArgumentNullException.ThrowIfNull(employeeEntity);

            if ((await GetAllEmployeesAsync()).Contains(employeeEntity))
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