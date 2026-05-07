using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
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
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<List<FAQEntry>> GetAllAsync()
        {
            try
            {
                var result = await httpClient.GetFromJsonAsync<List<FAQEntry>>(BaseUrl);
                return result ?? new List<FAQEntry>();
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException("Failed to retrieve FAQ entries from server.", ex);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Server returned invalid FAQ data format.", ex);
            }
        }

        public async Task<List<FAQEntry>> GetByCategoryAsync(FAQCategoryEnum category)
        {
            try
            {
                var result = await httpClient.GetFromJsonAsync<List<FAQEntry>>($"{BaseUrl}/by-category?category={category}");
                return result ?? new List<FAQEntry>();
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to retrieve FAQ entries for category {category}.", ex);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Server returned invalid FAQ data format.", ex);
            }
        }

        public async Task AddFAQEntryAsync(FAQEntry newElem)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync(BaseUrl, newElem);
                await HandleResponseAsync(response, nameof(AddFAQEntryAsync));
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException("Failed to add FAQ entry to server.", ex);
            }
        }

        public async Task EditFAQEntryAsync(FAQEntry tempEntry, int faqEntryId)
        {
            try
            {
                var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{faqEntryId}", tempEntry);
                await HandleResponseAsync(response, nameof(EditFAQEntryAsync));
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to edit FAQ entry with ID {faqEntryId}.", ex);
            }
        }

        public async Task DeleteFAQEntryAsync(int entryId)
        {
            try
            {
                var response = await httpClient.DeleteAsync($"{BaseUrl}/{entryId}");
                await HandleResponseAsync(response, nameof(DeleteFAQEntryAsync));
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to delete FAQ entry with ID {entryId}.", ex);
            }
        }

        public async Task IncrementViewCountAsync(FAQEntry entry)
        {
            try
            {
                var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{BaseUrl}/{entry.Id}/increment-view", content);
                await HandleResponseAsync(response, nameof(IncrementViewCountAsync));
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to increment view count for FAQ entry {entry.Id}.", ex);
            }
        }

        public async Task IncrementWasHelpfulVotesAsync(FAQEntry entry)
        {
            try
            {
                var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{BaseUrl}/{entry.Id}/increment-helpful", content);
                await HandleResponseAsync(response, nameof(IncrementWasHelpfulVotesAsync));
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to increment helpful votes for FAQ entry {entry.Id}.", ex);
            }
        }

        public async Task IncrementWasNotHelpfulVotesAsync(FAQEntry entry)
        {
            try
            {
                var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{BaseUrl}/{entry.Id}/increment-not-helpful", content);
                await HandleResponseAsync(response, nameof(IncrementWasNotHelpfulVotesAsync));
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to increment not-helpful votes for FAQ entry {entry.Id}.", ex);
            }
        }

        public async Task<List<FAQEntry>> FilterFAQEntryAsync(FAQCategoryEnum category, string searchQuery)
        {
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

        private async Task HandleResponseAsync(HttpResponseMessage response, string methodName)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            var errorMessage = $"{methodName} failed with status code {response.StatusCode}: {errorContent}";
            throw new HttpRequestException(errorMessage, null, response.StatusCode);
        }
    }
}