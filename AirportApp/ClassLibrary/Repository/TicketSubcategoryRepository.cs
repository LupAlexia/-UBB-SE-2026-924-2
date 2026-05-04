using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.ClassLibrary.Repository
{
    public class TicketSubcategoryRepository : ITicketSubcategoryRepository
    {
        private readonly AirportDbContext dataBaseContext;

        public TicketSubcategoryRepository(AirportDbContext context)
        {
            dataBaseContext = context ?? throw new ArgumentNullException(nameof(dataBaseContext));
        }

        public async Task<IEnumerable<TicketSubcategory>> GetAllAsync()
        {
            return await dataBaseContext.ticketSubcategories
                           .Include(s => s.ParentCategory)
                           .ToListAsync();
        }

        public async Task<TicketSubcategory> GetByIdAsync(int subcategoryId)
        {
            return await dataBaseContext.ticketSubcategories
                           .Include(s => s.ParentCategory)
                           .FirstOrDefaultAsync(s => s.Id == subcategoryId)
                   ?? throw new KeyNotFoundException($"Subcategory with id {subcategoryId} not found.");
        }

        public async Task<IEnumerable<TicketSubcategory>> GetByCategoryIdAsync(int categoryId)
        {
            return await dataBaseContext.ticketSubcategories
                           .Include(s => s.ParentCategory)
                           .Where(s => s.ParentCategory.Id == categoryId)
                           .ToListAsync();
        }
    }
}