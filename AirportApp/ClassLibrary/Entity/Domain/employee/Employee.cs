using AirportApp.ClassLibrary.Entity.Domain.Message;

namespace AirportApp.ClassLibrary.Entity.Domain.Employee
{
    public class Employee : ISender
    {
        //private int employeeId;
        //private string fullName;
        //private string emailAddress;
        //private EmployeeDepartment assignedDepartment;

        // 1. EF Core Auto-Properties
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public EmployeeDepartment AssignedDepartment { get; set; }

        // 2. Required Parameterless Constructor
        public Employee() { }

        public Employee(int employeeIdentificationNumber, string fullName, string emailAddress, EmployeeDepartment assignedDepartment)
        {
            Id = employeeIdentificationNumber;
            this.FullName = fullName;
            this.EmailAddress = emailAddress;
            this.AssignedDepartment = assignedDepartment;
        }

        public int EmployeeId => EmployeeId;
        public string GetDepartmentName() => AssignedDepartment.ToString();

        public string RetrieveConfiguredDisplayFullNameForBot() => FullName;
        public string RetrieveConfiguredEmailAddressForBotContact() => EmailAddress;
        public int RetrieveUniqueDatabaseIdentifierForBot() => EmployeeId;
    }
}
