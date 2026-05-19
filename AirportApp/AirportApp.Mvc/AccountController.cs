using System.Security.Claims;
using AirportApp.ClassLibrary.Service.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class AccountController : Controller
{
    private readonly IUserService userService;
    private readonly IEmployeeService employeeService;

    public AccountController(IUserService userService, IEmployeeService employeeService)
    {
        this.userService = userService;
        this.employeeService = employeeService;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login() => View();

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(int userId, string role)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        };

        if (role == "Employee")
        {
            var employee = await employeeService.GetEmployeeByIdAsync(userId);
            if (employee == null)
            {
                ModelState.AddModelError(string.Empty, "Employee not found.");
                return View();
            }
            claims.Add(new Claim(ClaimTypes.Name, employee.FullName));
            claims.Add(new Claim(ClaimTypes.Role, "Employee"));
        }
        else
        {
            var user = await userService.GetByIdAsync(userId);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return View();
            }
            claims.Add(new Claim(ClaimTypes.Name, user.FullName));
            claims.Add(new Claim(ClaimTypes.Role, "User"));
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal);

        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    public IActionResult AccessDenied() => View();

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}