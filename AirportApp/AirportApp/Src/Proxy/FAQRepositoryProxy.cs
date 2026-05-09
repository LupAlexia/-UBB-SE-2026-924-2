using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.Src.Proxy
{
    public class FAQRepositoryProxy : IFAQRepository
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/faq";

        public FAQRepositoryProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<FAQEntry>> GetAllAsync()
        {
            return await httpClient.GetFromJsonAsync<IEnumerable<FAQEntry>>(BaseUrl)
                   ?? new List<FAQEntry>();
        }

        public async Task<FAQEntry> GetByIdAsync(int id)
        {
            return await httpClient.GetFromJsonAsync<FAQEntry>($"{BaseUrl}/{id}")
                   ?? throw new KeyNotFoundException($"FAQ with id {id} was not found.");
        }

        public async Task<List<FAQEntry>> GetByCategoryAsync(FAQCategoryEnum category)
        {
            return await httpClient.GetFromJsonAsync<List<FAQEntry>>($"{BaseUrl}/by-category?category={category}")
                   ?? new List<FAQEntry>();
        }

        public async Task<int> CreateNewEntityAsync(FAQEntry entry)
        {
            var response = await httpClient.PostAsJsonAsync(BaseUrl, entry);
            response.EnsureSuccessStatusCode();
            var created = await response.Content.ReadFromJsonAsync<FAQEntry>();
            return created!.Id;
        }

        public async Task UpdateByIdAsync(int id, FAQEntry entry)
        {
            var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", entry);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var response = await httpClient.DeleteAsync($"{BaseUrl}/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task IncrementViewCountAsync(int id)
        {
            var response = await httpClient.PostAsync($"{BaseUrl}/{id}/increment-view", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task IncrementWasHelpfulVotesAsync(int id)
        {
            var response = await httpClient.PostAsync($"{BaseUrl}/{id}/increment-helpful", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task IncrementWasNotHelpfulVotesAsync(int id)
        {
            var response = await httpClient.PostAsync($"{BaseUrl}/{id}/increment-not-helpful", null);
            response.EnsureSuccessStatusCode();
        }
    }
}