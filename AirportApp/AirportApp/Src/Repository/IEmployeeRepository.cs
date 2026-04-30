using System.Collections.Generic;
using AirportApp.Src.Model.Employee;

namespace AirportApp.Src.Repository
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