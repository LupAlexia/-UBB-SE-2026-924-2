using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository
{
    public class ComplaintTicketRepository : ITicketRepository
    {
        private readonly AirportDbContext databaseContext;

        public ComplaintTicketRepository(AirportDbContext databaseContext)
        {
            this.databaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
        }

        public async Task<IEnumerable<ComplaintTicket>> GetAllAsync()
        {
            return await databaseContext.Tickets
                .Include(ticket => ticket.Creator)
                .Include(ticket => ticket.Category)
                .Include(ticket => ticket.Subcategory)
                .ToListAsync();
        }

        public async Task<ComplaintTicket> GetByIdAsync(int id)
        {
            return await databaseContext.Tickets
                .Include(ticket => ticket.Creator)
                .Include(ticket => ticket.Category)
                .Include(ticket => ticket.Subcategory)
                .FirstOrDefaultAsync(ticket => ticket.Id == id)
                ?? throw new KeyNotFoundException($"Ticket with id {id} not found.");
        }

        public async Task<int> CreateNewEntityAsync(ComplaintTicket ticket)
        {
            databaseContext.Tickets.Add(ticket);
            await databaseContext.SaveChangesAsync();
            return ticket.Id;
        }

        public async Task UpdateByIdAsync(int id, ComplaintTicket ticket)
        {
            databaseContext.Tickets.Update(ticket);
            await databaseContext.SaveChangesAsync();
        }

        public async Task UpdateStatusByIdAsync(int id, ComplaintTicketStatusEnum newStatus)
        {
            var ticket = await databaseContext.Tickets.FindAsync(id);
            if (ticket == null)
            {
                throw new KeyNotFoundException($"Ticket with id {id} not found.");
            }

            ticket.CurrentStatus = newStatus;
            await databaseContext.SaveChangesAsync();
        }

        public async Task UpdateUrgencyLevelByIdAsync(int id, ComplaintTicketUrgencyLevelEnum newUrgencyLevel)
        {
            var ticket = await databaseContext.Tickets.FindAsync(id);
            if (ticket == null)
            {
                throw new KeyNotFoundException($"Ticket with id {id} not found.");
            }

            ticket.UrgencyLevel = newUrgencyLevel;
            await databaseContext.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var ticket = await databaseContext.Tickets.FindAsync(id);
            if (ticket != null)
            {
                databaseContext.Tickets.Remove(ticket);
                await databaseContext.SaveChangesAsync();
            }
        }
    }
}