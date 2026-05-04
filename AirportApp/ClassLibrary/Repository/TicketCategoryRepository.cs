using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.ClassLibrary.Repository
{
    public class TicketCategoryRepository : ITicketCategoryRepository
    {
        private readonly AirportDbContext dataBaseContext;

        public TicketCategoryRepository(AirportDbContext context)
        {
            dataBaseContext = context ?? throw new ArgumentNullException(nameof(dataBaseContext));
        }

        public async Task<IEnumerable<TicketCategory>> GetAllAsync()
        {
            return await dataBaseContext.ticketCategories.ToListAsync();
        }

        public async Task<TicketCategory> GetByIdAsync(int categoryId)
        {
            return await dataBaseContext.ticketCategories.FindAsync(categoryId)
                   ?? throw new KeyNotFoundException($"Category with id {categoryId} not found.");
        }
    }
}