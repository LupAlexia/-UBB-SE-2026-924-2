using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    [Table("Employees")]
    public class Employee : Sender
    {
        // Id, FullName, EmailAddress inherited from Sender
        [Required]
        [Column("Department")]
        public EmployeeDepartment AssignedDepartment { get; set; }

        public Employee()
        {
            Discriminator = "Employee";
        }

        public Employee(int employeeIdentificationNumber, string fullName, string emailAddress, EmployeeDepartment assignedDepartment)
            : base(employeeIdentificationNumber, fullName, emailAddress)
        {
            AssignedDepartment = assignedDepartment;
            Discriminator = "Employee";
        }

        public string GetDepartmentName() => AssignedDepartment.ToString();

        public override string RetrieveConfiguredDisplayFullNameForBot() => FullName;
        public override string RetrieveConfiguredEmailAddressForBotContact() => EmailAddress;
        public override int RetrieveUniqueDatabaseIdentifierForBot() => Id;
    }
}
