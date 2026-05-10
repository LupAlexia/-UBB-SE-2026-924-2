using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.Entity.Domain;
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
        public DbSet<ComplaintTicketCategory> TicketCategories { get; set; }
        public DbSet<ComplaintTicketSubcategory> TicketSubcategories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Gate> Gates { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<FAQNode> FaqNodes { get; set; }
        public DbSet<FAQOption> FaqOptions { get; set; }

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

            modelBuilder.Entity<FAQNode>(faqNodeBuilder =>
            {
                faqNodeBuilder.ToTable("FAQNode");
                faqNodeBuilder.HasKey(node => node.NodeId);
                faqNodeBuilder.Property(node => node.NodeId).HasColumnName("node_id");
                faqNodeBuilder.Property(node => node.QuestionText).HasColumnName("question_text");
                faqNodeBuilder.Property(node => node.IsFinalAnswer).HasColumnName("is_final_answer");
                faqNodeBuilder.HasMany(node => node.Options)
                    .WithOne()
                    .HasForeignKey("NodeId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<FAQOption>(faqOptionBuilder =>
            {
                faqOptionBuilder.ToTable("FAQOption");
                faqOptionBuilder.HasKey(option => option.OptionId);
                faqOptionBuilder.Property(option => option.OptionId).HasColumnName("option_id");

                // Shadow property for node_id (parent node foreign key)
                faqOptionBuilder.Property<int>("NodeId").HasColumnName("node_id");

                faqOptionBuilder.Property(option => option.Label).HasColumnName("label");

                // Shadow FK property for next_option_id
                faqOptionBuilder.Property<int?>("NextOptionId").HasColumnName("next_option_id");

                faqOptionBuilder.HasOne(option => option.NextOption)
                    .WithMany()
                    .HasForeignKey("NextOptionId")
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);
            });

            modelBuilder.Entity<Airport>()
                .HasMany<Gate>(airport => airport.Gates)
                .WithOne(gate => gate.Airport)
                .HasForeignKey("AirportId")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Route>()
                .HasOne(route => route.Company)
                .WithMany()
                .HasForeignKey("CompanyId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Route>()
                .HasOne(route => route.Airport)
                .WithMany()
                .HasForeignKey("AirportId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Flight>()
                .HasOne(flight => flight.Route)
                .WithMany()
                .HasForeignKey("RouteId")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Flight>()
                .HasOne(flight => flight.Gate)
                .WithMany()
                .HasForeignKey("GateId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Membership>()
                .HasMany(membership => membership.AddonDiscounts)
                .WithOne(discount => discount.Membership)
                .HasForeignKey("MembershipId")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MembershipAddonDiscount>(discountBuilder =>
            {
                discountBuilder.ToTable("Membership_Addon_Discounts");
                discountBuilder.HasKey("MembershipId", "AddOnId");
                discountBuilder.Property<int>("MembershipId").HasColumnName("Membership_Id");
                discountBuilder.Property<int>("AddOnId").HasColumnName("AddOn_Id");
                discountBuilder.Property(discount => discount.DiscountPercentage).HasColumnName("Discount_Percentage");
                discountBuilder.HasOne(discount => discount.Membership)
                    .WithMany(membership => membership.AddonDiscounts)
                    .HasForeignKey("MembershipId")
                    .OnDelete(DeleteBehavior.Cascade);
                discountBuilder.HasOne(discount => discount.AddOn)
                    .WithMany()
                    .HasForeignKey("AddOnId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Customer>()
                .HasOne(customer => customer.Membership)
                .WithMany()
                .HasForeignKey("MembershipId")
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Chat>()
                .HasOne(chat => chat.User)
                .WithMany()
                .HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasOne(message => message.Chat)
                .WithMany(chat => chat.Messages)
                .HasForeignKey("ChatId") // Shadow FK
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasOne(message => message.Sender)
                .WithMany()
                .HasForeignKey("SenderId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(review => review.User)
                .WithMany()
                .HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ComplaintTicket>()
                .HasOne(ticket => ticket.Creator)
                .WithMany()
                .HasForeignKey("CreatorId")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ComplaintTicket>()
                .HasOne(ticket => ticket.Category)
                .WithMany()
                .HasForeignKey("CategoryId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ComplaintTicket>()
                .HasOne(ticket => ticket.Subcategory)
                .WithMany()
                .HasForeignKey("SubcategoryId")
                .OnDelete(DeleteBehavior.Restrict);

            // Configure many-to-many between FlightTicket and AddOn via explicit join table
            modelBuilder.Entity<FlightTicket>()
                .HasMany(flightTicket => flightTicket.SelectedAddOns)
                .WithMany(addon => addon.Tickets)
                .UsingEntity<Dictionary<string, object>>(
                    "FlightTicket_AddOns",
                    right => right.HasOne<AddOn>().WithMany().HasForeignKey("AddOn_Id").HasConstraintName("FK_FlightTicketAddOns_AddOn"),
                    left => left.HasOne<FlightTicket>().WithMany().HasForeignKey("Ticket_Id").HasConstraintName("FK_FlightTicketAddOns_FlightTicket"),
                    joinEntity =>
                    {
                        joinEntity.HasKey("Ticket_Id", "AddOn_Id");
                        joinEntity.ToTable("FlightTicket_AddOns");
                    });

            User user1 = new User { Id = 101, FullName = "Alice Bot", EmailAddress = "alice@bot.com" };
            User user2 = new User { Id = 102, FullName = "Bob Chat", EmailAddress = "bob@chat.com" };
            User user3 = new User { Id = 103, FullName = "Mia Passenger", EmailAddress = "mia@example.com" };

            // Seed some basic decision-tree data for FAQs using entity types
            modelBuilder.Entity<FAQNode>().HasData(
                new { NodeId = 1, QuestionText = "How can I help you today?", IsFinalAnswer = false },
                new { NodeId = 2, QuestionText = "What is the issue with your baggage?", IsFinalAnswer = false },
                new { NodeId = 3, QuestionText = "Check your email for a tracking link or visit the lost & found desk.", IsFinalAnswer = true },
                new { NodeId = 4, QuestionText = "Please file a \"Property Irregularity Report\" at the arrival hall.", IsFinalAnswer = true },
                new { NodeId = 5, QuestionText = "What would you like to do with your booking?", IsFinalAnswer = false },
                new { NodeId = 6, QuestionText = "You can change your flight via the \"My Bookings\" section on our website.", IsFinalAnswer = true });

            modelBuilder.Entity<FAQOption>().HasData(
                new { OptionId = 1, NodeId = 1, Label = "Baggage Issues", NextOptionId = 2 },
                new { OptionId = 2, NodeId = 1, Label = "Manage Booking", NextOptionId = 5 },
                new { OptionId = 3, NodeId = 2, Label = "Lost Baggage", NextOptionId = 3 },
                new { OptionId = 4, NodeId = 2, Label = "Damaged Baggage", NextOptionId = 4 },
                new { OptionId = 5, NodeId = 5, Label = "Change Flight Date", NextOptionId = 6 });

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

            modelBuilder.Entity<Message>().HasData(
                new { Id = 1, ChatId = 1, SenderId = 101, Text = "Hello! I need help with flights.", Timestamp = new DateTimeOffset(new DateTime(2026, 5, 4, 9, 0, 0)) },
                new { Id = 2, ChatId = 2, SenderId = 102, Text = "Hi, I have a question about membership.", Timestamp = new DateTimeOffset(new DateTime(2026, 5, 4, 9, 5, 0)) },
                new { Id = 3, ChatId = 3, SenderId = 103, Text = "Hello, I need support with my booking.", Timestamp = new DateTimeOffset(new DateTime(2026, 5, 4, 9, 10, 0)) });

            modelBuilder.Entity<User>().HasData(
                user1,
                user2,
                user3);

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
