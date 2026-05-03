using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain.Review;
using AirportApp.ClassLibrary.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository
{
    public class ReviewRepository : IRepository<int, Review>
    {
        private readonly AirportDbContext dataBaseContext;

        public ReviewRepository(AirportDbContext context)
        {
            dataBaseContext = context ?? throw new ArgumentNullException(nameof(dataBaseContext));
        }

        public Review GetById(int id)
        {
            // .Include(r => r.User) ensures the author is loaded
            return dataBaseContext.reviews
                .Include(r => r.User)
                .FirstOrDefault(r => r.Id == id)
                ?? throw new KeyNotFoundException($"Review with id {id} was not found.");
        }

        public IEnumerable<Review> GetAll()
        {
            return dataBaseContext.reviews
                .Include(r => r.User)
                .ToList();
        }

        public int CreateNewEntity(Review reviewElement)
        {
            if (reviewElement == null) throw new ArgumentNullException(nameof(reviewElement));

            dataBaseContext.reviews.Add(reviewElement);
            dataBaseContext.SaveChanges();
            return reviewElement.Id;
        }

        public void UpdateById(int id, Review reviewElement)
        {
            if (reviewElement == null) throw new ArgumentNullException(nameof(reviewElement));

            // EF Core updates existing entities
            dataBaseContext.reviews.Update(reviewElement);
            dataBaseContext.SaveChanges();
        }

        public void DeleteById(int id)
        {
            var review = dataBaseContext.reviews.Find(id);
            if (review != null)
            {
                dataBaseContext.reviews.Remove(review);
                dataBaseContext.SaveChanges();
            }
        }
    }
}