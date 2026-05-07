using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Domain.Review;

namespace AirportApp.Src.Service
{
    public interface IReviewService
    {
        Task<Review> GetByIdAsync(int identificationNumber);
        Task<int> AddAsync(Review review);
        Task UpdateByIdAsync(int identificationNumber, Review review);
        Task DeleteByIdAsync(int identificationNumber);
        Task<List<Review>?> GetAllAsync();
        Task CreateReviewAsync(int identificationNumber, User user, string message, int dutyFreeRating, int flightExperienceRating, int staffFriendlinessRating, int cleanlinessRating);
        Task ValidateReviewAsync(Review review);
        float CalculateAverageRating(Review review);
    }
}
