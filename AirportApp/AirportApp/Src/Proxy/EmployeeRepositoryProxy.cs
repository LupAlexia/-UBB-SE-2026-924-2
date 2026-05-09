using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Employee;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.Src.Proxy
{
    public class EmployeeRepositoryProxy : IEmployeeRepository
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/employee";

        public EmployeeRepositoryProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await httpClient.GetFromJsonAsync<IEnumerable<Employee>>(BaseUrl)
                   ?? new List<Employee>();
        }

        public async Task<Employee> GetByIdAsync(int identificationNumber)
        {
            return await httpClient.GetFromJsonAsync<Employee>($"{BaseUrl}/{identificationNumber}")
                   ?? throw new KeyNotFoundException($"Employee with id {identificationNumber} was not found.");
        }

        public async Task<int> CreateNewEntityAsync(Employee employeeEntity)
        {
            var response = await httpClient.PostAsJsonAsync(BaseUrl, employeeEntity);
            response.EnsureSuccessStatusCode();
            var created = await response.Content.ReadFromJsonAsync<Employee>();
            return created!.Id;
        }

        public async Task UpdateByIdAsync(int identificationNumber, Employee employeeEntity)
        {
            var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{identificationNumber}", employeeEntity);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteByIdAsync(int identificationNumber)
        {
            var response = await httpClient.DeleteAsync($"{BaseUrl}/{identificationNumber}");
            response.EnsureSuccessStatusCode();
        }
    }
}