using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Proxy.ServiceProxies;
using AirportApp.ClassLibrary.Service.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] !;

// Add services to the container.
// builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AirportDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dede Service Proxies:
builder.Services.AddHttpClient<IUserService, UserServiceProxy>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<IReviewService, ReviewServiceProxy>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<IFAQService, FAQServiceProxy>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<IMessageService, MessageServiceProxy>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// amalia Service Proxies
builder.Services.AddHttpClient<IChatService, ChatServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IComplaintTicketService, ComplaintTicketServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IDashboardService, DashboardServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IEmployeeService, EmployeeServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IAuthService, AuthServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IMembershipService, MembershipServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IDecisionTreeService, DecisionTreeServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IFlightSearchService, FlightSearchServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter());
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// app.UseStaticFiles();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");
// .WithStaticAssets();
app.Run();
