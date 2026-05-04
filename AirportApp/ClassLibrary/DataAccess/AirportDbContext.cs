using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.Entity.Domain.Chats;
using AirportApp.ClassLibrary.Entity.Domain.Employee;
using AirportApp.ClassLibrary.Entity.Domain.Faq;
using AirportApp.ClassLibrary.Entity.Domain.Faq.Bot;
using AirportApp.ClassLibrary.Entity.Domain.Review;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Domain.Message;
namespace AirportApp.ClassLibrary.DataAccess
{
    public class AirportDbContext : DbContext
    {
        public AirportDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Chat> chats { get; set; }
        public DbSet<Employee> employees { get; set; }
        public DbSet<FAQEntry> faqs { get; set; }
        public DbSet<Review> reviews { get; set; }
        public DbSet<Ticket> tickets { get; set; }
        public DbSet<AddOn> addOns { get; set; }
        public DbSet<Airport> airports { get; set; }
        public DbSet<Customer> customers { get; set; }
        public DbSet<Flight> flights { get; set; }
        public DbSet<FlightTicket> flightTickets { get; set; }
        public DbSet<Membership> memberships { get; set; }
        public DbSet<MembershipAddonDiscount> membershipAddonDiscounts { get; set; }
        public DbSet<Message> messages { get; set; }
        public DbSet<TicketCategory> ticketCategories { get; set; }
        public DbSet<TicketSubcategory> ticketSubcategories { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<Company> companies { get; set; }
        public DbSet<Gate> gates { get; set; }
        public DbSet<Route> routes { get; set; }
        public DbSet<FAQNodeEntity> faqNodes { get; set; }
        public DbSet<FAQOptionEntity> faqOptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FAQNodeEntity>(b =>
            {
                b.ToTable("FAQNode");
                b.HasKey(e => e.NodeId);
                b.Property(e => e.NodeId).HasColumnName("node_id");
                b.Property(e => e.QuestionText).HasColumnName("question_text");
                b.Property(e => e.IsFinalAnswer).HasColumnName("is_final_answer");
                b.HasMany(e => e.Options)
                    .WithOne()
                    .HasForeignKey(o => o.NodeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<FAQOptionEntity>(b =>
            {
                b.ToTable("FAQOption");
                b.HasKey(e => new { e.NodeId, e.Label });
                b.Property(e => e.NodeId).HasColumnName("node_id");
                b.Property(e => e.Label).HasColumnName("label");
                b.Property(e => e.NextOptionId).HasColumnName("next_option_id");
            });

            modelBuilder.Entity<Airport>()
                .HasMany<Gate>(a => a.Gates)
                .WithOne(g => g.Airport)
                .HasForeignKey(g => g.AirportId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Route>()
                .HasOne(r => r.Company)
                .WithMany()
                .HasForeignKey(r => r.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Route>()
                .HasOne(r => r.Airport)
                .WithMany()
                .HasForeignKey(r => r.AirportId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Flight>()
                .HasOne(f => f.Route)
                .WithMany()
                .HasForeignKey(f => f.RouteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Flight>()
                .HasOne(f => f.Gate)
                .WithMany()
                .HasForeignKey(f => f.GateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Membership>()
                .HasMany(m => m.AddonDiscounts)
                .WithOne(d => d.Membership)
                .HasForeignKey(d => d.MembershipId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MembershipAddonDiscount>(b =>
            {
                b.HasKey(m => new { m.MembershipId, m.AddOnId });
                b.ToTable("Membership_Addon_Discounts");
                b.Property(m => m.DiscountPercentage).HasColumnName("Discount_Percentage");
                b.HasOne(m => m.AddOn)
                    .WithMany()
                    .HasForeignKey(m => m.AddOnId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Customer>()
                .HasOne(c => c.Membership)
                .WithMany()
                .HasForeignKey(c => c.MembershipId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Chat>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Chat)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .Property(m => m.SenderUserId)
                .HasColumnName("Sender_User_Id");

            modelBuilder.Entity<Message>()
                .Property(m => m.SenderEmployeeId)
                .HasColumnName("Sender_Employee_Id");

            modelBuilder.Entity<Message>()
                .HasOne(m => m.SenderUser)
                .WithMany()
                .HasForeignKey(m => m.SenderUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.SenderEmployee)
                .WithMany()
                .HasForeignKey(m => m.SenderEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Creator)
                .WithMany()
                .HasForeignKey(t => t.CreatorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Category)
                .WithMany()
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Subcategory)
                .WithMany()
                .HasForeignKey(t => t.SubcategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure many-to-many between FlightTicket and AddOn via explicit join table
            modelBuilder.Entity<FlightTicket>()
                .HasMany(ft => ft.SelectedAddOns)
                .WithMany(a => a.Tickets)
                .UsingEntity<Dictionary<string, object>>(
                    "FlightTicket_AddOns",
                    right => right.HasOne<AddOn>().WithMany().HasForeignKey("AddOn_Id").HasConstraintName("FK_FlightTicketAddOns_AddOn"),
                    left => left.HasOne<FlightTicket>().WithMany().HasForeignKey("Ticket_Id").HasConstraintName("FK_FlightTicketAddOns_FlightTicket"),
                    je =>
                    {
                        je.HasKey("Ticket_Id", "AddOn_Id");
                        je.ToTable("FlightTicket_AddOns");
                    }
                );

            // Seed some basic decision-tree data for FAQs
            modelBuilder.Entity<FAQNodeEntity>().HasData(
                new FAQNodeEntity { NodeId = 1, QuestionText = "Welcome! How can I help you today?", IsFinalAnswer = false },
                new FAQNodeEntity { NodeId = 2, QuestionText = "Flights information: You can search and book flights.", IsFinalAnswer = true },
                new FAQNodeEntity { NodeId = 3, QuestionText = "Membership information: View plans and discounts.", IsFinalAnswer = true },
                new FAQNodeEntity { NodeId = 4, QuestionText = "Baggage information: Learn what is included and what costs extra.", IsFinalAnswer = true },
                new FAQNodeEntity { NodeId = 5, QuestionText = "Payments information: Find out which payment methods are accepted.", IsFinalAnswer = true },
                new FAQNodeEntity { NodeId = 6, QuestionText = "Support information: Contact our team for help with bookings or accounts.", IsFinalAnswer = true }
            );

            modelBuilder.Entity<FAQOptionEntity>().HasData(
                new FAQOptionEntity { NodeId = 1, Label = "Flights", NextOptionId = 2 },
                new FAQOptionEntity { NodeId = 1, Label = "Memberships", NextOptionId = 3 },
                new FAQOptionEntity { NodeId = 1, Label = "Baggage", NextOptionId = 4 },
                new FAQOptionEntity { NodeId = 1, Label = "Payments", NextOptionId = 5 },
                new FAQOptionEntity { NodeId = 1, Label = "Contact support", NextOptionId = 6 }
            );

            // Seed core domain data used by EF-backed repositories
            modelBuilder.Entity<Company>().HasData(
                new Company { Id = 1, Name = "Acme Airlines" },
                new Company { Id = 2, Name = "Contoso Air" },
                new Company { Id = 3, Name = "SkyLink Airways" }
            );

            modelBuilder.Entity<Airport>().HasData(
                new Airport { Id = 1, AirportCode = "LAX", City = "Los Angeles" },
                new Airport { Id = 2, AirportCode = "JFK", City = "New York" },
                new Airport { Id = 3, AirportCode = "ORD", City = "Chicago" }
            );

            modelBuilder.Entity<Gate>().HasData(
                new Gate { Id = 1, GateName = "A1", AirportId = 1 },
                new Gate { Id = 2, GateName = "B2", AirportId = 2 },
                new Gate { Id = 3, GateName = "C3", AirportId = 3 },
                new Gate { Id = 4, GateName = "D4", AirportId = 1 }
            );

            modelBuilder.Entity<Route>().HasData(
                new Route { Id = 1, CompanyId = 1, AirportId = 1, RouteType = "Departure", DepartureTime = new DateTime(2026,5,4,8,0,0), ArrivalTime = new DateTime(2026,5,4,11,0,0), Capacity = 180 },
                new Route { Id = 2, CompanyId = 2, AirportId = 2, RouteType = "Arrival", DepartureTime = new DateTime(2026,5,5,9,30,0), ArrivalTime = new DateTime(2026,5,5,12,45,0), Capacity = 150 },
                new Route { Id = 3, CompanyId = 3, AirportId = 3, RouteType = "Departure", DepartureTime = new DateTime(2026,5,6,7,15,0), ArrivalTime = new DateTime(2026,5,6,10,5,0), Capacity = 220 },
                new Route { Id = 4, CompanyId = 1, AirportId = 1, RouteType = "Arrival", DepartureTime = new DateTime(2026,5,7,14,0,0), ArrivalTime = new DateTime(2026,5,7,17,10,0), Capacity = 160 }
            );

            modelBuilder.Entity<Flight>().HasData(
                new Flight { Id = 1, RouteId = 1, GateId = 1, Date = new DateTime(2026,5,4,8,0,0), FlightNumber = "AC100" },
                new Flight { Id = 2, RouteId = 2, GateId = 2, Date = new DateTime(2026,5,5,9,30,0), FlightNumber = "CT200" },
                new Flight { Id = 3, RouteId = 3, GateId = 3, Date = new DateTime(2026,5,6,7,15,0), FlightNumber = "SK300" },
                new Flight { Id = 4, RouteId = 4, GateId = 4, Date = new DateTime(2026,5,7,14,0,0), FlightNumber = "AC400" }
            );

            modelBuilder.Entity<AddOn>().HasData(
                new AddOn { Id = 1, Name = "Extra Baggage", BasePrice = 30f },
                new AddOn { Id = 2, Name = "Priority Boarding", BasePrice = 15f },
                new AddOn { Id = 3, Name = "Seat Selection", BasePrice = 12f },
                new AddOn { Id = 4, Name = "Lounge Access", BasePrice = 45f }
            );

            modelBuilder.Entity<Membership>().HasData(
                new Membership { Id = 1, Name = "Silver", FlightDiscountPercentage = 5f },
                new Membership { Id = 2, Name = "Gold", FlightDiscountPercentage = 15f },
                new Membership { Id = 3, Name = "Platinum", FlightDiscountPercentage = 25f }
            );

            modelBuilder.Entity<Customer>().HasData(
                new Customer { Id = 1, Email = "user1@example.com", Phone = "", Username = "user1", PasswordHash = "passhash1", MembershipId = 1 },
                new Customer { Id = 2, Email = "user2@example.com", Phone = "", Username = "user2", PasswordHash = "passhash2", MembershipId = 2 },
                new Customer { Id = 3, Email = "user3@example.com", Phone = "", Username = "user3", PasswordHash = "passhash3", MembershipId = 3 }
            );

            modelBuilder.Entity<Employee>().HasData(
                new Employee { Id = 1, FullName = "Alice Smith", EmailAddress = "alice@acme.com", AssignedDepartment = EmployeeDepartment.MEDICAL },
                new Employee { Id = 2, FullName = "Bob Johnson", EmailAddress = "bob@contoso.com", AssignedDepartment = EmployeeDepartment.LEGAL },
                new Employee { Id = 3, FullName = "Clara Davis", EmailAddress = "clara@skylink.com", AssignedDepartment = EmployeeDepartment.HR },
                new Employee { Id = 4, FullName = "Daniel Green", EmailAddress = "daniel@acme.com", AssignedDepartment = EmployeeDepartment.CASHBACK }
            );

            modelBuilder.Entity<FlightTicket>().HasData(
                new FlightTicket { Id = 1, UserId = 1, FlightId = 1, Seat = "12A", Price = 199f, Status = "Booked", PassengerFirstName = "John", PassengerLastName = "Doe", PassengerEmail = "johndoe@example.com", PassengerPhone = "" },
                new FlightTicket { Id = 2, UserId = 2, FlightId = 2, Seat = "14C", Price = 149f, Status = "Booked", PassengerFirstName = "Jane", PassengerLastName = "Roe", PassengerEmail = "janeroe@example.com", PassengerPhone = "" },
                new FlightTicket { Id = 3, UserId = 3, FlightId = 3, Seat = "8F", Price = 249f, Status = "Booked", PassengerFirstName = "Mia", PassengerLastName = "Lane", PassengerEmail = "mialane@example.com", PassengerPhone = "" },
                new FlightTicket { Id = 4, UserId = 1, FlightId = 4, Seat = "5B", Price = 179f, Status = "Booked", PassengerFirstName = "Liam", PassengerLastName = "Stone", PassengerEmail = "liamstone@example.com", PassengerPhone = "" }
            );

            modelBuilder.Entity<Chat>().HasData(
                new Chat { Id = 1, UserId = 1, Status = ChatStatus.Active },
                new Chat { Id = 2, UserId = 2, Status = ChatStatus.Active },
                new Chat { Id = 3, UserId = 3, Status = ChatStatus.Active }
            );

            modelBuilder.Entity<Message>().HasData(
                new Message { Id = 1, ChatId = 1, Text = "Hello, I need help finding flights.", Timestamp = new DateTimeOffset(2026, 5, 4, 9, 0, 0, TimeSpan.Zero), SenderUserId = 1 },
                new Message { Id = 2, ChatId = 2, Text = "Is there a baggage allowance?", Timestamp = new DateTimeOffset(2026, 5, 4, 9, 5, 0, TimeSpan.Zero), SenderUserId = 2 },
                new Message { Id = 3, ChatId = 3, Text = "Can I change my booking after checkout?", Timestamp = new DateTimeOffset(2026, 5, 4, 9, 8, 0, TimeSpan.Zero), SenderUserId = 3 },
                new Message { Id = 4, ChatId = 3, Text = "Yes, you can request a change from your account page.", Timestamp = new DateTimeOffset(2026, 5, 4, 9, 9, 0, TimeSpan.Zero), SenderEmployeeId = 1 }
            );

            // Seed User data
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, FullName = "Alice Bot", EmailAddress = "alice@bot.com" },
                new User { Id = 2, FullName = "Bob Chat", EmailAddress = "bob@chat.com" },
                new User { Id = 3, FullName = "Mia Passenger", EmailAddress = "mia@example.com" }
            );

            // Seed TicketCategory data
            modelBuilder.Entity<TicketCategory>().HasData(
                new TicketCategory { Id = 1, CategoryName = "Booking Issues", CategoryUrgencyLevel = TicketUrgencyLevelEnum.HIGH },
                new TicketCategory { Id = 2, CategoryName = "General Inquiry", CategoryUrgencyLevel = TicketUrgencyLevelEnum.LOW },
                new TicketCategory { Id = 3, CategoryName = "Payment Problems", CategoryUrgencyLevel = TicketUrgencyLevelEnum.HIGH }
            );

            // Seed TicketSubcategory data
            modelBuilder.Entity<TicketSubcategory>().HasData(
                new TicketSubcategory { Id = 1, SubcategoryName = "Booking Error", SubcategoryExternalReferenceId = 101, ParentCategoryId = 1 },
                new TicketSubcategory { Id = 2, SubcategoryName = "Cancellation", SubcategoryExternalReferenceId = 102, ParentCategoryId = 1 },
                new TicketSubcategory { Id = 3, SubcategoryName = "Flight Info", SubcategoryExternalReferenceId = 201, ParentCategoryId = 2 },
                new TicketSubcategory { Id = 4, SubcategoryName = "Card Declined", SubcategoryExternalReferenceId = 301, ParentCategoryId = 3 },
                new TicketSubcategory { Id = 5, SubcategoryName = "Refund Status", SubcategoryExternalReferenceId = 302, ParentCategoryId = 3 }
            );

            // Seed Ticket data (depends on User, TicketCategory, TicketSubcategory)
            modelBuilder.Entity<Ticket>().HasData(
                new Ticket { Id = 1, Subject = "Cannot book flight", Description = "System error when attempting to book", CreationTimestamp = new DateTime(2026, 5, 4, 10, 0, 0), UrgencyLevel = TicketUrgencyLevelEnum.HIGH, CurrentStatus = TicketStatusEnum.RESOLVED, CreatorId = 1, CategoryId = 1, SubcategoryId = 1 },
                new Ticket { Id = 2, Subject = "Question about baggage", Description = "What is the baggage allowance?", CreationTimestamp = new DateTime(2026, 5, 4, 11, 0, 0), UrgencyLevel = TicketUrgencyLevelEnum.LOW, CurrentStatus = TicketStatusEnum.OPEN, CreatorId = 2, CategoryId = 2, SubcategoryId = 3 },
                new Ticket { Id = 3, Subject = "Payment failed", Description = "My card was declined during checkout", CreationTimestamp = new DateTime(2026, 5, 4, 12, 0, 0), UrgencyLevel = TicketUrgencyLevelEnum.HIGH, CurrentStatus = TicketStatusEnum.OPEN, CreatorId = 3, CategoryId = 3, SubcategoryId = 4 },
                new Ticket { Id = 4, Subject = "Refund pending", Description = "How long until my refund appears?", CreationTimestamp = new DateTime(2026, 5, 4, 12, 30, 0), UrgencyLevel = TicketUrgencyLevelEnum.MEDIUM, CurrentStatus = TicketStatusEnum.RESOLVED, CreatorId = 1, CategoryId = 3, SubcategoryId = 5 }
            );

            // Seed FAQEntry data
            modelBuilder.Entity<FAQEntry>().HasData(
                new FAQEntry { Id = 1, Question = "How do I reset my password?", Answer = "Click on the 'Forgot Password' link on the login page.", Category = FAQCategoryEnum.Parking, ViewCount = 150, HelpfulVotesCount = 120, NotHelpfulVotesCount = 5 },
                new FAQEntry { Id = 2, Question = "What is the baggage allowance?", Answer = "Standard baggage allowance is 2 checked bags. See add-ons for additional bags.", Category = FAQCategoryEnum.Baggage, ViewCount = 200, HelpfulVotesCount = 180, NotHelpfulVotesCount = 10 },
                new FAQEntry { Id = 3, Question = "Can I change my flight?", Answer = "Yes, flight changes can be made up to 24 hours before departure, subject to availability.", Category = FAQCategoryEnum.Facilities, ViewCount = 180, HelpfulVotesCount = 160, NotHelpfulVotesCount = 8 },
                new FAQEntry { Id = 4, Question = "Which payment methods are accepted?", Answer = "We accept major credit cards and selected digital wallets.", Category = FAQCategoryEnum.Facilities, ViewCount = 95, HelpfulVotesCount = 80, NotHelpfulVotesCount = 2 },
                new FAQEntry { Id = 5, Question = "How do I contact support?", Answer = "Use the chat assistant or submit a support ticket from your account.", Category = FAQCategoryEnum.Facilities, ViewCount = 120, HelpfulVotesCount = 110, NotHelpfulVotesCount = 3 }
            );

            // Seed MembershipAddonDiscount data (join table between Membership and AddOn)
            modelBuilder.Entity<MembershipAddonDiscount>().HasData(
                new MembershipAddonDiscount { MembershipId = 1, AddOnId = 1, DiscountPercentage = 10f },
                new MembershipAddonDiscount { MembershipId = 1, AddOnId = 2, DiscountPercentage = 10f },
                new MembershipAddonDiscount { MembershipId = 2, AddOnId = 1, DiscountPercentage = 20f },
                new MembershipAddonDiscount { MembershipId = 2, AddOnId = 2, DiscountPercentage = 20f },
                new MembershipAddonDiscount { MembershipId = 3, AddOnId = 1, DiscountPercentage = 30f },
                new MembershipAddonDiscount { MembershipId = 3, AddOnId = 2, DiscountPercentage = 30f },
                new MembershipAddonDiscount { MembershipId = 3, AddOnId = 3, DiscountPercentage = 30f },
                new MembershipAddonDiscount { MembershipId = 3, AddOnId = 4, DiscountPercentage = 35f }
            );
            }
    }
}
