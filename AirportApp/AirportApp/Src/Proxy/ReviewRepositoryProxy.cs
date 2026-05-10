using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.Entity.Dto;

namespace AirportApp.Src.Proxy
{
    public class ReviewRepositoryProxy : IRepository<int, Review>
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/review";

        public ReviewRepositoryProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<Review>> GetAllAsync()
        {
            return await httpClient.GetFromJsonAsync<IEnumerable<Review>>(BaseUrl)
                   ?? new List<Review>();
        }

        public async Task<Review> GetByIdAsync(int id)
        {
            return await httpClient.GetFromJsonAsync<Review>($"{BaseUrl}/{id}")
                   ?? throw new KeyNotFoundException($"Review with id {id} was not found.");
        }

        public async Task<int> CreateNewEntityAsync(Review reviewElement)
        {
            var reviewCreationData = new CreateReviewDTO(
                userId: reviewElement.User.Id,
                message: reviewElement.Message,
                dutyFreeRating: reviewElement.DutyFreeRating,
                flightExperienceRating: reviewElement.FlightExperienceRating,
                staffFriendlinessRating: reviewElement.StaffFriendlinessRating,
                cleanlinessRating: reviewElement.CleanlinessRating);

            var response = await httpClient.PostAsJsonAsync(BaseUrl, reviewCreationData);
            response.EnsureSuccessStatusCode();
            var created = await response.Content.ReadFromJsonAsync<Review>();
            return created!.Id;
        }

        public async Task UpdateByIdAsync(int id, Review reviewElement)
        {
            var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", reviewElement);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var response = await httpClient.DeleteAsync($"{BaseUrl}/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}