using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interfaces;

namespace AirportApp.ClassLibrary.Proxy.ServiceProxies
{
    public class UserServiceProxy : IUserService
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/user";

        public UserServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<User> GetByIdAsync(int identificationNumber)
        {
            try
            {
                User user = await httpClient.GetFromJsonAsync<User>($"{BaseUrl}/{identificationNumber}");
                if (user == null)
                {
                    throw new KeyNotFoundException($"User with id {identificationNumber} was not found.");
                }

                return user;
            }
            catch (HttpRequestException httpRequestException) when (httpRequestException.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new KeyNotFoundException($"User with id {identificationNumber} was not found.");
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Server communication error while retrieving user {identificationNumber}.", httpRequestException);
            }
        }

        public async Task<int> AddUserAsync(User userEntity)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PostAsJsonAsync(BaseUrl, userEntity);
                response.EnsureSuccessStatusCode();

                User createdUser = await response.Content.ReadFromJsonAsync<User>();
                if (createdUser != null)
                {
                    return createdUser.Id;
                }

                return userEntity.Id;
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException("Failed to add user through the service proxy.", httpRequestException);
            }
        }

        public async Task UpdateUserByIdAsync(int identificationNumber, User userEntity)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{identificationNumber}", userEntity);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to update user {identificationNumber}.", httpRequestException);
            }
        }

        public async Task DeleteUserByIdAsync(int identificationNumber)
        {
            try
            {
                HttpResponseMessage response = await httpClient.DeleteAsync($"{BaseUrl}/{identificationNumber}");
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to delete user {identificationNumber}.", httpRequestException);
            }
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                List<User> users = await httpClient.GetFromJsonAsync<List<User>>(BaseUrl);
                return users ?? new List<User>();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException("Failed to retrieve all users.", httpRequestException);
            }
        }

        public Task CreateNewUserAsync(int identificationNumber, string fullName, string emailAddress)
        {
            throw new NotSupportedException("CreateNewUserAsync is not available through the service proxy.");
        }

        public Task ValidateUserIntegrityAsync(User userEntity)
        {
            throw new NotSupportedException("ValidateUserIntegrityAsync is not available through the service proxy.");
        }
    }
}
