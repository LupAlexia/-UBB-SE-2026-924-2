using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AirportApp.ClassLibrary.Entity.Domain.Message;

namespace AirportApp.ClassLibrary.Entity.Domain.Employee
{
    [Table("Employees")]
    public class Employee : ISender
    {
        //private int employeeId;
        //private string fullName;
        //private string emailAddress;
        //private EmployeeDepartment assignedDepartment;

        // 1. EF Core Auto-Properties

        [Key]
        [Column("Employee_Id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("Full_Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        [Column("Email_Address")]
        public string EmailAddress { get; set; } = string.Empty;

        [Required]
        [Column("Department")]
        public EmployeeDepartment AssignedDepartment { get; set; }

        // 2. Required Parameterless Constructor
        public Employee() { }

        public Employee(int employeeIdentificationNumber, string fullName, string emailAddress, EmployeeDepartment assignedDepartment)
        {
            Id = employeeIdentificationNumber;
            FullName = fullName;
            EmailAddress = emailAddress;
            AssignedDepartment = assignedDepartment;
        }

        //public int EmployeeId => EmployeeId;
        public string GetDepartmentName() => AssignedDepartment.ToString();

        public string RetrieveConfiguredDisplayFullNameForBot() => FullName;
        public string RetrieveConfiguredEmailAddressForBotContact() => EmailAddress;
        public int RetrieveUniqueDatabaseIdentifierForBot() => Id;
    }
}
