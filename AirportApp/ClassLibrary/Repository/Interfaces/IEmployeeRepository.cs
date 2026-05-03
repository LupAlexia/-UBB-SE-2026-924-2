using System.Collections.Generic;
using AirportApp.ClassLibrary.Entity.Domain.Employee;

namespace AirportApp.ClassLibrary.Repository.Interfaces
{
    public interface IEmployeeRepository
    {
        int CreateNewEntity(Employee employeeEntity);
        void DeleteById(int identificationNumber);
        IEnumerable<Employee> GetAll();
        Employee GetById(int identificationNumber);
        void UpdateById(int identificationNumber, Employee employeeEntity);
    }
}