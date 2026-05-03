using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.Entity.Domain.Employee;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.DataAccess;

namespace AirportApp.ClassLibrary.Repository
{
    public class EmployeeRepository : IRepository<int, Employee>, IEmployeeRepository
    {
        private readonly AirportDbContext dataBaseContext;

        public EmployeeRepository(AirportDbContext dataBaseContext)
        {
            this.dataBaseContext = dataBaseContext ?? throw new ArgumentNullException(nameof(dataBaseContext));
        }

        public int CreateNewEntity(Employee employeeEntity)
        {
            if (employeeEntity == null)
            {
                throw new ArgumentNullException(nameof(employeeEntity), "Employee cannot be null.");
            }

            this.dataBaseContext.employees.Add(employeeEntity);
            this.dataBaseContext.SaveChanges();
            return employeeEntity.Id;
        }

        public void DeleteById(int identificationNumber)
        {
            var employee = this.dataBaseContext.employees.FirstOrDefault(e => e.Id == identificationNumber);
            if (employee != null)
            {
                this.dataBaseContext.employees.Remove(employee);
                this.dataBaseContext.SaveChanges();
            }
        }

        public IEnumerable<Employee> GetAll()
        {
            return this.dataBaseContext.employees.ToList();
        }

        public Employee GetById(int identificationNumber)
        {
            var employee = this.dataBaseContext.employees.FirstOrDefault(e => e.Id == identificationNumber);
            if (employee == null)
            {
                throw new KeyNotFoundException($"Employee with id {identificationNumber} was not found.");
            }

            return employee;
        }

        public void UpdateById(int identificationNumber, Employee employeeEntity)
        {
            if (employeeEntity == null)
            {
                throw new ArgumentNullException(nameof(employeeEntity), "Employee cannot be null.");
            }

            var employee = this.dataBaseContext.employees.FirstOrDefault(e => e.Id == identificationNumber);
            if (employee != null)
            {
                employee.FullName = employeeEntity.FullName;
                employee.EmailAddress = employeeEntity.EmailAddress;
                employee.AssignedDepartment = employeeEntity.AssignedDepartment;
                this.dataBaseContext.SaveChanges();
            }
        }
    }
}
