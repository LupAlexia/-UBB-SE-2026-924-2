using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Faq;
using AirportApp.Src.Service.Interfaces;

namespace AirportApp.Src.Proxy
{
    public class FAQServiceProxy : IFAQService
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/faq";

        public FAQServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<FAQEntry>> GetAllAsync()
        {
            return await httpClient.GetFromJsonAsync<List<FAQEntry>>(BaseUrl);
        }

        public async Task<List<FAQEntry>> GetByCategoryAsync(FAQCategoryEnum category)
        {
            return await httpClient.GetFromJsonAsync<List<FAQEntry>>($"{BaseUrl}/by-category?category={category}");
        }

        public async Task AddFAQEntryAsync(FAQEntry newElem)
        {
            var response = await httpClient.PostAsJsonAsync(BaseUrl, newElem);
            response.EnsureSuccessStatusCode();
        }

        public async Task EditFAQEntryAsync(FAQEntry tempEntry, int faqEntryId)
        {
            var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{faqEntryId}", tempEntry);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteFAQEntryAsync(int entryId)
        {
            var response = await httpClient.DeleteAsync($"{BaseUrl}/{entryId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task IncrementViewCountAsync(FAQEntry entry)
        {
            var response = await httpClient.PostAsync($"{BaseUrl}/{entry.Id}/increment-view", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task IncrementWasHelpfulVotesAsync(FAQEntry entry)
        {
            var response = await httpClient.PostAsync($"{BaseUrl}/{entry.Id}/increment-helpful", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task IncrementWasNotHelpfulVotesAsync(FAQEntry entry)
        {
            var response = await httpClient.PostAsync($"{BaseUrl}/{entry.Id}/increment-not-helpful", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<FAQEntry>> FilterFAQEntryAsync(FAQCategoryEnum category, string searchQuery)
        {
            // logica pura locala — filtreaza in memorie dupa ce ia datele din API
            IEnumerable<FAQEntry> entries;

            if (category != FAQCategoryEnum.All)
            {
                entries = await GetByCategoryAsync(category);
            }
            else
            {
                entries = await GetAllAsync();
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                entries = entries.Where(q =>
                    (q.Question?.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (q.Answer?.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            return entries.ToList();
        }
    }
}