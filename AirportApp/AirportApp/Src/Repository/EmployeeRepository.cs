using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.Src.Model.Employee;
using AirportApp.Src.Repository.Database;
using Microsoft.Data.SqlClient;

namespace AirportApp.Src.Repository
{
    public class EmployeeRepository : DatabaseRepository<int, Employee>, IRepository<int, Employee>, IEmployeeRepository
    {
        public int CreateNewEntity(Employee employeeEntity)
        {
            if (employeeEntity == null)
            {
                throw new ArgumentNullException(nameof(employeeEntity), "Employee cannot be null.");
            }

            string insertQuery = "INSERT INTO Employee " +
                "(name, email, group) " +
                "OUTPUT INSERTED.Employee_id " +
                "VALUES (@name, @email, @group)";

            SqlCommand insertCommand = new SqlCommand(insertQuery);

            insertCommand.Parameters.AddWithValue("@name", employeeEntity.RetrieveConfiguredDisplayFullNameForBot());
            insertCommand.Parameters.AddWithValue("@email", employeeEntity.RetrieveConfiguredEmailAddressForBotContact());
            insertCommand.Parameters.AddWithValue("@group", employeeEntity.GetDepartmentName());

            int identificationNumber = Add(insertCommand, employeeEntity);
            return identificationNumber;
        }

        public void DeleteById(int identificationNumber)
        {
            string deleteQuery = "DELETE FROM Employee WHERE employee_id = @id";
            SqlCommand deleteCommand = new SqlCommand(deleteQuery);
            deleteCommand.Parameters.AddWithValue("@id", identificationNumber);

            DeleteById(identificationNumber, deleteCommand);
        }

        public IEnumerable<Employee> GetAll()
        {
            string selectAllQuery = "SELECT * FROM Employee";
            SqlCommand selectCommand = new SqlCommand(selectAllQuery);
            return GetAll(selectCommand);
        }

        public Employee GetById(int identificationNumber)
        {
            string selectByIdQuery = "SELECT * FROM Employee WHERE employee_id = @id";
            SqlCommand selectByIdCommand = new SqlCommand(selectByIdQuery);
            selectByIdCommand.Parameters.AddWithValue("@id", identificationNumber);

            Employee foundEmployee = GetById(identificationNumber, selectByIdCommand);

            if (foundEmployee == null)
            {
                throw new KeyNotFoundException($"Employee with id {identificationNumber} was not found.");
            }

            return foundEmployee;
        }

        public void UpdateById(int identificationNumber, Employee employeeEntity)
        {
            if (employeeEntity == null)
            {
                throw new ArgumentNullException(nameof(employeeEntity), "Employee cannot be null.");
            }

            string updateQuery = "UPDATE Employee SET " +
                "name = @name, " +
                "email = @email " +
                "group = @group " +
                "WHERE employee_id = @id";

            SqlCommand updateCommand = new SqlCommand(updateQuery);

            updateCommand.Parameters.AddWithValue("@id", identificationNumber);
            updateCommand.Parameters.AddWithValue("@name", employeeEntity.RetrieveConfiguredDisplayFullNameForBot());
            updateCommand.Parameters.AddWithValue("@email", employeeEntity.RetrieveConfiguredEmailAddressForBotContact());
            updateCommand.Parameters.AddWithValue("@group", employeeEntity.GetDepartmentName());

            UpdateById(identificationNumber, updateCommand, employeeEntity);
        }

        protected override int GetEntityId(Employee employeeEntity)
        {
            return employeeEntity.EmployeeId;
        }

        protected override Employee MapRowToEntity(SqlDataReader dataReader)
        {
            int employeeIdentificationNumber = dataReader.GetInt32(dataReader.GetOrdinal("employee_id"));
            string employeeFullName = dataReader.GetString(dataReader.GetOrdinal("name"));
            string employeeEmailAddress = dataReader.GetString(dataReader.GetOrdinal("email"));
            string departmentName = dataReader.GetString(dataReader.GetOrdinal("group"));

            EmployeeDepartment departmentEnum = (EmployeeDepartment)Enum.Parse(typeof(EmployeeDepartment), departmentName);

            return new Employee(employeeIdentificationNumber, employeeFullName, employeeEmailAddress, departmentEnum);
        }
    }
}
