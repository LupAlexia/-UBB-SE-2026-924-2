using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.Src.Proxy
{
    public class UserRepositoryProxy : IUserRepository, IRepository<int, User>
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/user";

        public UserRepositoryProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await httpClient.GetFromJsonAsync<IEnumerable<User>>(BaseUrl)
                   ?? new List<User>();
        }

        public async Task<User> GetByIdAsync(int identificationNumber)
        {
            try
            {
                return await httpClient.GetFromJsonAsync<User>($"{BaseUrl}/{identificationNumber}")
                       ?? throw new KeyNotFoundException($"User with id {identificationNumber} was not found.");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new KeyNotFoundException($"User with id {identificationNumber} was not found.");
            }
        }

        public async Task<int> CreateNewEntityAsync(User userEntity)
        {
            var response = await httpClient.PostAsJsonAsync(BaseUrl, userEntity);
            response.EnsureSuccessStatusCode();
            var created = await response.Content.ReadFromJsonAsync<User>();
            return created!.Id;
        }

        public async Task UpdateByIdAsync(int identificationNumber, User userEntity)
        {
            var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{identificationNumber}", userEntity);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteByIdAsync(int identificationNumber)
        {
            var response = await httpClient.DeleteAsync($"{BaseUrl}/{identificationNumber}");
            response.EnsureSuccessStatusCode();
        }
    }
}