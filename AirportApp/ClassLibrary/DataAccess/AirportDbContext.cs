using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.Entity.Domain.Chats;
using AirportApp.ClassLibrary.Entity.Domain.Employee;
using AirportApp.ClassLibrary.Entity.Domain.Faq;
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
        public DbSet<Message> messages { get; set; }
        public DbSet<TicketCategory> ticketCategories { get; set; }
        public DbSet<TicketSubcategory> ticketSubcategories { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<Company> companies { get; set; }
        public DbSet<Gate> gates { get; set; }
        public DbSet<Route> routes { get; set; }
    }
}
