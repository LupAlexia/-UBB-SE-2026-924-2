using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Mvc.Models.Employee
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string EmailAddress { get; set; } = string.Empty;

        public EmployeeDepartment AssignedDepartment { get; set; }

        public string DepartmentName => AssignedDepartment.ToString();
    }
}
