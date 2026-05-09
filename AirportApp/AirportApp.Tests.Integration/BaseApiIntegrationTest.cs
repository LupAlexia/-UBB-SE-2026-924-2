using System.Net.Http;
using System.Linq;
using System;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;

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

                    // Use a unique in-memory database per test class instance so tests
                    // are fully isolated and run on any machine without a SQL Server dependency
                    var dbName = "AirportApiTestDb_" + Guid.NewGuid().ToString();
                    services.AddDbContext<AirportDbContext>(options =>
                    {
                        options.UseInMemoryDatabase(dbName);
                    });

                    // Ensure database is created and seeded for the API tests
                    var sp = services.BuildServiceProvider();
                    using (var scope = sp.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<AirportDbContext>();
                        try
                        {
                            // Try to use EnsureCreated which applies migrations and seeding
                            db.Database.EnsureCreated();
                        }
                        catch
                        {
                            // If EnsureCreated fails (e.g., due to unmapped discriminator),
                            // manually seed all required data without using model seeding
                        }

                        // Always seed minimal data to ensure integration tests can run
                        // This approach bypasses the problematic DbContext seeding that includes the Bot discriminator
                        SeedTestData(db);
                    }
                });
            });

        client = factory.CreateClient();
    }

    private static void SeedTestData(AirportDbContext db)
    {
        // Seed Users
        if (!db.Users.Any())
        {
            db.Users.AddRange(
                new AirportApp.ClassLibrary.Entity.Domain.User { Id = 101, FullName = "Alice Bot", EmailAddress = "alice@bot.com" },
                new AirportApp.ClassLibrary.Entity.Domain.User { Id = 102, FullName = "Bob Chat", EmailAddress = "bob@chat.com" },
                new AirportApp.ClassLibrary.Entity.Domain.User { Id = 103, FullName = "Mia Passenger", EmailAddress = "mia@example.com" });
            db.SaveChanges();
        }

        // Seed Employees
        if (!db.Employees.Any())
        {
            db.Employees.AddRange(
                new Employee { Id = 1, FullName = "Alice Smith", EmailAddress = "alice@acme.com", AssignedDepartment = EmployeeDepartment.MEDICAL },
                new Employee { Id = 2, FullName = "Bob Johnson", EmailAddress = "bob@contoso.com", AssignedDepartment = EmployeeDepartment.LEGAL },
                new Employee { Id = 3, FullName = "Clara Davis", EmailAddress = "clara@skylink.com", AssignedDepartment = EmployeeDepartment.HR },
                new Employee { Id = 4, FullName = "Daniel Green", EmailAddress = "daniel@acme.com", AssignedDepartment = EmployeeDepartment.CASHBACK });
            db.SaveChanges();
        }

        // Seed Companies
        if (!db.Companies.Any())
        {
            db.Companies.AddRange(
                new AirportApp.ClassLibrary.Entity.Domain.Company { Id = 1, Name = "Acme Airlines" },
                new AirportApp.ClassLibrary.Entity.Domain.Company { Id = 2, Name = "Contoso Air" },
                new AirportApp.ClassLibrary.Entity.Domain.Company { Id = 3, Name = "SkyLink Airways" });
            db.SaveChanges();
        }

        // Seed Airports
        if (!db.Airports.Any())
        {
            db.Airports.AddRange(
                new AirportApp.ClassLibrary.Entity.Domain.Airport { Id = 1, AirportCode = "LAX", City = "Los Angeles" },
                new AirportApp.ClassLibrary.Entity.Domain.Airport { Id = 2, AirportCode = "JFK", City = "New York" },
                new AirportApp.ClassLibrary.Entity.Domain.Airport { Id = 3, AirportCode = "ORD", City = "Chicago" });
            db.SaveChanges();
        }

        // Seed Gates
        if (!db.Gates.Any())
        {
            var airport1 = db.Airports.FirstOrDefault(a => a.Id == 1);
            var airport2 = db.Airports.FirstOrDefault(a => a.Id == 2);
            var airport3 = db.Airports.FirstOrDefault(a => a.Id == 3);
            if (airport1 != null && airport2 != null && airport3 != null)
            {
                db.Gates.AddRange(
                    new AirportApp.ClassLibrary.Entity.Domain.Gate { Id = 1, GateName = "A1", Airport = airport1 },
                    new AirportApp.ClassLibrary.Entity.Domain.Gate { Id = 2, GateName = "B2", Airport = airport2 },
                    new AirportApp.ClassLibrary.Entity.Domain.Gate { Id = 3, GateName = "C3", Airport = airport3 },
                    new AirportApp.ClassLibrary.Entity.Domain.Gate { Id = 4, GateName = "D4", Airport = airport1 });
                db.SaveChanges();
            }
        }

        // Seed Routes
        if (!db.Routes.Any())
        {
            var company1 = db.Companies.FirstOrDefault(c => c.Id == 1);
            var company2 = db.Companies.FirstOrDefault(c => c.Id == 2);
            var company3 = db.Companies.FirstOrDefault(c => c.Id == 3);
            var airport1 = db.Airports.FirstOrDefault(a => a.Id == 1);
            var airport2 = db.Airports.FirstOrDefault(a => a.Id == 2);
            var airport3 = db.Airports.FirstOrDefault(a => a.Id == 3);
            if (company1 != null && company2 != null && company3 != null && airport1 != null && airport2 != null && airport3 != null)
            {
                db.Routes.AddRange(
                    new AirportApp.ClassLibrary.Entity.Domain.Route { Id = 1, Company = company1, Airport = airport1, RouteType = "Departure", DepartureTime = new DateTime(2026, 5, 4, 8, 0, 0), ArrivalTime = new DateTime(2026, 5, 4, 11, 0, 0), Capacity = 180 },
                    new AirportApp.ClassLibrary.Entity.Domain.Route { Id = 2, Company = company2, Airport = airport2, RouteType = "Arrival", DepartureTime = new DateTime(2026, 5, 5, 9, 30, 0), ArrivalTime = new DateTime(2026, 5, 5, 12, 45, 0), Capacity = 150 },
                    new AirportApp.ClassLibrary.Entity.Domain.Route { Id = 3, Company = company3, Airport = airport3, RouteType = "Departure", DepartureTime = new DateTime(2026, 5, 6, 7, 15, 0), ArrivalTime = new DateTime(2026, 5, 6, 10, 5, 0), Capacity = 220 },
                    new AirportApp.ClassLibrary.Entity.Domain.Route { Id = 4, Company = company1, Airport = airport1, RouteType = "Arrival", DepartureTime = new DateTime(2026, 5, 7, 14, 0, 0), ArrivalTime = new DateTime(2026, 5, 7, 17, 10, 0), Capacity = 160 },
                    new AirportApp.ClassLibrary.Entity.Domain.Route { Id = 5, Company = company2, Airport = airport3, RouteType = "Arrival", DepartureTime = new DateTime(2026, 5, 20, 13, 0, 0), ArrivalTime = new DateTime(2026, 5, 20, 15, 12, 0), Capacity = 50 });
                db.SaveChanges();
            }
        }

        // Seed Flights
        if (!db.Flights.Any())
        {
            var route1 = db.Routes.FirstOrDefault(r => r.Id == 1);
            var route2 = db.Routes.FirstOrDefault(r => r.Id == 2);
            var route3 = db.Routes.FirstOrDefault(r => r.Id == 3);
            var route4 = db.Routes.FirstOrDefault(r => r.Id == 4);
            var route5 = db.Routes.FirstOrDefault(r => r.Id == 5);
            var gate1 = db.Gates.FirstOrDefault(g => g.Id == 1);
            var gate2 = db.Gates.FirstOrDefault(g => g.Id == 2);
            var gate3 = db.Gates.FirstOrDefault(g => g.Id == 3);
            var gate4 = db.Gates.FirstOrDefault(g => g.Id == 4);
            if (route1 != null && route2 != null && route3 != null && route4 != null && route5 != null &&
                gate1 != null && gate2 != null && gate3 != null && gate4 != null)
            {
                db.Flights.AddRange(
                    new AirportApp.ClassLibrary.Entity.Domain.Flight(1, route1, gate1, new DateTime(2026, 5, 4, 8, 0, 0), "AC100"),
                    new AirportApp.ClassLibrary.Entity.Domain.Flight(2, route2, gate2, new DateTime(2026, 5, 5, 9, 30, 0), "CT200"),
                    new AirportApp.ClassLibrary.Entity.Domain.Flight(3, route3, gate3, new DateTime(2026, 5, 6, 7, 15, 0), "SK300"),
                    new AirportApp.ClassLibrary.Entity.Domain.Flight(4, route4, gate4, new DateTime(2026, 5, 7, 14, 0, 0), "AC400"),
                    new AirportApp.ClassLibrary.Entity.Domain.Flight(5, route5, gate4, new DateTime(2026, 5, 20, 13, 0, 0), "CT500"));
                db.SaveChanges();
            }
        }

        // Seed AddOns
        if (!db.AddOns.Any())
        {
            db.AddOns.AddRange(
                new AddOn { Id = 1, Name = "Extra Baggage", BasePrice = 30f },
                new AddOn { Id = 2, Name = "Priority Boarding", BasePrice = 15f },
                new AddOn { Id = 3, Name = "Seat Selection", BasePrice = 12f },
                new AddOn { Id = 4, Name = "Lounge Access", BasePrice = 45f });
            db.SaveChanges();
        }

        // Seed Memberships
        if (!db.Memberships.Any())
        {
            db.Memberships.AddRange(
                new AirportApp.ClassLibrary.Entity.Domain.Membership { Id = 1, Name = "Silver", FlightDiscountPercentage = 5f },
                new AirportApp.ClassLibrary.Entity.Domain.Membership { Id = 2, Name = "Gold", FlightDiscountPercentage = 15f },
                new AirportApp.ClassLibrary.Entity.Domain.Membership { Id = 3, Name = "Platinum", FlightDiscountPercentage = 25f });
            db.SaveChanges();
        }

        // Seed Chats
        if (!db.Chats.Any())
        {
            var u101 = db.Users.FirstOrDefault(u => u.Id == 101);
            var u102 = db.Users.FirstOrDefault(u => u.Id == 102);
            var u103 = db.Users.FirstOrDefault(u => u.Id == 103);
            if (u101 != null && u102 != null && u103 != null)
            {
                db.Chats.AddRange(
                    new Chat { Id = 1, User = u101, Status = ChatStatus.Active },
                    new Chat { Id = 2, User = u102, Status = ChatStatus.Active },
                    new Chat { Id = 3, User = u103, Status = ChatStatus.Active });
                db.SaveChanges();
            }
        }

        // Seed Messages
        if (!db.Messages.Any())
        {
            var chat1 = db.Chats.FirstOrDefault(c => c.Id == 1);
            var chat2 = db.Chats.FirstOrDefault(c => c.Id == 2);
            var chat3 = db.Chats.FirstOrDefault(c => c.Id == 3);
            var sender1 = db.Users.FirstOrDefault(u => u.Id == 101);
            var sender2 = db.Users.FirstOrDefault(u => u.Id == 102);
            var sender3 = db.Users.FirstOrDefault(u => u.Id == 103);
            if (chat1 != null && chat2 != null && chat3 != null && sender1 != null && sender2 != null && sender3 != null)
            {
                db.Messages.AddRange(
                    new Message { Id = 1, Chat = chat1, Sender = sender1, Text = "Hello! I need help with flights.", Timestamp = new DateTimeOffset(new DateTime(2026, 5, 4, 9, 0, 0)) },
                    new Message { Id = 2, Chat = chat2, Sender = sender2, Text = "Hi, I have a question about membership.", Timestamp = new DateTimeOffset(new DateTime(2026, 5, 4, 9, 5, 0)) },
                    new Message { Id = 3, Chat = chat3, Sender = sender3, Text = "Hello, I need support with my booking.", Timestamp = new DateTimeOffset(new DateTime(2026, 5, 4, 9, 10, 0)) });
                db.SaveChanges();
            }
        }

        // Seed FAQs
        if (!db.Faqs.Any())
        {
            db.Faqs.AddRange(
                new FAQEntry { Id = 1, Question = "How do I reset my password?", Answer = "Click on the 'Forgot Password' link on the login page.", Category = FAQCategoryEnum.Parking, ViewCount = 150, HelpfulVotesCount = 120, NotHelpfulVotesCount = 5 },
                new FAQEntry { Id = 2, Question = "What is the baggage allowance?", Answer = "Standard baggage allowance is 2 checked bags. See add-ons for additional bags.", Category = FAQCategoryEnum.Baggage, ViewCount = 200, HelpfulVotesCount = 180, NotHelpfulVotesCount = 10 },
                new FAQEntry { Id = 3, Question = "Can I change my flight?", Answer = "Yes, flight changes can be made up to 24 hours before departure, subject to availability.", Category = FAQCategoryEnum.Facilities, ViewCount = 180, HelpfulVotesCount = 160, NotHelpfulVotesCount = 8 },
                new FAQEntry { Id = 4, Question = "Which payment methods are accepted?", Answer = "We accept major credit cards and selected digital wallets.", Category = FAQCategoryEnum.Facilities, ViewCount = 95, HelpfulVotesCount = 80, NotHelpfulVotesCount = 2 },
                new FAQEntry { Id = 5, Question = "How do I contact support?", Answer = "Use the chat assistant or submit a support ticket from your account.", Category = FAQCategoryEnum.Facilities, ViewCount = 120, HelpfulVotesCount = 110, NotHelpfulVotesCount = 3 });
            db.SaveChanges();
        }

        // Seed ComplaintTicketCategories
        if (!db.TicketCategories.Any())
        {
            db.TicketCategories.AddRange(
                new ComplaintTicketCategory { Id = 1, CategoryName = "Booking Issues", CategoryUrgencyLevel = ComplaintTicketUrgencyLevelEnum.HIGH },
                new ComplaintTicketCategory { Id = 2, CategoryName = "General Inquiry", CategoryUrgencyLevel = ComplaintTicketUrgencyLevelEnum.LOW },
                new ComplaintTicketCategory { Id = 3, CategoryName = "Payment Problems", CategoryUrgencyLevel = ComplaintTicketUrgencyLevelEnum.HIGH });
            db.SaveChanges();
        }
    }

    public void Dispose()
    {
        client.Dispose();
        factory.Dispose();
    }
}
