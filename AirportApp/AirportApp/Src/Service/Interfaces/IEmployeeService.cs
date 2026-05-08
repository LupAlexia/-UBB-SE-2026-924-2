using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Employee;

namespace AirportApp.Src.Service.Interfaces
{
    public interface IEmployeeService
    {
        Task<Employee> GetEmployeeByIdAsync(int identificationNumber);
        Task<int> AddEmployeeAsync(Employee employeeEntity);
        Task UpdateEmployeeByIdAsync(int identificationNumber, Employee employeeEntity);
        Task DeleteEmployeeByIdAsync(int identificationNumber);
        Task<List<Employee>> GetAllEmployeesAsync();
        Task CreateNewEmployeeAsync(int identificationNumber, string fullName, string emailAddress, string departmentName);
        Task ValidateEmployeeIntegrityAsync(Employee employeeEntity);
    }
}