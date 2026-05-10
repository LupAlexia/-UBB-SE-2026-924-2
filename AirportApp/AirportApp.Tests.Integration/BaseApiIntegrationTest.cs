using System.Net.Http;
using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
                        serviceDescriptor => serviceDescriptor.ServiceType == typeof(DbContextOptions<AirportDbContext>));

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

    private static void SeedTestData(AirportDbContext dataBaseContext)
    {
        // Seed Users
        if (!dataBaseContext.Users.Any())
        {
            dataBaseContext.Users.AddRange(
                new AirportApp.ClassLibrary.Entity.Domain.User { Id = 101, FullName = "Alice Bot", EmailAddress = "alice@bot.com" },
                new AirportApp.ClassLibrary.Entity.Domain.User { Id = 102, FullName = "Bob Chat", EmailAddress = "bob@chat.com" },
                new AirportApp.ClassLibrary.Entity.Domain.User { Id = 103, FullName = "Mia Passenger", EmailAddress = "mia@example.com" });
            dataBaseContext.SaveChanges();
        }

        // Seed Employees
        if (!dataBaseContext.Employees.Any())
        {
            dataBaseContext.Employees.AddRange(
                new Employee { Id = 1, FullName = "Alice Smith", EmailAddress = "alice@acme.com", AssignedDepartment = EmployeeDepartment.MEDICAL },
                new Employee { Id = 2, FullName = "Bob Johnson", EmailAddress = "bob@contoso.com", AssignedDepartment = EmployeeDepartment.LEGAL },
                new Employee { Id = 3, FullName = "Clara Davis", EmailAddress = "clara@skylink.com", AssignedDepartment = EmployeeDepartment.HR },
                new Employee { Id = 4, FullName = "Daniel Green", EmailAddress = "daniel@acme.com", AssignedDepartment = EmployeeDepartment.CASHBACK });
            dataBaseContext.SaveChanges();
        }

        // Seed Companies
        if (!dataBaseContext.Companies.Any())
        {
            dataBaseContext.Companies.AddRange(
                new AirportApp.ClassLibrary.Entity.Domain.Company { Id = 1, Name = "Acme Airlines" },
                new AirportApp.ClassLibrary.Entity.Domain.Company { Id = 2, Name = "Contoso Air" },
                new AirportApp.ClassLibrary.Entity.Domain.Company { Id = 3, Name = "SkyLink Airways" });
            dataBaseContext.SaveChanges();
        }

        // Seed Airports
        if (!dataBaseContext.Airports.Any())
        {
            dataBaseContext.Airports.AddRange(
                new AirportApp.ClassLibrary.Entity.Domain.Airport { Id = 1, AirportCode = "LAX", City = "Los Angeles" },
                new AirportApp.ClassLibrary.Entity.Domain.Airport { Id = 2, AirportCode = "JFK", City = "New York" },
                new AirportApp.ClassLibrary.Entity.Domain.Airport { Id = 3, AirportCode = "ORD", City = "Chicago" });
            dataBaseContext.SaveChanges();
        }

        // Seed Gates
        if (!dataBaseContext.Gates.Any())
        {
            var airport1 = dataBaseContext.Airports.FirstOrDefault(airport => airport.Id == 1);
            var airport2 = dataBaseContext.Airports.FirstOrDefault(airport => airport.Id == 2);
            var airport3 = dataBaseContext.Airports.FirstOrDefault(airport => airport.Id == 3);
            if (airport1 != null && airport2 != null && airport3 != null)
            {
                dataBaseContext.Gates.AddRange(
                    new AirportApp.ClassLibrary.Entity.Domain.Gate { Id = 1, GateName = "A1", Airport = airport1 },
                    new AirportApp.ClassLibrary.Entity.Domain.Gate { Id = 2, GateName = "B2", Airport = airport2 },
                    new AirportApp.ClassLibrary.Entity.Domain.Gate { Id = 3, GateName = "C3", Airport = airport3 },
                    new AirportApp.ClassLibrary.Entity.Domain.Gate { Id = 4, GateName = "D4", Airport = airport1 });
                dataBaseContext.SaveChanges();
            }
        }

        // Seed Routes
        if (!dataBaseContext.Routes.Any())
        {
            var company1 = dataBaseContext.Companies.FirstOrDefault(company => company.Id == 1);
            var company2 = dataBaseContext.Companies.FirstOrDefault(company => company.Id == 2);
            var company3 = dataBaseContext.Companies.FirstOrDefault(company => company.Id == 3);
            var airport1 = dataBaseContext.Airports.FirstOrDefault(airport => airport.Id == 1);
            var airport2 = dataBaseContext.Airports.FirstOrDefault(airport => airport.Id == 2);
            var airport3 = dataBaseContext.Airports.FirstOrDefault(airport => airport.Id == 3);
            if (company1 != null && company2 != null && company3 != null && airport1 != null && airport2 != null && airport3 != null)
            {
                dataBaseContext.Routes.AddRange(
                    new AirportApp.ClassLibrary.Entity.Domain.Route { Id = 1, Company = company1, Airport = airport1, RouteType = "Departure", DepartureTime = new DateTime(2026, 5, 4, 8, 0, 0), ArrivalTime = new DateTime(2026, 5, 4, 11, 0, 0), Capacity = 180 },
                    new AirportApp.ClassLibrary.Entity.Domain.Route { Id = 2, Company = company2, Airport = airport2, RouteType = "Arrival", DepartureTime = new DateTime(2026, 5, 5, 9, 30, 0), ArrivalTime = new DateTime(2026, 5, 5, 12, 45, 0), Capacity = 150 },
                    new AirportApp.ClassLibrary.Entity.Domain.Route { Id = 3, Company = company3, Airport = airport3, RouteType = "Departure", DepartureTime = new DateTime(2026, 5, 6, 7, 15, 0), ArrivalTime = new DateTime(2026, 5, 6, 10, 5, 0), Capacity = 220 },
                    new AirportApp.ClassLibrary.Entity.Domain.Route { Id = 4, Company = company1, Airport = airport1, RouteType = "Arrival", DepartureTime = new DateTime(2026, 5, 7, 14, 0, 0), ArrivalTime = new DateTime(2026, 5, 7, 17, 10, 0), Capacity = 160 },
                    new AirportApp.ClassLibrary.Entity.Domain.Route { Id = 5, Company = company2, Airport = airport3, RouteType = "Arrival", DepartureTime = new DateTime(2026, 5, 20, 13, 0, 0), ArrivalTime = new DateTime(2026, 5, 20, 15, 12, 0), Capacity = 50 });
                dataBaseContext.SaveChanges();
            }
        }

        // Seed Flights
        if (!dataBaseContext.Flights.Any())
        {
            var route1 = dataBaseContext.Routes.FirstOrDefault(route => route.Id == 1);
            var route2 = dataBaseContext.Routes.FirstOrDefault(route => route.Id == 2);
            var route3 = dataBaseContext.Routes.FirstOrDefault(route => route.Id == 3);
            var route4 = dataBaseContext.Routes.FirstOrDefault(route => route.Id == 4);
            var route5 = dataBaseContext.Routes.FirstOrDefault(route => route.Id == 5);
            var gate1 = dataBaseContext.Gates.FirstOrDefault(gate => gate.Id == 1);
            var gate2 = dataBaseContext.Gates.FirstOrDefault(gate => gate.Id == 2);
            var gate3 = dataBaseContext.Gates.FirstOrDefault(gate => gate.Id == 3);
            var gate4 = dataBaseContext.Gates.FirstOrDefault(gate => gate.Id == 4);
            if (route1 != null && route2 != null && route3 != null && route4 != null && route5 != null &&
                gate1 != null && gate2 != null && gate3 != null && gate4 != null)
            {
                dataBaseContext.Flights.AddRange(
                    new AirportApp.ClassLibrary.Entity.Domain.Flight(1, route1, gate1, new DateTime(2026, 5, 4, 8, 0, 0), "AC100"),
                    new AirportApp.ClassLibrary.Entity.Domain.Flight(2, route2, gate2, new DateTime(2026, 5, 5, 9, 30, 0), "CT200"),
                    new AirportApp.ClassLibrary.Entity.Domain.Flight(3, route3, gate3, new DateTime(2026, 5, 6, 7, 15, 0), "SK300"),
                    new AirportApp.ClassLibrary.Entity.Domain.Flight(4, route4, gate4, new DateTime(2026, 5, 7, 14, 0, 0), "AC400"),
                    new AirportApp.ClassLibrary.Entity.Domain.Flight(5, route5, gate4, new DateTime(2026, 5, 20, 13, 0, 0), "CT500"));
                dataBaseContext.SaveChanges();
            }
        }

        // Seed AddOns
        if (!dataBaseContext.AddOns.Any())
        {
            dataBaseContext.AddOns.AddRange(
                new AddOn { Id = 1, Name = "Extra Baggage", BasePrice = 30f },
                new AddOn { Id = 2, Name = "Priority Boarding", BasePrice = 15f },
                new AddOn { Id = 3, Name = "Seat Selection", BasePrice = 12f },
                new AddOn { Id = 4, Name = "Lounge Access", BasePrice = 45f });
            dataBaseContext.SaveChanges();
        }

        // Seed Memberships
        if (!dataBaseContext.Memberships.Any())
        {
            dataBaseContext.Memberships.AddRange(
                new AirportApp.ClassLibrary.Entity.Domain.Membership { Id = 1, Name = "Silver", FlightDiscountPercentage = 5f },
                new AirportApp.ClassLibrary.Entity.Domain.Membership { Id = 2, Name = "Gold", FlightDiscountPercentage = 15f },
                new AirportApp.ClassLibrary.Entity.Domain.Membership { Id = 3, Name = "Platinum", FlightDiscountPercentage = 25f });
            dataBaseContext.SaveChanges();
        }

        // Seed Chats
        if (!dataBaseContext.Chats.Any())
        {
            var user101 = dataBaseContext.Users.FirstOrDefault(user => user.Id == 101);
            var user102 = dataBaseContext.Users.FirstOrDefault(user => user.Id == 102);
            var user103 = dataBaseContext.Users.FirstOrDefault(user => user.Id == 103);
            if (user101 != null && user102 != null && user103 != null)
            {
                dataBaseContext.Chats.AddRange(
                    new Chat { Id = 1, User = user101, Status = ChatStatus.Active },
                    new Chat { Id = 2, User = user102, Status = ChatStatus.Active },
                    new Chat { Id = 3, User = user103, Status = ChatStatus.Active });
                dataBaseContext.SaveChanges();
            }
        }

        // Seed Messages
        if (!dataBaseContext.Messages.Any())
        {
            var chat1 = dataBaseContext.Chats.FirstOrDefault(chat => chat.Id == 1);
            var chat2 = dataBaseContext.Chats.FirstOrDefault(chat => chat.Id == 2);
            var chat3 = dataBaseContext.Chats.FirstOrDefault(chat => chat.Id == 3);
            var sender1 = dataBaseContext.Users.FirstOrDefault(user => user.Id == 101);
            var sender2 = dataBaseContext.Users.FirstOrDefault(user => user.Id == 102);
            var sender3 = dataBaseContext.Users.FirstOrDefault(user => user.Id == 103);
            if (chat1 != null && chat2 != null && chat3 != null && sender1 != null && sender2 != null && sender3 != null)
            {
                dataBaseContext.Messages.AddRange(
                    new Message { Id = 1, Chat = chat1, Sender = sender1, Text = "Hello! I need help with flights.", Timestamp = new DateTimeOffset(new DateTime(2026, 5, 4, 9, 0, 0)) },
                    new Message { Id = 2, Chat = chat2, Sender = sender2, Text = "Hi, I have a question about membership.", Timestamp = new DateTimeOffset(new DateTime(2026, 5, 4, 9, 5, 0)) },
                    new Message { Id = 3, Chat = chat3, Sender = sender3, Text = "Hello, I need support with my booking.", Timestamp = new DateTimeOffset(new DateTime(2026, 5, 4, 9, 10, 0)) });
                dataBaseContext.SaveChanges();
            }
        }

        // Seed FAQs
        if (!dataBaseContext.Faqs.Any())
        {
            dataBaseContext.Faqs.AddRange(
                new FAQEntry { Id = 1, Question = "How do I reset my password?", Answer = "Click on the 'Forgot Password' link on the login page.", Category = FAQCategoryEnum.Parking, ViewCount = 150, HelpfulVotesCount = 120, NotHelpfulVotesCount = 5 },
                new FAQEntry { Id = 2, Question = "What is the baggage allowance?", Answer = "Standard baggage allowance is 2 checked bags. See add-ons for additional bags.", Category = FAQCategoryEnum.Baggage, ViewCount = 200, HelpfulVotesCount = 180, NotHelpfulVotesCount = 10 },
                new FAQEntry { Id = 3, Question = "Can I change my flight?", Answer = "Yes, flight changes can be made up to 24 hours before departure, subject to availability.", Category = FAQCategoryEnum.Facilities, ViewCount = 180, HelpfulVotesCount = 160, NotHelpfulVotesCount = 8 },
                new FAQEntry { Id = 4, Question = "Which payment methods are accepted?", Answer = "We accept major credit cards and selected digital wallets.", Category = FAQCategoryEnum.Facilities, ViewCount = 95, HelpfulVotesCount = 80, NotHelpfulVotesCount = 2 },
                new FAQEntry { Id = 5, Question = "How do I contact support?", Answer = "Use the chat assistant or submit a support ticket from your account.", Category = FAQCategoryEnum.Facilities, ViewCount = 120, HelpfulVotesCount = 110, NotHelpfulVotesCount = 3 });
            dataBaseContext.SaveChanges();
        }

        // Seed ComplaintTicketCategories
        if (!dataBaseContext.TicketCategories.Any())
        {
            dataBaseContext.TicketCategories.AddRange(
                new ComplaintTicketCategory { Id = 1, CategoryName = "Booking Issues", CategoryUrgencyLevel = ComplaintTicketUrgencyLevelEnum.HIGH },
                new ComplaintTicketCategory { Id = 2, CategoryName = "General Inquiry", CategoryUrgencyLevel = ComplaintTicketUrgencyLevelEnum.LOW },
                new ComplaintTicketCategory { Id = 3, CategoryName = "Payment Problems", CategoryUrgencyLevel = ComplaintTicketUrgencyLevelEnum.HIGH });
            dataBaseContext.SaveChanges();
        }
    }

    public void Dispose()
    {
        client.Dispose();
        factory.Dispose();
    }
}