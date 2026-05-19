using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AirportApp.Mvc.Models;
using Microsoft.AspNetCore.Authorization;

namespace AirportApp.Mvc.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> logger;

    public HomeController(ILogger<HomeController> logger)
    {
        this.logger = logger;
    }

    public IActionResult Index()
    {
        // TEMP DEBUG - remove after fixing
        var isAuth = User.Identity?.IsAuthenticated;
        var name = User.Identity?.Name;
        var authType = User.Identity?.AuthenticationType;

        System.Console.WriteLine($"IsAuthenticated: {isAuth}, Name: {name}, AuthType: {authType}");

        if (isAuth != true)
        {
            return RedirectToAction("Login", "Account");
        }

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
