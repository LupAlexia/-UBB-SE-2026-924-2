using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository
{
    public class ReviewRepository : IRepository<int, Review>
    {
        private readonly AirportDbContext databaseContext;

        public ReviewRepository(AirportDbContext databaseContext)
        {
            this.databaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
        }

        public async Task<Review> GetByIdAsync(int id)
        {
            return await databaseContext.Reviews
                .Include(review => review.User)
                .FirstOrDefaultAsync(review => review.Id == id)
                ?? throw new KeyNotFoundException($"Review with id {id} was not found.");
        }

        public async Task<IEnumerable<Review>> GetAllAsync()
        {
            return await databaseContext.Reviews
                .Include(review => review.User)
                .ToListAsync();
        }

        public async Task<int> CreateNewEntityAsync(Review reviewElement)
        {
            if (reviewElement == null)
            {
                throw new ArgumentNullException(nameof(reviewElement));
            }

            databaseContext.Reviews.Add(reviewElement);
            await databaseContext.SaveChangesAsync();
            return reviewElement.Id;
        }

        public async Task UpdateByIdAsync(int id, Review reviewElement)
        {
            if (reviewElement == null)
            {
                throw new ArgumentNullException(nameof(reviewElement));
            }

            databaseContext.Reviews.Update(reviewElement);
            await databaseContext.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var review = await databaseContext.Reviews.FindAsync(id);
            if (review != null)
            {
                databaseContext.Reviews.Remove(review);
                await databaseContext.SaveChangesAsync();
            }
        }
    }
}