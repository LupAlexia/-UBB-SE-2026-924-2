using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.DataAccess;

namespace AirportApp.ClassLibrary.Repository
{
    public class FlightTicketRepository : IFlightTicketRepository
    {
        private const string CancelledStatus = "Cancelled";

        private readonly AirportDbContext dataBaseContext;

        public FlightTicketRepository(AirportDbContext dataBaseContext)
        {
            this.dataBaseContext = dataBaseContext ?? throw new ArgumentNullException(nameof(dataBaseContext));
        }

        public async Task<IEnumerable<FlightTicket>> GetTicketsByUserIdAsync(int userId)
        {
            return await this.dataBaseContext.flightTickets
                .Where(ticket => ticket.User.Id == userId)
                .Include(ticket => ticket.User)
                .Include(ticket => ticket.Flight)
                    .ThenInclude(flight => flight.Route)
                        .ThenInclude(route => route.Airport)
                .Include(ticket => ticket.Flight)
                    .ThenInclude(flight => flight.Route)
                        .ThenInclude(route => route.Company)
                .Include(ticket => ticket.Flight)
                    .ThenInclude(flight => flight.Gate)
                .Include(ticket => ticket.SelectedAddOns)
                .ToListAsync();
        }

        public async Task AddTicketAsync(FlightTicket ticket)
        {
            this.dataBaseContext.Add(ticket);
            await this.dataBaseContext.SaveChangesAsync();
        }

        public async Task UpdateTicketStatusAsync(int ticketId, string status)
        {
            var ticket = await this.dataBaseContext.flightTickets.FirstOrDefaultAsync(t => t.Id == ticketId);
            if (ticket != null)
            {
                ticket.Status = status;
                await this.dataBaseContext.SaveChangesAsync();
            }
        }

        public async Task AddTicketAddOnsAsync(int ticketId, IEnumerable<int> addOnIds)
        {
            if (addOnIds == null || !addOnIds.Any())
            {
                return;
            }

            var ticket = await this.dataBaseContext.flightTickets
                .Include(t => t.SelectedAddOns)
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
            {
                return;
            }

            foreach (var addOnId in addOnIds)
            {
                var addOn = await this.dataBaseContext.addOns.FirstOrDefaultAsync(a => a.Id == addOnId);
                if (addOn != null && !ticket.SelectedAddOns.Contains(addOn))
                {
                    ticket.SelectedAddOns.Add(addOn);
                }
            }

            await this.dataBaseContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<string>> GetOccupiedSeatsAsync(int flightId)
        {
            return await this.dataBaseContext.flightTickets
                .Where(ticket => ticket.Flight.Id == flightId
                    && ticket.Status != CancelledStatus
                    && ticket.Seat != null)
                .Select(ticket => ticket.Seat)
                .ToListAsync();
        }

        public async Task<bool> IsSeatAvailableAsync(int flightId, string seat)
        {
            int count = await this.dataBaseContext.flightTickets
                .Where(ticket => ticket.Flight.Id == flightId
                    && ticket.Seat == seat
                    && ticket.Status != CancelledStatus)
                .CountAsync();

            return count == 0;
        }

        public async Task<bool> SaveTicketsWithAddOnsAsync(List<FlightTicket> tickets)
        {
            try
            {
                foreach (var ticket in tickets)
                {
                    this.dataBaseContext.flightTickets.Add(ticket);
                }

                await this.dataBaseContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
