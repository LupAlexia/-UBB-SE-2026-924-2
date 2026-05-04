using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Employee;

namespace AirportApp.ClassLibrary.Repository.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<int> CreateNewEntityAsync(Employee employeeEntity);

        Task DeleteByIdAsync(int identificationNumber);

        Task<IEnumerable<Employee>> GetAllAsync();

        Task<Employee> GetByIdAsync(int identificationNumber);

        Task UpdateByIdAsync(int identificationNumber, Employee employeeEntity);
    }
}