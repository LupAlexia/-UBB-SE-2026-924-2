using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using AirportApp.ClassLibrary.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository
{
    public class TicketSubcategoryRepository : ITicketSubcategoryRepository
    {
        private readonly AirportDbContext _context;

        public TicketSubcategoryRepository(AirportDbContext context)
        {
            _context = context;
        }

        public IEnumerable<TicketSubcategory> GetAll()
        {
            // .Include(s => s.Category) ensures the parent category is loaded
            return _context.ticketSubcategories
                           .Include(s => s.ParentCategory)
                           .ToList();
        }

        public TicketSubcategory GetById(int subcategoryId)
        {
            return _context.ticketSubcategories
                           .Include(s => s.ParentCategory)
                           .FirstOrDefault(s => s.Id == subcategoryId)
                   ?? throw new KeyNotFoundException($"Subcategory with id {subcategoryId} not found.");
        }

        public IEnumerable<TicketSubcategory> GetByCategoryId(int categoryId)
        {
            return _context.ticketSubcategories
                           .Include(s => s.ParentCategory)
                           .Where(s => s.ParentCategory.Id == categoryId)
                           .ToList();
        }
    }
}