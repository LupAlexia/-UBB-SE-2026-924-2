using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using AirportApp.ClassLibrary.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository
{
    public class TicketRepository : ITicketRepository
    {
        private readonly AirportDbContext dataBaseContext;

        public TicketRepository(AirportDbContext context)
        {
            dataBaseContext = context ?? throw new ArgumentNullException(nameof(dataBaseContext));
        }

        public IEnumerable<Ticket> GetAll()
        {
            // Eager loading related entities
            return dataBaseContext.tickets
                .Include(t => t.Creator)
                .Include(t => t.Category)
                .Include(t => t.Subcategory)
                .ToList();
        }

        public Ticket GetById(int id)
        {
            return dataBaseContext.tickets
                .Include(t => t.Creator)
                .Include(t => t.Category)
                .Include(t => t.Subcategory)
                .FirstOrDefault(t => t.Id == id)
                ?? throw new KeyNotFoundException($"Ticket with id {id} not found.");
        }

        public int CreateNewEntity(Ticket ticket)
        {
            dataBaseContext.tickets.Add(ticket);
            dataBaseContext.SaveChanges();
            return ticket.Id;
        }

        public void UpdateById(int id, Ticket ticket)
        {
            dataBaseContext.tickets.Update(ticket);
            dataBaseContext.SaveChanges();
        }

        public void DeleteById(int id)
        {
            var ticket = dataBaseContext.tickets.Find(id);
            if (ticket != null)
            {
                dataBaseContext.tickets.Remove(ticket);
                dataBaseContext.SaveChanges();
            }
        }
    }
}