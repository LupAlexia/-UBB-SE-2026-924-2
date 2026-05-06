using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.DataAccess;
using System;
using System.Linq;

namespace AirportApp.Tests.Integration;

public abstract class BaseIntegrationTest
{
    protected AirportDbContext CreateDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AirportDbContext>();
        // Use In-Memory database for tests instead of a real SQL Server
        optionsBuilder.UseInMemoryDatabase("AirportTestDb_" + Guid.NewGuid().ToString());
        
        var context = new AirportDbContext(optionsBuilder.Options);
        
        // Ensure the database is created and seeded (seeds are defined in OnModelCreating)
        context.Database.EnsureCreated();
        
        return context;
    }

    protected int GetFirstAvailableFlightId()
    {
        using var dbContext = CreateDbContext();
        var flight = dbContext.flights
            .OrderBy(f => f.Date > DateTime.Now ? 0 : 1)
            .ThenBy(f => f.Date)
            .FirstOrDefault();

        if (flight == null)
        {
            throw new Exception("No flights found in the test database.");
        }

        return flight.Id;
    }
}
