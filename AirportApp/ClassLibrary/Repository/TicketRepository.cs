using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using AirportApp.ClassLibrary.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository
{
    public class TicketRepository : ITicketRepository
    {
        private readonly AirportDbContext _context;

        public TicketRepository(AirportDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Ticket> GetAll()
        {
            // Eager loading related entities
            return _context.tickets
                .Include(t => t.Creator)
                .Include(t => t.Category)
                .Include(t => t.Subcategory)
                .ToList();
        }

        public Ticket GetById(int id)
        {
            return _context.tickets
                .Include(t => t.Creator)
                .Include(t => t.Category)
                .Include(t => t.Subcategory)
                .FirstOrDefault(t => t.Id == id)
                ?? throw new KeyNotFoundException($"Ticket with id {id} not found.");
        }

        public int CreateNewEntity(Ticket ticket)
        {
            _context.tickets.Add(ticket);
            _context.SaveChanges();
            return ticket.Id;
        }

        public void UpdateById(int id, Ticket ticket)
        {
            _context.tickets.Update(ticket);
            _context.SaveChanges();
        }

        public void DeleteById(int id)
        {
            var ticket = _context.tickets.Find(id);
            if (ticket != null)
            {
                _context.tickets.Remove(ticket);
                _context.SaveChanges();
            }
        }
    }
}