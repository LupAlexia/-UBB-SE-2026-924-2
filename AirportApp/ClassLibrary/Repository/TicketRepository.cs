using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.ClassLibrary.Repository
{
    public class TicketRepository : ITicketRepository
    {
        private readonly AirportDbContext dataBaseContext;

        public TicketRepository(AirportDbContext context)
        {
            dataBaseContext = context ?? throw new ArgumentNullException(nameof(dataBaseContext));
        }

        public async Task<IEnumerable<Ticket>> GetAllAsync()
        {
            return await dataBaseContext.tickets
                .Include(t => t.Creator)
                .Include(t => t.Category)
                .Include(t => t.Subcategory)
                .ToListAsync();
        }

        public async Task<Ticket> GetByIdAsync(int id)
        {
            return await dataBaseContext.tickets
                .Include(t => t.Creator)
                .Include(t => t.Category)
                .Include(t => t.Subcategory)
                .FirstOrDefaultAsync(t => t.Id == id)
                ?? throw new KeyNotFoundException($"Ticket with id {id} not found.");
        }

        public async Task<int> CreateNewEntityAsync(Ticket ticket)
        {
            dataBaseContext.tickets.Add(ticket);
            await dataBaseContext.SaveChangesAsync();
            return ticket.Id;
        }

        public async Task UpdateByIdAsync(int id, Ticket ticket)
        {
            

            dataBaseContext.tickets.Update(ticket);
            await dataBaseContext.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var ticket = await dataBaseContext.tickets.FindAsync(id);
            if (ticket != null)
            {
                dataBaseContext.tickets.Remove(ticket);
                await dataBaseContext.SaveChangesAsync();
            }
        }
    }
}