using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Domain.Review;
using AirportApp.Src.Service;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace AirportApp.Src.Proxy
{
    public class ReviewServiceProxy : IReviewService
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/review";

        public ReviewServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<Review> GetByIdAsync(int identificationNumber)
            => await httpClient.GetFromJsonAsync<Review>($"{BaseUrl}/{identificationNumber}");

        public async Task<int> AddAsync(Review review)
        {
            var response = await httpClient.PostAsJsonAsync(BaseUrl, review);
            response.EnsureSuccessStatusCode();
            return review.Id;
        }

        public async Task UpdateByIdAsync(int identificationNumber, Review review)
        {
            var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{identificationNumber}", review);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteByIdAsync(int identificationNumber)
        {
            var response = await httpClient.DeleteAsync($"{BaseUrl}/{identificationNumber}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<Review>?> GetAllAsync()
            => await httpClient.GetFromJsonAsync<List<Review>>(BaseUrl);

        public async Task CreateReviewAsync(int identificationNumber, User user, string message,
            int dutyFreeRating, int flightExperienceRating, int staffFriendlinessRating, int cleanlinessRating)
        {
            // logica ramane locala
            Review review = new(identificationNumber, user, message, dutyFreeRating,
                flightExperienceRating, staffFriendlinessRating, cleanlinessRating);
            await ValidateReviewAsync(review);
            await AddAsync(review);
        }

        public async Task ValidateReviewAsync(Review review)
        {
            // logica ramane locala
            ArgumentNullException.ThrowIfNull(review);
            var allReviews = await GetAllAsync();
            if (allReviews != null && allReviews.Contains(review))
                throw new ArgumentException("Review already exists");
            if (review.User == null)
                throw new ArgumentException("User cannot be null");
            if (string.IsNullOrEmpty(review.Message))
                throw new ArgumentException("Message cannot be null or empty");
        }

        public float CalculateAverageRating(Review review)
        {
            // logica pura locala
            return (review.DutyFreeRating + review.FlightExperienceRating +
                    review.StaffFriendlinessRating + review.CleanlinessRating) / 4f;
        }
    }
}