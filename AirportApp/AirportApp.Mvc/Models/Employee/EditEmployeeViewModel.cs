using System.ComponentModel.DataAnnotations;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Mvc.Models.Employee
{
    public class EditEmployeeViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Department")]
        public EmployeeDepartment AssignedDepartment { get; set; }
    }
}
