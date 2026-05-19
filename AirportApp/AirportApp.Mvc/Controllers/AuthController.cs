using System.Security.Claims;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interfaces;
using AirportApp.Mvc.Models.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirportApp.Mvc.Controllers;

public class AuthController : Controller
{
    private static readonly TimeSpan CookieLifetime = TimeSpan.FromHours(8);

    private readonly IAuthService authService;
    private readonly IEmployeeService employeeService;

    public AuthController(IAuthService authService, IEmployeeService employeeService)
    {
        this.authService = authService;
        this.employeeService = employeeService;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult ChooseRole()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction(nameof(Index), "Home");
        }

        return View();
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult CustomerLogin()
    {
        return View(new CustomerLoginViewModel());
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CustomerLogin(CustomerLoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            Customer customer = await authService.LoginAsync(model.Email, model.Password);
            await SignInAsync(customer.Id.ToString(), customer.Username, customer.Email, new[] { "Customer" }, customer);
            return RedirectToAction(nameof(Index), "Home");
        }
        catch (Exception exception) when (exception is InvalidOperationException or ArgumentException)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(model);
        }
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult EmployeeLogin()
    {
        return View(new EmployeeLoginViewModel());
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EmployeeLogin(EmployeeLoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            Employee employee = await employeeService.GetEmployeeByIdAsync(model.EmployeeId);

            if (!string.Equals(employee.FullName.Trim(), model.FullName.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(string.Empty, "The employee id and full name do not match.");
                return View(model);
            }

            string[] roles = employee.AssignedDepartment == EmployeeDepartment.ADMIN
                ? new[] { "Employee", EmployeeDepartment.ADMIN.ToString() }
                : new[] { "Employee" };

            await SignInAsync(employee.Id.ToString(), employee.FullName, employee.EmailAddress, roles, null);
            return RedirectToAction(nameof(Index), "Home");
        }
        catch (KeyNotFoundException)
        {
            ModelState.AddModelError(string.Empty, "Employee not found.");
            return View(model);
        }
        catch (Exception exception) when (exception is InvalidOperationException or ArgumentException)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        UserSession.CurrentUser = null;
        UserSession.PendingBookingParameters = null;
        return RedirectToAction(nameof(ChooseRole));
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private async Task SignInAsync(string userId, string displayName, string email, IEnumerable<string> roles, Customer? customer)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, displayName),
            new Claim(ClaimTypes.Email, email)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.Add(CookieLifetime)
            });

        UserSession.CurrentUser = customer ?? new Customer
        {
            Id = int.TryParse(userId, out int parsedId) ? parsedId : 0,
            Username = displayName,
            Email = email,
            PasswordHash = string.Empty
        };
    }
}