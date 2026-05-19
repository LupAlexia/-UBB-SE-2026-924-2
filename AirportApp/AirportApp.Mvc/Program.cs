using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Proxy.ServiceProxies;
using AirportApp.ClassLibrary.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] !;

// Add services to the container.
builder.Services.AddControllersWithViews();

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
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.UseStaticFiles();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
   // .WithStaticAssets();
app.Run();
