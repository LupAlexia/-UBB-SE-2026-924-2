using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Repository;
using AirportApp.ClassLibrary.Entity.Domain;
using System.Linq;
using System;

namespace AirportApp.Tests.Integration;

public abstract class BaseIntegrationTest
{
    protected string GetTestConnectionString()
    {
        return "Server=DESKTOP-NENJ194\\SQLEXPRESS;Database=AirportAppDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;";
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
            throw new Exception("Nu s-au gasit zboruri in baza de date. Va rugam sa rulati scriptul de seed.");
        }

        return flight.Id;
    }
    protected AirportDbContext CreateDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AirportDbContext>();
        optionsBuilder.UseSqlServer(GetTestConnectionString());
        return new AirportDbContext(optionsBuilder.Options);
    }
}
