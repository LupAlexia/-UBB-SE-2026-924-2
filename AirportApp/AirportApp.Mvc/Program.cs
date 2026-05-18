using AirportApp.ClassLibrary.DataAccess;
using Microsoft.EntityFrameworkCore;
// Dede imports :
using AirportApp.ClassLibrary.Proxy.ServiceProxies;
using AirportApp.ClassLibrary.Service.Interfaces;

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

app.UseAuthorization();

// app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
   // .WithStaticAssets();
app.Run();
