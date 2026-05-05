using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.Src.Service.Interfaces;

namespace AirportApp.Src.Proxy
{
    public class UserServiceProxy : IUserService
    {
        private const string BaseUrl = "api/user";
        private readonly HttpClient httpClient;

        public UserServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<User> GetByIdAsync(int identificationNumber)
        {
            var user = await httpClient.GetFromJsonAsync<User>($"{BaseUrl}/{identificationNumber}");
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {identificationNumber} not found.");
            }
            return user;
        }

        public async Task<int> AddUserAsync(User userEntity)
        {
            HttpResponseMessage response = await httpClient.PostAsJsonAsync(BaseUrl, userEntity);
            response.EnsureSuccessStatusCode();

            // The controller returns CreatedAtAction. We need to extract the returned User
            // to get the database-assigned ID, or we assume the ID is returned if it was modified.
            // Based on UserController: return CreatedAtAction(nameof(GetByIdAsync), new { id = createdId }, user);
            var createdUser = await response.Content.ReadFromJsonAsync<User>();
            return createdUser?.Id ?? userEntity.Id;
        }

        public async Task UpdateUserByIdAsync(int identificationNumber, User userEntity)
        {
            HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{identificationNumber}", userEntity);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteUserByIdAsync(int identificationNumber)
        {
            HttpResponseMessage response = await httpClient.DeleteAsync($"{BaseUrl}/{identificationNumber}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = await httpClient.GetFromJsonAsync<IEnumerable<User>>(BaseUrl);
            return users?.ToList() ?? new List<User>();
        }

        // Pure logic combining other methods
        public async Task CreateNewUserAsync(int identificationNumber, string fullName, string emailAddress)
        {
            User user = new User(identificationNumber, fullName, emailAddress);
            await ValidateUserIntegrityAsync(user);
            await AddUserAsync(user);
        }

        // Pure logic combining other methods
        public async Task ValidateUserIntegrityAsync(User userEntity)
        {
            ArgumentNullException.ThrowIfNull(userEntity);
            
            var allUsers = await this.GetAllUsersAsync();
            if (allUsers.Any(u => u.Id == userEntity.Id)) // Simplified check vs .Contains to avoid reference issues
            {
                throw new ArgumentException("User already exists");
            }
            
            if (string.IsNullOrEmpty(userEntity.RetrieveConfiguredDisplayFullNameForBot()))
            {
                throw new ArgumentException("Name cannot be null or empty");
            }
            
            if (string.IsNullOrEmpty(userEntity.RetrieveConfiguredEmailAddressForBotContact()))
            {
                throw new ArgumentException("Email cannot be null or empty");
            }
        }
    }
}
