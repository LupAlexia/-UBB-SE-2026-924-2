using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository
{
    public class ComplaintTicketSubcategoryRepository : ITicketSubcategoryRepository
    {
        private readonly AirportDbContext dataBaseContext;

        public ComplaintTicketSubcategoryRepository(AirportDbContext context)
        {
            dataBaseContext = context ?? throw new ArgumentNullException(nameof(dataBaseContext));
        }

        public async Task<IEnumerable<ComplaintTicketSubcategory>> GetAllAsync()
        {
            return await dataBaseContext.TicketSubcategories
                           .Include(s => s.ParentCategory)
                           .ToListAsync();
        }

        public async Task<ComplaintTicketSubcategory> GetByIdAsync(int subcategoryId)
        {
            return await dataBaseContext.TicketSubcategories
                           .Include(s => s.ParentCategory)
                           .FirstOrDefaultAsync(s => s.Id == subcategoryId)
                   ?? throw new KeyNotFoundException($"Subcategory with id {subcategoryId} not found.");
        }

        public async Task<IEnumerable<ComplaintTicketSubcategory>> GetByCategoryIdAsync(int categoryId)
        {
            return await dataBaseContext.TicketSubcategories
                           .Include(s => s.ParentCategory)
                           .Where(s => s.ParentCategory.Id == categoryId)
                           .ToListAsync();
        }
    }
}