using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.Src.Proxy
{
    public class DecisionTreeRepositoryProxy : IRepository<int, FAQNode>
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/decisiontree";

        public DecisionTreeRepositoryProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<FAQNode>> GetAllAsync()
        {
            var dtos = await httpClient.GetFromJsonAsync<IEnumerable<FAQNodeDto>>(BaseUrl)
                       ?? new List<FAQNodeDto>();
            return dtos.Select(MapToFAQNode);
        }

        public async Task<FAQNode> GetByIdAsync(int id)
        {
            var dto = await httpClient.GetFromJsonAsync<FAQNodeDto>($"{BaseUrl}/{id}")
                      ?? throw new KeyNotFoundException($"FAQNode with id {id} was not found.");
            return MapToFAQNode(dto);
        }

        public async Task<int> CreateNewEntityAsync(FAQNode node)
        {
            var response = await httpClient.PostAsJsonAsync(BaseUrl, node);
            response.EnsureSuccessStatusCode();
            var created = await response.Content.ReadFromJsonAsync<FAQNodeDto>();
            return created!.FaqNodeId;
        }

        public async Task UpdateByIdAsync(int id, FAQNode node)
        {
            var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", node);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var response = await httpClient.DeleteAsync($"{BaseUrl}/{id}");
            response.EnsureSuccessStatusCode();
        }

        private static FAQNode MapToFAQNode(FAQNodeDto dto)
        {
            var options = (dto.Options ?? new List<FAQOptionDto>())
                .Select(o => new FAQOption(o.Label, o.NextOptionId))
                .ToImmutableArray();
            return new FAQNode(dto.FaqNodeId, dto.QuestionText, options, dto.IsFinalAnswer);
        }

        // clase locale doar pentru deserializare
        private class FAQNodeDto
        {
            public int FaqNodeId { get; set; }
            public string QuestionText { get; set; } = string.Empty;
            public bool IsFinalAnswer { get; set; }
            public List<FAQOptionDto> Options { get; set; } = new ();
        }

        private class FAQOptionDto
        {
            public string Label { get; set; } = string.Empty;
            public int NextOptionId { get; set; }
        }
    }
}