using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interfaces;

namespace AirportApp.ClassLibrary.Proxy.ServiceProxies
{
    public class FAQServiceProxy : IFAQService
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/faq";

        public FAQServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<List<FAQEntry>> GetAllAsync()
        {
            try
            {
                List<FAQEntry> entries = await httpClient.GetFromJsonAsync<List<FAQEntry>>(BaseUrl);
                return entries ?? new List<FAQEntry>();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException("Failed to retrieve all FAQ entries.", httpRequestException);
            }
        }

        public async Task<List<FAQEntry>> GetByCategoryAsync(FAQCategoryEnum category)
        {
            try
            {
                List<FAQEntry> entries = await httpClient.GetFromJsonAsync<List<FAQEntry>>($"{BaseUrl}/by-category?category={category}");
                return entries ?? new List<FAQEntry>();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to retrieve FAQ entries for category {category}.", httpRequestException);
            }
        }

        public async Task AddFAQEntryAsync(FAQEntry newElem)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PostAsJsonAsync(BaseUrl, newElem);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException("Failed to add FAQ entry.", httpRequestException);
            }
        }

        public async Task EditFAQEntryAsync(FAQEntry tempEntry, int faqEntryId)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{faqEntryId}", tempEntry);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to update FAQ entry {faqEntryId}.", httpRequestException);
            }
        }

        public async Task DeleteFAQEntryAsync(int entryId)
        {
            try
            {
                HttpResponseMessage response = await httpClient.DeleteAsync($"{BaseUrl}/{entryId}");
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to delete FAQ entry {entryId}.", httpRequestException);
            }
        }

        public async Task IncrementViewCountAsync(FAQEntry entry)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PostAsync($"{BaseUrl}/{entry.Id}/increment-view", null);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to increment view count for FAQ entry {entry.Id}.", httpRequestException);
            }
        }

        public async Task IncrementWasHelpfulVotesAsync(FAQEntry entry)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PostAsync($"{BaseUrl}/{entry.Id}/increment-helpful", null);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to increment helpful votes for FAQ entry {entry.Id}.", httpRequestException);
            }
        }

        public async Task IncrementWasNotHelpfulVotesAsync(FAQEntry entry)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PostAsync($"{BaseUrl}/{entry.Id}/increment-not-helpful", null);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to increment not-helpful votes for FAQ entry {entry.Id}.", httpRequestException);
            }
        }

        public Task<List<FAQEntry>> FilterFAQEntryAsync(FAQCategoryEnum category, string searchQuery)
        {
            throw new NotSupportedException("FilterFAQEntryAsync is not available through the service proxy.");
        }
    }
}
