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
        private readonly AirportDbContext databaseContext;

        public ComplaintTicketSubcategoryRepository(AirportDbContext databaseContext)
        {
            this.databaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
        }

        public async Task<IEnumerable<ComplaintTicketSubcategory>> GetAllAsync()
        {
            return await databaseContext.TicketSubcategories
                           .Include(subcategory => subcategory.ParentCategory)
                           .ToListAsync();
        }

        public async Task<ComplaintTicketSubcategory> GetByIdAsync(int subcategoryId)
        {
            return await databaseContext.TicketSubcategories
                           .Include(subcategory => subcategory.ParentCategory)
                           .FirstOrDefaultAsync(subcategory => subcategory.Id == subcategoryId)
                   ?? throw new KeyNotFoundException($"Subcategory with id {subcategoryId} not found.");
        }

        public async Task<IEnumerable<ComplaintTicketSubcategory>> GetByCategoryIdAsync(int categoryId)
        {
            return await databaseContext.TicketSubcategories
                           .Include(subcategory => subcategory.ParentCategory)
                           .Where(subcategory => subcategory.ParentCategory.Id == categoryId)
                           .ToListAsync();
        }
    }
}