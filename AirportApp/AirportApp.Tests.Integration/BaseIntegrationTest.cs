using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.DataAccess;

namespace AirportApp.Tests.Integration;

public abstract class BaseIntegrationTest
{
    // Each test class gets its own isolated in-memory database so tests
    // run on any machine without a SQL Server dependency
    private readonly string dbName = $"AirportIntegrationTestDb_{Guid.NewGuid()}";

    protected AirportDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AirportDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        var dbContext = new AirportDbContext(options);
        dbContext.Database.EnsureCreated();
        return dbContext;
    }

    // Overload used by workflow tests that already hold a dbContext reference
    protected int GetFirstAvailableFlightId(AirportDbContext dbContext)
    {
        var flight = dbContext.Flights
            .OrderBy(f => f.Date > DateTime.Now ? 0 : 1)
            .ThenBy(f => f.Date)
            .FirstOrDefault();

        if (flight == null)
        {
            throw new Exception("No flights found in the database. Please check that EnsureCreated seeded the data.");
        }

        return flight.Id;
    }

    // Parameterless overload for tests that don't hold an existing dbContext
    protected int GetFirstAvailableFlightId()
    {
        using var dbContext = CreateDbContext();
        return GetFirstAvailableFlightId(dbContext);
    }
}