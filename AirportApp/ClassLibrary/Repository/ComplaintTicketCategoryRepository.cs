using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository
{
    public class ComplaintTicketCategoryRepository : ITicketCategoryRepository
    {
        private readonly AirportDbContext databaseContext;

        public ComplaintTicketCategoryRepository(AirportDbContext databaseContext)
        {
            this.databaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
        }

        public async Task<IEnumerable<ComplaintTicketCategory>> GetAllAsync()
        {
            return await databaseContext.TicketCategories.ToListAsync();
        }

        public async Task<ComplaintTicketCategory> GetByIdAsync(int categoryId)
        {
            return await databaseContext.TicketCategories.FindAsync(categoryId)
                   ?? throw new KeyNotFoundException($"Category with id {categoryId} not found.");
        }
    }
}