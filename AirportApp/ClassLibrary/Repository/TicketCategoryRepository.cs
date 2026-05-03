using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using AirportApp.ClassLibrary.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository
{
    public class TicketCategoryRepository : ITicketCategoryRepository
    {
        private readonly AirportDbContext _context;

        public TicketCategoryRepository(AirportDbContext context)
        {
            _context = context;
        }

        public IEnumerable<TicketCategory> GetAll()
        {
            // EF Core executes the SQL query immediately when ToList() is called
            return _context.ticketCategories.ToList();
        }

        public TicketCategory GetById(int categoryId)
        {
            // Find() looks for the entity by its primary key
            return _context.ticketCategories.Find(categoryId)
                   ?? throw new KeyNotFoundException($"Category with id {categoryId} not found.");
        }
    }
}