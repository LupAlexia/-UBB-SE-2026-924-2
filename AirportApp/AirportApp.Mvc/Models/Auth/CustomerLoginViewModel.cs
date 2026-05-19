using System.ComponentModel.DataAnnotations;

namespace AirportApp.Mvc.Models.Auth;

public class CustomerLoginViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;
}