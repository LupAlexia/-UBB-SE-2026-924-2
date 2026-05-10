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
            return await this.dataBaseContext.FlightTickets
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
            if (ticket == null)
            {
                throw new ArgumentNullException(nameof(ticket));
            }

            if (ticket.User == null || ticket.User.Id <= 0)
            {
                throw new ArgumentException("Flight ticket must contain a valid user.", nameof(ticket));
            }

            if (ticket.Flight == null || ticket.Flight.Id <= 0)
            {
                throw new ArgumentException("Flight ticket must contain a valid flight.", nameof(ticket));
            }

            this.dataBaseContext.Add(ticket);
            this.dataBaseContext.Entry(ticket).Property("UserId").CurrentValue = ticket.User.Id;
            this.dataBaseContext.Entry(ticket).Property("FlightId").CurrentValue = ticket.Flight.Id;
            await this.dataBaseContext.SaveChangesAsync();
        }

        public async Task UpdateTicketStatusAsync(int ticketId, string status)
        {
            var ticket = await this.dataBaseContext.FlightTickets.FirstOrDefaultAsync(ticketEntity => ticketEntity.Id == ticketId);
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

            var ticket = await this.dataBaseContext.FlightTickets
                .Include(t => t.SelectedAddOns)
                .FirstOrDefaultAsync(ticketEntity => ticketEntity.Id == ticketId);

            if (ticket == null)
            {
                return;
            }

            foreach (var addOnId in addOnIds)
            {
                var addOn = await this.dataBaseContext.AddOns.FirstOrDefaultAsync(addOn => addOn.Id == addOnId);
                if (addOn != null && !ticket.SelectedAddOns.Contains(addOn))
                {
                    ticket.SelectedAddOns.Add(addOn);
                }
            }

            await this.dataBaseContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<string>> GetOccupiedSeatsAsync(int flightId)
        {
            return await this.dataBaseContext.FlightTickets
                .Where(ticket => ticket.Flight.Id == flightId
                    && ticket.Status != CancelledStatus
                    && ticket.Seat != null)
                .Select(ticket => ticket.Seat)
                .ToListAsync();
        }

        public async Task<bool> IsSeatAvailableAsync(int flightId, string seat)
        {
            int count = await this.dataBaseContext.FlightTickets
                .Where(ticket => ticket.Flight.Id == flightId
                    && ticket.Seat == seat
                    && ticket.Status != CancelledStatus)
                .CountAsync();

            return count == 0;
        }

        public async Task<bool> SaveTicketsWithAddOnsAsync(List<FlightTicket> tickets, List<List<int>> addOnIds = null)
        {
            try
            {
                for (int ticketIndex = 0; ticketIndex < tickets.Count; ticketIndex++)
                {
                        var ticket = tickets[ticketIndex];

                        var userId = ticket.User?.Id ?? 0;
                        var flightId = ticket.Flight?.Id ?? 0;

                    ticket.User = null;
                    ticket.Flight = null;

                    var currentAddOnIds = (addOnIds != null && ticketIndex < addOnIds.Count)
                        ? addOnIds[ticketIndex]
                        : new List<int>();

                    if (currentAddOnIds.Any())
                    {
                        var attachedAddOns = new List<AddOn>();
                        foreach (var addOnId in currentAddOnIds)
                        {
                            // Fetch the tracked entity from the database
                            var existing = await dataBaseContext.AddOns.FindAsync(addOnId);
                            if (existing != null)
                            {
                                attachedAddOns.Add(existing);
                            }
                        }
                        ticket.SelectedAddOns = attachedAddOns;
                    }
                    else
                    {
                        ticket.SelectedAddOns = new List<AddOn>();
                    }

                    this.dataBaseContext.FlightTickets.Add(ticket);

                    if (userId > 0)
                    {
                        this.dataBaseContext.Entry(ticket).Property("UserId").CurrentValue = userId;
                    }

                    if (flightId > 0)
                    {
                        this.dataBaseContext.Entry(ticket).Property("FlightId").CurrentValue = flightId;
                    }
                }

                await this.dataBaseContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving tickets: {ex}");
                return false;
            }
        }
    }
}
