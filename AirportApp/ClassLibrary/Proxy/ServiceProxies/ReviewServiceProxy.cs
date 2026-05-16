using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interfaces;

namespace AirportApp.ClassLibrary.Proxy.ServiceProxies
{
    public class ReviewServiceProxy : IReviewService
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/review";

        public ReviewServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<Review> GetByIdAsync(int identificationNumber)
        {
            try
            {
                Review review = await httpClient.GetFromJsonAsync<Review>($"{BaseUrl}/{identificationNumber}");
                if (review == null)
                {
                    throw new KeyNotFoundException($"Review with id {identificationNumber} was not found.");
                }

                return review;
            }
            catch (HttpRequestException httpRequestException) when (httpRequestException.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new KeyNotFoundException($"Review with id {identificationNumber} was not found.");
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Server communication error while retrieving review {identificationNumber}.", httpRequestException);
            }
        }

        public async Task<int> AddAsync(Review review)
        {
            try
            {
                var reviewCreationData = new CreateReviewDTO(
                    review.User.Id,
                    review.Message,
                    review.DutyFreeRating,
                    review.FlightExperienceRating,
                    review.StaffFriendlinessRating,
                    review.CleanlinessRating);

                HttpResponseMessage response = await httpClient.PostAsJsonAsync(BaseUrl, reviewCreationData);
                response.EnsureSuccessStatusCode();

                Review createdReview = await response.Content.ReadFromJsonAsync<Review>();
                return createdReview?.Id ?? 0;
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException("Failed to add review through the service proxy.", httpRequestException);
            }
        }

        public async Task UpdateByIdAsync(int identificationNumber, Review review)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{identificationNumber}", review);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to update review {identificationNumber}.", httpRequestException);
            }
        }

        public async Task DeleteByIdAsync(int identificationNumber)
        {
            try
            {
                HttpResponseMessage response = await httpClient.DeleteAsync($"{BaseUrl}/{identificationNumber}");
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to delete review {identificationNumber}.", httpRequestException);
            }
        }

        public async Task<List<Review>?> GetAllAsync()
        {
            try
            {
                List<Review> reviews = await httpClient.GetFromJsonAsync<List<Review>>(BaseUrl);
                return reviews ?? new List<Review>();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException("Failed to retrieve all reviews.", httpRequestException);
            }
        }

        public Task CreateReviewAsync(int identificationNumber, User user, string message, int dutyFreeRating, int flightExperienceRating, int staffFriendlinessRating, int cleanlinessRating)
        {
            throw new NotSupportedException("CreateReviewAsync is not available through the service proxy. Use AddAsync instead.");
        }

        public Task ValidateReviewAsync(Review review)
        {
            throw new NotSupportedException("ValidateReviewAsync is not available through the service proxy.");
        }

        public float CalculateAverageRating(Review review)
        {
            throw new NotSupportedException("CalculateAverageRating is not available through the service proxy.");
        }
    }
}
