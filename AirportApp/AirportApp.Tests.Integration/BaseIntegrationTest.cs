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
        try
        {
            dbContext.Database.EnsureCreated();
        }
        catch (InvalidOperationException)
        {
            // InMemory provider can throw when there is a seeded discriminator value
            // with no mapped CLR type (e.g. 'Bot'). Fall back to manual seeding below.
        }

        // Ensure minimal test data exists (fallback for when model seeding fails)
        SeedTestData(dbContext);

        return dbContext;
    }

    private static void SeedTestData(AirportDbContext db)
    {
        // Minimal, idempotent seeding used by integration tests. Keep it small to
        // avoid depending on unrelated parts of the production seed.
        if (!db.Users.Any())
        {
            db.Users.AddRange(
                new AirportApp.ClassLibrary.Entity.Domain.User { Id = 101, FullName = "Alice Bot", EmailAddress = "alice@bot.com" },
                new AirportApp.ClassLibrary.Entity.Domain.User { Id = 102, FullName = "Bob Chat", EmailAddress = "bob@chat.com" },
                new AirportApp.ClassLibrary.Entity.Domain.User { Id = 103, FullName = "Mia Passenger", EmailAddress = "mia@example.com" });
            db.SaveChanges();
        }

        if (!db.Employees.Any())
        {
            db.Employees.AddRange(
                new AirportApp.ClassLibrary.Entity.Domain.Employee.Employee { Id = 1, FullName = "Alice Smith", EmailAddress = "alice@acme.com", AssignedDepartment = AirportApp.ClassLibrary.Entity.Domain.Employee.EmployeeDepartment.MEDICAL },
                new AirportApp.ClassLibrary.Entity.Domain.Employee.Employee { Id = 2, FullName = "Bob Johnson", EmailAddress = "bob@contoso.com", AssignedDepartment = AirportApp.ClassLibrary.Entity.Domain.Employee.EmployeeDepartment.LEGAL });
            db.SaveChanges();
        }

        if (!db.Companies.Any())
        {
            db.Companies.AddRange(
                new AirportApp.ClassLibrary.Entity.Domain.Company { Id = 1, Name = "Acme Airlines" },
                new AirportApp.ClassLibrary.Entity.Domain.Company { Id = 2, Name = "Contoso Air" });
            db.SaveChanges();
        }

        if (!db.Airports.Any())
        {
            db.Airports.AddRange(
                new AirportApp.ClassLibrary.Entity.Domain.Airport { Id = 1, AirportCode = "LAX", City = "Los Angeles" },
                new AirportApp.ClassLibrary.Entity.Domain.Airport { Id = 2, AirportCode = "JFK", City = "New York" });
            db.SaveChanges();
        }

        if (!db.Gates.Any())
        {
            var airport1 = db.Airports.FirstOrDefault(a => a.Id == 1);
            var airport2 = db.Airports.FirstOrDefault(a => a.Id == 2);
            if (airport1 != null && airport2 != null)
            {
                db.Gates.AddRange(
                    new AirportApp.ClassLibrary.Entity.Domain.Gate { Id = 1, GateName = "A1", Airport = airport1 },
                    new AirportApp.ClassLibrary.Entity.Domain.Gate { Id = 2, GateName = "B2", Airport = airport2 });
                db.SaveChanges();
            }
        }

        if (!db.Routes.Any())
        {
            var comp1 = db.Companies.FirstOrDefault(c => c.Id == 1);
            var ap1 = db.Airports.FirstOrDefault(a => a.Id == 1);
            if (comp1 != null && ap1 != null)
            {
                db.Routes.Add(new AirportApp.ClassLibrary.Entity.Domain.Route { Id = 1, Company = comp1, Airport = ap1, RouteType = "Departure", DepartureTime = DateTime.Now.AddDays(1), ArrivalTime = DateTime.Now.AddDays(1).AddHours(3), Capacity = 100 });
                db.SaveChanges();
            }
        }

        if (!db.Flights.Any())
        {
            var route = db.Routes.FirstOrDefault(r => r.Id == 1);
            var gate = db.Gates.FirstOrDefault(g => g.Id == 1);
            if (route != null && gate != null)
            {
                db.Flights.Add(new AirportApp.ClassLibrary.Entity.Domain.Flight(1, route, gate, DateTime.Now.AddDays(1), "AC100"));
                db.SaveChanges();
            }
        }

        // Seed AddOns, Memberships and membership-addon discounts when model seeding was skipped
        if (!db.AddOns.Any())
        {
            db.AddOns.AddRange(
                new AirportApp.ClassLibrary.Entity.Domain.AddOn { Id = 1, Name = "Extra Baggage", BasePrice = 30f },
                new AirportApp.ClassLibrary.Entity.Domain.AddOn { Id = 2, Name = "Priority Boarding", BasePrice = 15f },
                new AirportApp.ClassLibrary.Entity.Domain.AddOn { Id = 3, Name = "Seat Selection", BasePrice = 12f },
                new AirportApp.ClassLibrary.Entity.Domain.AddOn { Id = 4, Name = "Lounge Access", BasePrice = 45f });
            db.SaveChanges();
        }

        if (!db.Memberships.Any())
        {
            db.Memberships.AddRange(
                new AirportApp.ClassLibrary.Entity.Domain.Membership { Id = 1, Name = "Silver", FlightDiscountPercentage = 5f },
                new AirportApp.ClassLibrary.Entity.Domain.Membership { Id = 2, Name = "Gold", FlightDiscountPercentage = 15f },
                new AirportApp.ClassLibrary.Entity.Domain.Membership { Id = 3, Name = "Platinum", FlightDiscountPercentage = 25f });
            db.SaveChanges();
        }

        if (!db.MembershipAddonDiscounts.Any())
        {
            var m1 = db.Memberships.FirstOrDefault(m => m.Id == 1);
            var m2 = db.Memberships.FirstOrDefault(m => m.Id == 2);
            var m3 = db.Memberships.FirstOrDefault(m => m.Id == 3);
            var a1 = db.AddOns.FirstOrDefault(a => a.Id == 1);
            var a2 = db.AddOns.FirstOrDefault(a => a.Id == 2);
            var a3 = db.AddOns.FirstOrDefault(a => a.Id == 3);
            var a4 = db.AddOns.FirstOrDefault(a => a.Id == 4);

            if (m1 != null && m2 != null && m3 != null && a1 != null && a2 != null)
            {
                db.MembershipAddonDiscounts.AddRange(
                    new AirportApp.ClassLibrary.Entity.Domain.MembershipAddonDiscount(m1, a1, 10f),
                    new AirportApp.ClassLibrary.Entity.Domain.MembershipAddonDiscount(m1, a2, 10f),
                    new AirportApp.ClassLibrary.Entity.Domain.MembershipAddonDiscount(m2, a1, 20f),
                    new AirportApp.ClassLibrary.Entity.Domain.MembershipAddonDiscount(m2, a2, 20f));

                // extra discounts for premium
                if (a3 != null) db.MembershipAddonDiscounts.Add(new AirportApp.ClassLibrary.Entity.Domain.MembershipAddonDiscount(m3, a3, 30f));
                if (a4 != null) db.MembershipAddonDiscounts.Add(new AirportApp.ClassLibrary.Entity.Domain.MembershipAddonDiscount(m3, a4, 35f));

                db.SaveChanges();
            }
        }

        if (!db.Chats.Any())
        {
            var u101 = db.Users.FirstOrDefault(u => u.Id == 101);
            if (u101 != null)
            {
                db.Chats.Add(new AirportApp.ClassLibrary.Entity.Domain.Chats.Chat { Id = 1, User = u101, Status = AirportApp.ClassLibrary.Entity.Domain.Chats.ChatStatus.Active });
                db.SaveChanges();
            }
        }
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