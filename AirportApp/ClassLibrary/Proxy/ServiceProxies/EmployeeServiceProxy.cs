using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interfaces;

namespace AirportApp.ClassLibrary.Proxy.ServiceProxies
{
    public class EmployeeServiceProxy : IEmployeeService
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/employee";

        public EmployeeServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<Employee> GetEmployeeByIdAsync(int identificationNumber)
        {
            try
            {
                Employee employee = await httpClient.GetFromJsonAsync<Employee>($"{BaseUrl}/{identificationNumber}");
                if (employee == null)
                {
                    throw new KeyNotFoundException($"Employee with id {identificationNumber} was not found.");
                }

                return employee;
            }
            catch (HttpRequestException httpRequestException) when (httpRequestException.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new KeyNotFoundException($"Employee with id {identificationNumber} was not found.");
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Server communication error while retrieving employee {identificationNumber}.", httpRequestException);
            }
        }

        public async Task<int> AddEmployeeAsync(Employee employeeEntity)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PostAsJsonAsync(BaseUrl, employeeEntity);
                response.EnsureSuccessStatusCode();

                Employee createdEmployee = await response.Content.ReadFromJsonAsync<Employee>();
                if (createdEmployee != null)
                {
                    return createdEmployee.Id;
                }

                return employeeEntity.Id;
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException("Failed to add employee through the service proxy.", httpRequestException);
            }
        }

        public async Task UpdateEmployeeByIdAsync(int identificationNumber, Employee employeeEntity)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{identificationNumber}", employeeEntity);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to update employee {identificationNumber}.", httpRequestException);
            }
        }

        public async Task DeleteEmployeeByIdAsync(int identificationNumber)
        {
            try
            {
                HttpResponseMessage response = await httpClient.DeleteAsync($"{BaseUrl}/{identificationNumber}");
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to delete employee {identificationNumber}.", httpRequestException);
            }
        }

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            try
            {
                List<Employee> employees = await httpClient.GetFromJsonAsync<List<Employee>>(BaseUrl);
                return employees ?? new List<Employee>();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException("Failed to retrieve all employees.", httpRequestException);
            }
        }

        public Task CreateNewEmployeeAsync(int identificationNumber, string fullName, string emailAddress, string departmentName)
        {
            throw new NotSupportedException("CreateNewEmployeeAsync is not available through the service proxy.");
        }

        public Task ValidateEmployeeIntegrityAsync(Employee employeeEntity)
        {
            throw new NotSupportedException("ValidateEmployeeIntegrityAsync is not available through the service proxy.");
        }
    }
}
