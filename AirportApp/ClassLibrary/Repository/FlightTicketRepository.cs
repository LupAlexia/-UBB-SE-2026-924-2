using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository
{
    public class FlightTicketRepository : IFlightTicketRepository
    {
        private const string CancelledStatus = "Cancelled";

        private readonly AirportDbContext dataBaseContext;

        public FlightTicketRepository(AirportDbContext dataBaseContext)
        {
            this.dataBaseContext = dataBaseContext;
        }

        public IEnumerable<FlightTicket> GetTicketsByUserId(int userId)
        {
            return this.dataBaseContext.flightTickets
                .Where(ticket => ticket.User.UserId == userId)
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
                .ToList();
        }

        public void AddTicket(FlightTicket ticket)
        {
            this.dataBaseContext.Add(ticket);
            this.dataBaseContext.SaveChanges();
        }

        public void UpdateTicketStatus(int ticketId, string status)
        {
            var ticket = this.dataBaseContext.flightTickets.FirstOrDefault(t => t.TicketId == ticketId);
            if (ticket != null)
            {
                ticket.Status = status;
                this.dataBaseContext.SaveChanges();
            }
        }

        public void AddTicketAddOns(int ticketId, IEnumerable<int> addOnIds)
        {
            if (addOnIds == null || !addOnIds.Any())
            {
                return;
            }

            var ticket = this.dataBaseContext.flightTickets
                .Include(t => t.SelectedAddOns)
                .FirstOrDefault(t => t.TicketId == ticketId);

            if (ticket == null)
            {
                return;
            }

            foreach (var addOnId in addOnIds)
            {
                var addOn = this.dataBaseContext.addOns.FirstOrDefault(a => a.AddOnId == addOnId);
                if (addOn != null && !ticket.SelectedAddOns.Contains(addOn))
                {
                    ticket.SelectedAddOns.Add(addOn);
                }
            }

            this.dataBaseContext.SaveChanges();
        }

        public IEnumerable<string> GetOccupiedSeats(int flightId)
        {
            return this.dataBaseContext.flightTickets
                .Where(ticket => ticket.Flight.FlightId == flightId 
                    && ticket.Status != CancelledStatus 
                    && ticket.Seat != null)
                .Select(ticket => ticket.Seat)
                .ToList();
        }

        public async Task<bool> IsSeatAvailable(int flightId, string seat)
        {
            int count = await this.dataBaseContext.flightTickets
                .Where(ticket => ticket.Flight.FlightId == flightId 
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


