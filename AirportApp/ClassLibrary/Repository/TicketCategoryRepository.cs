using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using AirportApp.ClassLibrary.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository
{
    public class TicketCategoryRepository : ITicketCategoryRepository
    {
        private readonly AirportDbContext dataBaseContext;

        public TicketCategoryRepository(AirportDbContext context)
        {
            dataBaseContext = context ?? throw new ArgumentNullException(nameof(dataBaseContext));
        }

        public IEnumerable<TicketCategory> GetAll()
        {
            // EF Core executes the SQL query immediately when ToList() is called
            return dataBaseContext.ticketCategories.ToList();
        }

        public TicketCategory GetById(int categoryId)
        {
            // Find() looks for the entity by its primary key
            return dataBaseContext.ticketCategories.Find(categoryId)
                   ?? throw new KeyNotFoundException($"Category with id {categoryId} not found.");
        }
    }
}