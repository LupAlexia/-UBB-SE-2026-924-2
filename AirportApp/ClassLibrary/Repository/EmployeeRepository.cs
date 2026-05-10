using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository
{
    public class EmployeeRepository : IRepository<int, Employee>, IEmployeeRepository
    {
        private readonly AirportDbContext databaseContext;

        public EmployeeRepository(AirportDbContext databaseContext)
        {
            this.databaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
        }

        public async Task<int> CreateNewEntityAsync(Employee employeeEntity)
        {
            if (employeeEntity == null)
            {
                throw new ArgumentNullException(nameof(employeeEntity), "Employee cannot be null.");
            }

            this.databaseContext.Employees.Add(employeeEntity);
            await this.databaseContext.SaveChangesAsync();
            return employeeEntity.Id;
        }

        public async Task DeleteByIdAsync(int identificationNumber)
        {
            var employee = await this.databaseContext.Employees.FirstOrDefaultAsync(employee => employee.Id == identificationNumber);
            if (employee != null)
            {
                this.databaseContext.Employees.Remove(employee);
                await this.databaseContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await this.databaseContext.Employees.ToListAsync();
        }

        public async Task<Employee> GetByIdAsync(int identificationNumber)
        {
            var employee = await this.databaseContext.Employees.FirstOrDefaultAsync(employee => employee.Id == identificationNumber);
            if (employee == null)
            {
                throw new KeyNotFoundException($"Employee with id {identificationNumber} was not found.");
            }

            return employee;
        }

        public async Task UpdateByIdAsync(int identificationNumber, Employee employeeEntity)
        {
            if (employeeEntity == null)
            {
                throw new ArgumentNullException(nameof(employeeEntity), "Employee cannot be null.");
            }

            var employee = await this.databaseContext.Employees.FirstOrDefaultAsync(employee => employee.Id == identificationNumber);
            if (employee != null)
            {
                employee.FullName = employeeEntity.FullName;
                employee.EmailAddress = employeeEntity.EmailAddress;
                employee.AssignedDepartment = employeeEntity.AssignedDepartment;
                await this.databaseContext.SaveChangesAsync();
            }
        }
    }
}
