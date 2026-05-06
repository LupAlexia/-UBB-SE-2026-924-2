using System.Net.Http;
using Airport.Web;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.DataAccess;
using System.Linq;
using System;

namespace AirportApp.Tests.Integration;

public abstract class BaseApiIntegrationTest : IDisposable
{
    protected readonly WebApplicationFactory<Airport.Web.Controllers.UserController> factory;
    protected readonly HttpClient client;

    protected BaseApiIntegrationTest()
    {
        factory = new WebApplicationFactory<Airport.Web.Controllers.UserController>()
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

                    // Add the test DbContext using In-Memory provider
                    services.AddDbContext<AirportDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("AirportApiTestDb_" + Guid.NewGuid().ToString());
                    });

                    // Ensure database is created and seeded for the API tests
                    var sp = services.BuildServiceProvider();
                    using (var scope = sp.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<AirportDbContext>();
                        db.Database.EnsureCreated();
                    }
                });
            });

        client = factory.CreateClient();
    }

    public void Dispose()
    {
        client.Dispose();
        factory.Dispose();
    }
}
