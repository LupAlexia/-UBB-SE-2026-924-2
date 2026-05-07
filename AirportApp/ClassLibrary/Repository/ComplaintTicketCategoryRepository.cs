using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.ClassLibrary.Repository
{
    public class ComplaintTicketCategoryRepository : ITicketCategoryRepository
    {
        private readonly AirportDbContext dataBaseContext;

        public ComplaintTicketCategoryRepository(AirportDbContext context)
        {
            dataBaseContext = context ?? throw new ArgumentNullException(nameof(dataBaseContext));
        }

        public async Task<IEnumerable<ComplaintTicketCategory>> GetAllAsync()
        {
            return await dataBaseContext.TicketCategories.ToListAsync();
        }

        public async Task<ComplaintTicketCategory> GetByIdAsync(int categoryId)
        {
            return await dataBaseContext.TicketCategories.FindAsync(categoryId)
                   ?? throw new KeyNotFoundException($"Category with id {categoryId} not found.");
        }
    }
}