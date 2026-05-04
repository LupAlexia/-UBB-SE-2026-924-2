using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain.Review;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.ClassLibrary.Repository
{
    public class ReviewRepository : IRepository<int, Review>
    {
        private readonly AirportDbContext dataBaseContext;

        public ReviewRepository(AirportDbContext context)
        {
            dataBaseContext = context ?? throw new ArgumentNullException(nameof(dataBaseContext));
        }

        public async Task<Review> GetByIdAsync(int id)
        {
            return await dataBaseContext.reviews
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id)
                ?? throw new KeyNotFoundException($"Review with id {id} was not found.");
        }

        public async Task<IEnumerable<Review>> GetAllAsync()
        {
            return await dataBaseContext.reviews
                .Include(r => r.User)
                .ToListAsync();
        }

        public async Task<int> CreateNewEntityAsync(Review reviewElement)
        {
            if (reviewElement == null)
            {
                throw new ArgumentNullException(nameof(reviewElement));
            }

            dataBaseContext.reviews.Add(reviewElement);
            await dataBaseContext.SaveChangesAsync();
            return reviewElement.Id;
        }

        public async Task UpdateByIdAsync(int id, Review reviewElement)
        {
            if (reviewElement == null)
            {
                throw new ArgumentNullException(nameof(reviewElement));
            }

            dataBaseContext.reviews.Update(reviewElement);
            await dataBaseContext.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var review = await dataBaseContext.reviews.FindAsync(id);
            if (review != null)
            {
                dataBaseContext.reviews.Remove(review);
                await dataBaseContext.SaveChangesAsync();
            }
        }
    }
}