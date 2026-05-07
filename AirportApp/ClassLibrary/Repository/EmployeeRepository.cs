using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<int> CreateNewEntityAsync(Employee employeeEntity)
        {
            if (employeeEntity == null)
            {
                throw new ArgumentNullException(nameof(employeeEntity), "Employee cannot be null.");
            }

            this.dataBaseContext.Employees.Add(employeeEntity);
            await this.dataBaseContext.SaveChangesAsync();
            return employeeEntity.Id;
        }

        public async Task DeleteByIdAsync(int identificationNumber)
        {
            var employee = await this.dataBaseContext.Employees.FirstOrDefaultAsync(e => e.Id == identificationNumber);
            if (employee != null)
            {
                this.dataBaseContext.Employees.Remove(employee);
                await this.dataBaseContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await this.dataBaseContext.Employees.ToListAsync();
        }

        public async Task<Employee> GetByIdAsync(int identificationNumber)
        {
            var employee = await this.dataBaseContext.Employees.FirstOrDefaultAsync(e => e.Id == identificationNumber);
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

            var employee = await this.dataBaseContext.Employees.FirstOrDefaultAsync(e => e.Id == identificationNumber);
            if (employee != null)
            {
                employee.FullName = employeeEntity.FullName;
                employee.EmailAddress = employeeEntity.EmailAddress;
                employee.AssignedDepartment = employeeEntity.AssignedDepartment;
                await this.dataBaseContext.SaveChangesAsync();
            }
        }
    }
}
