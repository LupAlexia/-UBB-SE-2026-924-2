using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;

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

    private static void SeedTestData(AirportDbContext dataBaseContext)
    {
        // Minimal, idempotent seeding used by integration tests. Keep it small to
        // avoid depending on unrelated parts of the production seed.
        if (!dataBaseContext.Users.Any())
        {
            dataBaseContext.Users.AddRange(
                new User { Id = 101, FullName = "Alice Bot", EmailAddress = "alice@bot.com" },
                new User { Id = 102, FullName = "Bob Chat", EmailAddress = "bob@chat.com" },
                new User { Id = 103, FullName = "Mia Passenger", EmailAddress = "mia@example.com" });
            dataBaseContext.SaveChanges();
        }

        if (!dataBaseContext.Employees.Any())
        {
            dataBaseContext.Employees.AddRange(
                new Employee { Id = 1, FullName = "Alice Smith", EmailAddress = "alice@acme.com", AssignedDepartment = EmployeeDepartment.MEDICAL },
                new Employee { Id = 2, FullName = "Bob Johnson", EmailAddress = "bob@contoso.com", AssignedDepartment = EmployeeDepartment.LEGAL });
            dataBaseContext.SaveChanges();
        }

        if (!dataBaseContext.Companies.Any())
        {
            dataBaseContext.Companies.AddRange(
                new Company { Id = 1, Name = "Acme Airlines" },
                new Company { Id = 2, Name = "Contoso Air" });
            dataBaseContext.SaveChanges();
        }

        if (!dataBaseContext.Airports.Any())
        {
            dataBaseContext.Airports.AddRange(
                new AirportApp.ClassLibrary.Entity.Domain.Airport { Id = 1, AirportCode = "LAX", City = "Los Angeles" },
                new AirportApp.ClassLibrary.Entity.Domain.Airport { Id = 2, AirportCode = "JFK", City = "New York" });
            dataBaseContext.SaveChanges();
        }

        if (!dataBaseContext.Gates.Any())
        {
            var airport1 = dataBaseContext.Airports.FirstOrDefault(airport => airport.Id == 1);
            var airport2 = dataBaseContext.Airports.FirstOrDefault(airport => airport.Id == 2);
            if (airport1 != null && airport2 != null)
            {
                dataBaseContext.Gates.AddRange(
                    new Gate { Id = 1, GateName = "A1", Airport = airport1 },
                    new Gate { Id = 2, GateName = "B2", Airport = airport2 });
                dataBaseContext.SaveChanges();
            }
        }

        if (!dataBaseContext.Routes.Any())
        {
            var company1 = dataBaseContext.Companies.FirstOrDefault(company => company.Id == 1);
            var airport1 = dataBaseContext.Airports.FirstOrDefault(airport => airport.Id == 1);
            if (company1 != null && airport1 != null)
            {
                dataBaseContext.Routes.Add(new Route { Id = 1, Company = company1, Airport = airport1, RouteType = "Departure", DepartureTime = DateTime.Now.AddDays(1), ArrivalTime = DateTime.Now.AddDays(1).AddHours(3), Capacity = 100 });
                dataBaseContext.SaveChanges();
            }
        }

        if (!dataBaseContext.Flights.Any())
        {
            var route = dataBaseContext.Routes.FirstOrDefault(route => route.Id == 1);
            var gate = dataBaseContext.Gates.FirstOrDefault(gate => gate.Id == 1);
            if (route != null && gate != null)
            {
                dataBaseContext.Flights.Add(new AirportApp.ClassLibrary.Entity.Domain.Flight(1, route, gate, DateTime.Now.AddDays(1), "AC100"));
                dataBaseContext.SaveChanges();
            }
        }

        // Seed AddOns, Memberships and membership-addon discounts when model seeding was skipped
        if (!dataBaseContext.AddOns.Any())
        {
            dataBaseContext.AddOns.AddRange(
                new AddOn { Id = 1, Name = "Extra Baggage", BasePrice = 30f },
                new AddOn { Id = 2, Name = "Priority Boarding", BasePrice = 15f },
                new AddOn { Id = 3, Name = "Seat Selection", BasePrice = 12f },
                new AddOn { Id = 4, Name = "Lounge Access", BasePrice = 45f });
            dataBaseContext.SaveChanges();
        }

        if (!dataBaseContext.Memberships.Any())
        {
            dataBaseContext.Memberships.AddRange(
                new Membership { Id = 1, Name = "Silver", FlightDiscountPercentage = 5f },
                new Membership { Id = 2, Name = "Gold", FlightDiscountPercentage = 15f },
                new Membership { Id = 3, Name = "Platinum", FlightDiscountPercentage = 25f });
            dataBaseContext.SaveChanges();
        }

        if (!dataBaseContext.MembershipAddonDiscounts.Any())
        {
            var membership1 = dataBaseContext.Memberships.FirstOrDefault(membership => membership.Id == 1);
            var membership2 = dataBaseContext.Memberships.FirstOrDefault(membership => membership.Id == 2);
            var membership3 = dataBaseContext.Memberships.FirstOrDefault(membership => membership.Id == 3);
            var addOn1 = dataBaseContext.AddOns.FirstOrDefault(addon => addon.Id == 1);
            var addOn2 = dataBaseContext.AddOns.FirstOrDefault(addon => addon.Id == 2);
            var addOn3 = dataBaseContext.AddOns.FirstOrDefault(addon => addon.Id == 3);
            var addOn4 = dataBaseContext.AddOns.FirstOrDefault(addon => addon.Id == 4);

            if (membership1 != null && membership2 != null && membership3 != null && addOn1 != null && addOn2 != null)
            {
                dataBaseContext.MembershipAddonDiscounts.AddRange(
                    new MembershipAddonDiscount(membership1, addOn1, 10f),
                    new MembershipAddonDiscount(membership1, addOn2, 10f),
                    new MembershipAddonDiscount(membership2, addOn1, 20f),
                    new MembershipAddonDiscount(membership2, addOn2, 20f));

                // extra discounts for premium
                if (addOn3 != null)
                {
                    dataBaseContext.MembershipAddonDiscounts.Add(new MembershipAddonDiscount(membership3, addOn3, 30f));
                }
                if (addOn4 != null)
                {
                    dataBaseContext.MembershipAddonDiscounts.Add(new MembershipAddonDiscount(membership3, addOn4, 35f));
                }

                dataBaseContext.SaveChanges();
            }
        }

        if (!dataBaseContext.Chats.Any())
        {
            var user101 = dataBaseContext.Users.FirstOrDefault(user => user.Id == 101);
            if (user101 != null)
            {
                dataBaseContext.Chats.Add(new Chat { Id = 1, User = user101, Status = ChatStatus.Active });
                dataBaseContext.SaveChanges();
            }
        }
    }

    // Overload used by workflow tests that already hold a dbContext reference
    protected int GetFirstAvailableFlightId(AirportDbContext dbContext)
    {
        var flight = dbContext.Flights
            .OrderBy(flightItem => flightItem.Date > DateTime.Now ? 0 : 1)
            .ThenBy(flightItem => flightItem.Date)
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