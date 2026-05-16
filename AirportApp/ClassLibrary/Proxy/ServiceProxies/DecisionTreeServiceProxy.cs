using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interfaces;

namespace AirportApp.ClassLibrary.Proxy.ServiceProxies
{
    public class DecisionTreeServiceProxy : IDecisionTreeService
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/decisiontree";

        public DecisionTreeServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<IEnumerable<FAQNode>> GetAllNodesAsync()
        {
            try
            {
                IEnumerable<FAQNode> nodes = await httpClient.GetFromJsonAsync<IEnumerable<FAQNode>>(BaseUrl);
                return nodes ?? new List<FAQNode>();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException("Failed to retrieve all decision tree nodes.", httpRequestException);
            }
        }

        public async Task<FAQNode> GetNodeByIdAsync(int id)
        {
            try
            {
                FAQNode node = await httpClient.GetFromJsonAsync<FAQNode>($"{BaseUrl}/{id}");
                if (node == null)
                {
                    throw new KeyNotFoundException($"Decision tree node with id {id} was not found.");
                }

                return node;
            }
            catch (HttpRequestException httpRequestException) when (httpRequestException.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new KeyNotFoundException($"Decision tree node with id {id} was not found.");
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Server communication error while retrieving node {id}.", httpRequestException);
            }
        }

        public async Task<int> CreateNodeAsync(FAQNode node)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PostAsJsonAsync(BaseUrl, node);
                response.EnsureSuccessStatusCode();

                FAQNode createdNode = await response.Content.ReadFromJsonAsync<FAQNode>();
                return createdNode?.NodeId ?? 0;
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException("Failed to create decision tree node.", httpRequestException);
            }
        }

        public async Task UpdateNodeAsync(int id, FAQNode node)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", node);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to update decision tree node {id}.", httpRequestException);
            }
        }

        public async Task DeleteNodeAsync(int id)
        {
            try
            {
                HttpResponseMessage response = await httpClient.DeleteAsync($"{BaseUrl}/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to delete decision tree node {id}.", httpRequestException);
            }
        }
    }
}
