using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.DataAccess;

namespace AirportApp.Tests.Integration;

public abstract class BaseApiIntegrationTest : IDisposable
{
    protected readonly WebApplicationFactory<Airport.Web.Controllers.UserController> Factory;
    protected readonly HttpClient Client;

    protected BaseApiIntegrationTest()
    {
        Factory = new WebApplicationFactory<Airport.Web.Controllers.UserController>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove the real DbContext registration
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AirportDbContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    // Use a unique in-memory database per test class instance so tests
                    // are fully isolated and run on any machine without a SQL Server dependency
                    var dbName = $"AirportTestDb_{Guid.NewGuid()}";
                    services.AddDbContext<AirportDbContext>(options =>
                    {
                        options.UseInMemoryDatabase(dbName);
                    });

                    // Build a temporary service provider to seed the in-memory database
                    // using the same HasData seed already defined in AirportDbContext.OnModelCreating
                    var serviceProvider = services.BuildServiceProvider();
                    using var scope = serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<AirportDbContext>();
                    dbContext.Database.EnsureCreated();
                });
            });

        Client = Factory.CreateClient();
    }

    public void Dispose()
    {
        Client.Dispose();
        Factory.Dispose();
    }
}