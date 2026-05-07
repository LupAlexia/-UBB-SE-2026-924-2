using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.ClassLibrary.Repository
{
    public class ComplaintTicketRepository : ITicketRepository
    {
        private readonly AirportDbContext dataBaseContext;

        public ComplaintTicketRepository(AirportDbContext context)
        {
            dataBaseContext = context ?? throw new ArgumentNullException(nameof(dataBaseContext));
        }

        public async Task<IEnumerable<ComplaintTicket>> GetAllAsync()
        {
            return await dataBaseContext.Tickets
                .Include(t => t.Creator)
                .Include(t => t.Category)
                .Include(t => t.Subcategory)
                .ToListAsync();
        }

        public async Task<ComplaintTicket> GetByIdAsync(int id)
        {
            return await dataBaseContext.Tickets
                .Include(t => t.Creator)
                .Include(t => t.Category)
                .Include(t => t.Subcategory)
                .FirstOrDefaultAsync(t => t.Id == id)
                ?? throw new KeyNotFoundException($"Ticket with id {id} not found.");
        }

        public async Task<int> CreateNewEntityAsync(ComplaintTicket ticket)
        {
            dataBaseContext.Tickets.Add(ticket);
            await dataBaseContext.SaveChangesAsync();
            return ticket.Id;
        }

        public async Task UpdateByIdAsync(int id, ComplaintTicket ticket)
        {
            

            dataBaseContext.Tickets.Update(ticket);
            await dataBaseContext.SaveChangesAsync();
        }

        public async Task UpdateStatusByIdAsync(int id, ComplaintTicketStatusEnum newStatus)
        {
            var ticket = await dataBaseContext.Tickets.FindAsync(id);
            if (ticket == null)
            {
                throw new KeyNotFoundException($"Ticket with id {id} not found.");
            }

            ticket.CurrentStatus = newStatus;
            await dataBaseContext.SaveChangesAsync();
        }

        public async Task UpdateUrgencyLevelByIdAsync(int id, ComplaintTicketUrgencyLevelEnum newUrgencyLevel)
        {
            var ticket = await dataBaseContext.Tickets.FindAsync(id);
            if (ticket == null)
            {
                throw new KeyNotFoundException($"Ticket with id {id} not found.");
            }

            ticket.UrgencyLevel = newUrgencyLevel;
            await dataBaseContext.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var ticket = await dataBaseContext.Tickets.FindAsync(id);
            if (ticket != null)
            {
                dataBaseContext.Tickets.Remove(ticket);
                await dataBaseContext.SaveChangesAsync();
            }
        }
    }
}