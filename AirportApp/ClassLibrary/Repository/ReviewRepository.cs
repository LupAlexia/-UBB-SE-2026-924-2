using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain.Review;
using AirportApp.ClassLibrary.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository
{
    public class ReviewRepository : IRepository<int, Review>
    {
        private readonly AirportDbContext _context;

        public ReviewRepository(AirportDbContext context)
        {
            _context = context;
        }

        public Review GetById(int id)
        {
            // .Include(r => r.User) ensures the author is loaded
            return _context.reviews
                .Include(r => r.User)
                .FirstOrDefault(r => r.Id == id)
                ?? throw new KeyNotFoundException($"Review with id {id} was not found.");
        }

        public IEnumerable<Review> GetAll()
        {
            return _context.reviews
                .Include(r => r.User)
                .ToList();
        }

        public int CreateNewEntity(Review reviewElement)
        {
            if (reviewElement == null) throw new ArgumentNullException(nameof(reviewElement));

            _context.reviews.Add(reviewElement);
            _context.SaveChanges();
            return reviewElement.Id;
        }

        public void UpdateById(int id, Review reviewElement)
        {
            if (reviewElement == null) throw new ArgumentNullException(nameof(reviewElement));

            // EF Core updates existing entities
            _context.reviews.Update(reviewElement);
            _context.SaveChanges();
        }

        public void DeleteById(int id)
        {
            var review = _context.reviews.Find(id);
            if (review != null)
            {
                _context.reviews.Remove(review);
                _context.SaveChanges();
            }
        }
    }
}