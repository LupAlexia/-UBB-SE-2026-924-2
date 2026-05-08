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

        public DbSet<Chat> Chats { get; set; }
        public DbSet<Sender> Senders { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<FAQEntry> Faqs { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ComplaintTicket> Tickets { get; set; }
        public DbSet<AddOn> AddOns { get; set; }
        public DbSet<Airport> Airports { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<FlightTicket> FlightTickets { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<MembershipAddonDiscount> MembershipAddonDiscounts { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<BotMessage> BotMessages { get; set; }
        public DbSet<ComplaintTicketCategory> TicketCategories { get; set; }
        public DbSet<ComplaintTicketSubcategory> TicketSubcategories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Gate> Gates { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<FAQNode> FaqNodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Sender inheritance (TPH - Table Per Hierarchy)
            modelBuilder.Entity<Sender>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue<User>("User")
                .HasValue<Employee>("Employee");

            modelBuilder.Entity<User>()
                .ToTable("Users");

            modelBuilder.Entity<Employee>()
                .ToTable("Employees");

            modelBuilder.Entity<FAQNode>(b =>
            {
                b.ToTable("FAQNode");
                b.HasKey(e => e.FaqNodeId);
                b.Property(e => e.FaqNodeId).HasColumnName("node_id");
                b.Property(e => e.QuestionText).HasColumnName("question_text");
                b.Property(e => e.IsFinalAnswer).HasColumnName("is_final_answer");
                b.OwnsMany(e => e.Options, ob =>
                {
                    ob.ToTable("FAQOption");
                    ob.WithOwner().HasForeignKey("node_id");
                    ob.Property<int>("node_id");
                    ob.HasKey("node_id", nameof(FAQOption.label));
                    ob.Property(e => e.label).HasColumnName("label");
                    ob.Property(e => e.nextOptionId).HasColumnName("next_option_id");
                });
            });

            modelBuilder.Entity<Airport>()
                .HasMany<Gate>(a => a.Gates)
                .WithOne(g => g.Airport)
                .HasForeignKey("AirportId")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Route>()
                .HasOne(r => r.Company)
                .WithMany()
                .HasForeignKey("CompanyId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Route>()
                .HasOne(r => r.Airport)
                .WithMany()
                .HasForeignKey("AirportId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Flight>()
                .HasOne(f => f.Route)
                .WithMany()
                .HasForeignKey("RouteId")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Flight>()
                .HasOne(f => f.Gate)
                .WithMany()
                .HasForeignKey("GateId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Membership>()
                .HasMany(m => m.AddonDiscounts)
                .WithOne(d => d.Membership)
                .HasForeignKey("MembershipId")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MembershipAddonDiscount>(b =>
            {
                b.ToTable("Membership_Addon_Discounts");
                b.HasKey("MembershipId", "AddOnId");
                b.Property<int>("MembershipId").HasColumnName("Membership_Id");
                b.Property<int>("AddOnId").HasColumnName("AddOn_Id");
                b.Property(m => m.DiscountPercentage).HasColumnName("Discount_Percentage");
                b.HasOne(m => m.Membership)
                    .WithMany(m => m.AddonDiscounts)
                    .HasForeignKey("MembershipId")
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(m => m.AddOn)
                    .WithMany()
                    .HasForeignKey("AddOnId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Customer>()
                .HasOne(c => c.Membership)
                .WithMany()
                .HasForeignKey("MembershipId")
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Chat>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Chat)
                .WithMany(c => c.Messages)
                .HasForeignKey("ChatId") // Shadow FK
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey("SenderId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BotMessage>()
                .HasOne(m => m.Chat)
                .WithMany()
                .HasForeignKey("ChatId")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BotMessage>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey("SenderId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BotMessage>()
                .OwnsMany(m => m.FAQOptions, b =>
                {
                    b.ToTable("BotMessageFAQOptions");
                    b.WithOwner().HasForeignKey("Message_Id");
                    b.Property<int>("Message_Id");
                    b.HasKey("Message_Id", nameof(FAQOption.label), nameof(FAQOption.nextOptionId));
                    b.Property(p => p.label).HasColumnName("label");
                    b.Property(p => p.nextOptionId).HasColumnName("next_option_id");
                });

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ComplaintTicket>()
                .HasOne(t => t.Creator)
                .WithMany()
                .HasForeignKey("CreatorId")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ComplaintTicket>()
                .HasOne(t => t.Category)
                .WithMany()
                .HasForeignKey("CategoryId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ComplaintTicket>()
                .HasOne(t => t.Subcategory)
                .WithMany()
                .HasForeignKey("SubcategoryId")
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
                    });

            User user1 = new User { Id = 101, FullName = "Alice Bot", EmailAddress = "alice@bot.com" };
            User user2 = new User { Id = 102, FullName = "Bob Chat", EmailAddress = "bob@chat.com" };
            User user3 = new User { Id = 103, FullName = "Mia Passenger", EmailAddress = "mia@example.com" };

            FAQNode node1 = new FAQNode { FaqNodeId = 1, QuestionText = "Welcome! How can I help you today?", IsFinalAnswer = false };
            FAQNode node2 = new FAQNode { FaqNodeId = 2, QuestionText = "Flights information: You can search and book flights.", IsFinalAnswer = true };
            FAQNode node3 = new FAQNode { FaqNodeId = 3, QuestionText = "Membership information: View plans and discounts.", IsFinalAnswer = true };
            FAQNode node4 = new FAQNode { FaqNodeId = 4, QuestionText = "Baggage information: Learn what is included and what costs extra.", IsFinalAnswer = true };
            FAQNode node5 = new FAQNode { FaqNodeId = 5, QuestionText = "Payments information: Find out which payment methods are accepted.", IsFinalAnswer = true };
            FAQNode node6 = new FAQNode { FaqNodeId = 6, QuestionText = "Support information: Contact our team for help with bookings or accounts.", IsFinalAnswer = true };

            // Seed some basic decision-tree data for FAQs
            modelBuilder.Entity<FAQNode>().HasData(
                node1,
                node2,
                node3,
                node4,
                node5,
                node6);

            modelBuilder.Entity<FAQNode>().OwnsMany(n => n.Options).HasData(
                new { node_id = 1, label = "Flights", nextOptionId = 2 },
                new { node_id = 1, label = "Memberships", nextOptionId = 3 },
                new { node_id = 1, label = "Baggage", nextOptionId = 4 },
                new { node_id = 1, label = "Payments", nextOptionId = 5 },
                new { node_id = 1, label = "Contact support", nextOptionId = 6 });

            // Seed core domain data used by EF-backed repositories
            modelBuilder.Entity<Company>().HasData(
                new Company { Id = 1, Name = "Acme Airlines" },
                new Company { Id = 2, Name = "Contoso Air" },
                new Company { Id = 3, Name = "SkyLink Airways" });

            modelBuilder.Entity<Airport>().HasData(
                new Airport { Id = 1, AirportCode = "LAX", City = "Los Angeles" },
                new Airport { Id = 2, AirportCode = "JFK", City = "New York" },
                new Airport { Id = 3, AirportCode = "ORD", City = "Chicago" });

            modelBuilder.Entity<Gate>().HasData(
                new { Id = 1, GateName = "A1", AirportId = 1 },
                new { Id = 2, GateName = "B2", AirportId = 2 },
                new { Id = 3, GateName = "C3", AirportId = 3 },
                new { Id = 4, GateName = "D4", AirportId = 1 });

            modelBuilder.Entity<Route>().HasData(
                new { Id = 1, CompanyId = 1, AirportId = 1, RouteType = "Departure", DepartureTime = new DateTime(2026, 5, 4, 8, 0, 0), ArrivalTime = new DateTime(2026, 5, 4, 11, 0, 0), Capacity = 180 },
                new { Id = 2, CompanyId = 2, AirportId = 2, RouteType = "Arrival", DepartureTime = new DateTime(2026, 5, 5, 9, 30, 0), ArrivalTime = new DateTime(2026, 5, 5, 12, 45, 0), Capacity = 150 },
                new { Id = 3, CompanyId = 3, AirportId = 3, RouteType = "Departure", DepartureTime = new DateTime(2026, 5, 6, 7, 15, 0), ArrivalTime = new DateTime(2026, 5, 6, 10, 5, 0), Capacity = 220 },
                new { Id = 4, CompanyId = 1, AirportId = 1, RouteType = "Arrival", DepartureTime = new DateTime(2026, 5, 7, 14, 0, 0), ArrivalTime = new DateTime(2026, 5, 7, 17, 10, 0), Capacity = 160 },
                new { Id = 5, CompanyId = 2, AirportId = 3, RouteType = "Arrival", DepartureTime = new DateTime(2026, 5, 20, 13, 0, 0), ArrivalTime = new DateTime(2026, 5, 20, 15, 12, 0), Capacity = 50 });

            modelBuilder.Entity<Flight>().HasData(
                new { Id = 1, RouteId = 1, GateId = 1, Date = new DateTime(2026, 5, 4, 8, 0, 0), FlightNumber = "AC100" },
                new { Id = 2, RouteId = 2, GateId = 2, Date = new DateTime(2026, 5, 5, 9, 30, 0), FlightNumber = "CT200" },
                new { Id = 3, RouteId = 3, GateId = 3, Date = new DateTime(2026, 5, 6, 7, 15, 0), FlightNumber = "SK300" },
                new { Id = 4, RouteId = 4, GateId = 4, Date = new DateTime(2026, 5, 7, 14, 0, 0), FlightNumber = "AC400" },
                new { Id = 5, RouteId = 5, GateId = 4, Date = new DateTime(2026, 5, 20, 13, 0, 0), FlightNumber = "CT500" });

            modelBuilder.Entity<AddOn>().HasData(
                new AddOn { Id = 1, Name = "Extra Baggage", BasePrice = 30f },
                new AddOn { Id = 2, Name = "Priority Boarding", BasePrice = 15f },
                new AddOn { Id = 3, Name = "Seat Selection", BasePrice = 12f },
                new AddOn { Id = 4, Name = "Lounge Access", BasePrice = 45f });

            modelBuilder.Entity<Membership>().HasData(
                new Membership { Id = 1, Name = "Silver", FlightDiscountPercentage = 5f },
                new Membership { Id = 2, Name = "Gold", FlightDiscountPercentage = 15f },
                new Membership { Id = 3, Name = "Platinum", FlightDiscountPercentage = 25f });

            // Instantiate the hasher
            var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<Customer>();

            // Seed Sender data - Bot user for system messages
            modelBuilder.Entity<Sender>().HasData(
                new { Id = -1, FullName = "Carlos", EmailAddress = "customer-support@cloudspritzers.com", Discriminator = "Bot" });

            modelBuilder.Entity<Customer>().HasData(
                new { Id = 101, Email = "alice@bot.com", Phone = string.Empty, Username = "alice", PasswordHash = hasher.HashPassword(null!, "alice123"), MembershipId = 1 },
                new { Id = 102, Email = "bob@chat.com", Phone = string.Empty, Username = "bob", PasswordHash = hasher.HashPassword(null!, "bob123"), MembershipId = 2 },
                new { Id = 103, Email = "mia@example.com", Phone = string.Empty, Username = "mia", PasswordHash = hasher.HashPassword(null!, "mia123"), MembershipId = 3 });

            modelBuilder.Entity<Employee>().HasData(
                new Employee { Id = 1, FullName = "Alice Smith", EmailAddress = "alice@acme.com", AssignedDepartment = EmployeeDepartment.MEDICAL },
                new Employee { Id = 2, FullName = "Bob Johnson", EmailAddress = "bob@contoso.com", AssignedDepartment = EmployeeDepartment.LEGAL },
                new Employee { Id = 3, FullName = "Clara Davis", EmailAddress = "clara@skylink.com", AssignedDepartment = EmployeeDepartment.HR },
                new Employee { Id = 4, FullName = "Daniel Green", EmailAddress = "daniel@acme.com", AssignedDepartment = EmployeeDepartment.CASHBACK });

            modelBuilder.Entity<FlightTicket>().HasData(
                new { Id = 1, UserId = 101, FlightId = 1, Seat = "12A", Price = 199f, Status = "Booked", PassengerFirstName = "John", PassengerLastName = "Doe", PassengerEmail = "johndoe@example.com", PassengerPhone = string.Empty },
                new { Id = 2, UserId = 102, FlightId = 2, Seat = "14C", Price = 149f, Status = "Booked", PassengerFirstName = "Jane", PassengerLastName = "Roe", PassengerEmail = "janeroe@example.com", PassengerPhone = string.Empty },
                new { Id = 3, UserId = 103, FlightId = 3, Seat = "8F", Price = 249f, Status = "Booked", PassengerFirstName = "Mia", PassengerLastName = "Lane", PassengerEmail = "mialane@example.com", PassengerPhone = string.Empty },
                new { Id = 4, UserId = 101, FlightId = 4, Seat = "5B", Price = 179f, Status = "Booked", PassengerFirstName = "Liam", PassengerLastName = "Stone", PassengerEmail = "liamstone@example.com", PassengerPhone = string.Empty });

            modelBuilder.Entity<Chat>().HasData(
                new { Id = 1, UserId = 101, Status = ChatStatus.Active },
                new { Id = 2, UserId = 102, Status = ChatStatus.Active },
                new { Id = 3, UserId = 103, Status = ChatStatus.Active });

            // Seed BotMessage data with FAQOptions (owned collection)
            modelBuilder.Entity<BotMessage>().HasData(
                new { Id = 1, ChatId = 1, SenderId = -1, Text = "Welcome! How can I help you today?", Timestamp = new DateTimeOffset(new DateTime(2026, 5, 4, 10, 0, 0)) },
                new { Id = 2, ChatId = 2, SenderId = -1, Text = "What would you like to know about?", Timestamp = new DateTimeOffset(new DateTime(2026, 5, 4, 11, 0, 0)) },
                new { Id = 3, ChatId = 3, SenderId = -1, Text = "How can I assist you?", Timestamp = new DateTimeOffset(new DateTime(2026, 5, 4, 12, 0, 0)) });

            modelBuilder.Entity<BotMessage>()
                .OwnsMany(m => m.FAQOptions).HasData(
                    new { Message_Id = 1, label = "Flights", nextOptionId = 2 },
                    new { Message_Id = 1, label = "Bookings", nextOptionId = 3 },
                    new { Message_Id = 1, label = "Baggage", nextOptionId = 4 },
                    new { Message_Id = 2, label = "Payment", nextOptionId = 5 },
                    new { Message_Id = 2, label = "Refund", nextOptionId = 6 },
                    new { Message_Id = 3, label = "Account", nextOptionId = 7 },
                    new { Message_Id = 3, label = "Support", nextOptionId = 8 });

            // Message seed data removed - shadow FKs cannot be seeded with HasData()
            // Seed messages programmatically after context initialization or use a separate migration

            // Seed User data
            modelBuilder.Entity<User>().HasData(
                user1,
                user2,
                user3);

            // Seed TicketCategory data
            modelBuilder.Entity<ComplaintTicketCategory>().HasData(
                new ComplaintTicketCategory { Id = 1, CategoryName = "Booking Issues", CategoryUrgencyLevel = ComplaintTicketUrgencyLevelEnum.HIGH },
                new ComplaintTicketCategory { Id = 2, CategoryName = "General Inquiry", CategoryUrgencyLevel = ComplaintTicketUrgencyLevelEnum.LOW },
                new ComplaintTicketCategory { Id = 3, CategoryName = "Payment Problems", CategoryUrgencyLevel = ComplaintTicketUrgencyLevelEnum.HIGH });

            // Seed TicketSubcategory data
            modelBuilder.Entity<ComplaintTicketSubcategory>().HasData(
                new { Id = 1, SubcategoryName = "Booking Error", SubcategoryExternalReferenceId = 101, ParentCategoryId = 1 },
                new { Id = 2, SubcategoryName = "Cancellation", SubcategoryExternalReferenceId = 102, ParentCategoryId = 1 },
                new { Id = 3, SubcategoryName = "Flight Info", SubcategoryExternalReferenceId = 201, ParentCategoryId = 2 },
                new { Id = 4, SubcategoryName = "Card Declined", SubcategoryExternalReferenceId = 301, ParentCategoryId = 3 },
                new { Id = 5, SubcategoryName = "Refund Status", SubcategoryExternalReferenceId = 302, ParentCategoryId = 3 });

            // Seed Ticket data (depends on User, TicketCategory, TicketSubcategory)
            modelBuilder.Entity<ComplaintTicket>().HasData(
                new { Id = 1, Subject = "Cannot book flight", Description = "System error when attempting to book", CreationTimestamp = new DateTime(2026, 5, 4, 10, 0, 0), UrgencyLevel = ComplaintTicketUrgencyLevelEnum.HIGH, CurrentStatus = ComplaintTicketStatusEnum.RESOLVED, CreatorId = 101, CategoryId = 1, SubcategoryId = 1 },
                new { Id = 2, Subject = "Question about baggage", Description = "What is the baggage allowance?", CreationTimestamp = new DateTime(2026, 5, 4, 11, 0, 0), UrgencyLevel = ComplaintTicketUrgencyLevelEnum.LOW, CurrentStatus = ComplaintTicketStatusEnum.OPEN, CreatorId = 102, CategoryId = 2, SubcategoryId = 3 },
                new { Id = 3, Subject = "Payment failed", Description = "My card was declined during checkout", CreationTimestamp = new DateTime(2026, 5, 4, 12, 0, 0), UrgencyLevel = ComplaintTicketUrgencyLevelEnum.HIGH, CurrentStatus = ComplaintTicketStatusEnum.OPEN, CreatorId = 103, CategoryId = 3, SubcategoryId = 4 },
                new { Id = 4, Subject = "Refund pending", Description = "How long until my refund appears?", CreationTimestamp = new DateTime(2026, 5, 4, 12, 30, 0), UrgencyLevel = ComplaintTicketUrgencyLevelEnum.MEDIUM, CurrentStatus = ComplaintTicketStatusEnum.RESOLVED, CreatorId = 101, CategoryId = 3, SubcategoryId = 5 });

            // Seed FAQEntry data
            modelBuilder.Entity<FAQEntry>().HasData(
                new FAQEntry { Id = 1, Question = "How do I reset my password?", Answer = "Click on the 'Forgot Password' link on the login page.", Category = FAQCategoryEnum.Parking, ViewCount = 150, HelpfulVotesCount = 120, NotHelpfulVotesCount = 5 },
                new FAQEntry { Id = 2, Question = "What is the baggage allowance?", Answer = "Standard baggage allowance is 2 checked bags. See add-ons for additional bags.", Category = FAQCategoryEnum.Baggage, ViewCount = 200, HelpfulVotesCount = 180, NotHelpfulVotesCount = 10 },
                new FAQEntry { Id = 3, Question = "Can I change my flight?", Answer = "Yes, flight changes can be made up to 24 hours before departure, subject to availability.", Category = FAQCategoryEnum.Facilities, ViewCount = 180, HelpfulVotesCount = 160, NotHelpfulVotesCount = 8 },
                new FAQEntry { Id = 4, Question = "Which payment methods are accepted?", Answer = "We accept major credit cards and selected digital wallets.", Category = FAQCategoryEnum.Facilities, ViewCount = 95, HelpfulVotesCount = 80, NotHelpfulVotesCount = 2 },
                new FAQEntry { Id = 5, Question = "How do I contact support?", Answer = "Use the chat assistant or submit a support ticket from your account.", Category = FAQCategoryEnum.Facilities, ViewCount = 120, HelpfulVotesCount = 110, NotHelpfulVotesCount = 3 });

            // Seed MembershipAddonDiscount data using shadow FK values
            modelBuilder.Entity<MembershipAddonDiscount>().HasData(
                new { MembershipId = 1, AddOnId = 1, DiscountPercentage = 10f },
                new { MembershipId = 1, AddOnId = 2, DiscountPercentage = 10f },
                new { MembershipId = 2, AddOnId = 1, DiscountPercentage = 20f },
                new { MembershipId = 2, AddOnId = 2, DiscountPercentage = 20f },
                new { MembershipId = 3, AddOnId = 1, DiscountPercentage = 30f },
                new { MembershipId = 3, AddOnId = 2, DiscountPercentage = 30f },
                new { MembershipId = 3, AddOnId = 3, DiscountPercentage = 30f },
                new { MembershipId = 3, AddOnId = 4, DiscountPercentage = 35f });
            }
    }
}
