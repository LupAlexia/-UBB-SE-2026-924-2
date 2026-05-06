using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.Src.Service.Interfaces;

namespace AirportApp.Src.Proxy
{
    public class UserServiceProxy : IUserService
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/user";

        public UserServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<User> GetByIdAsync(int identificationNumber)
        {
            return await httpClient.GetFromJsonAsync<User>($"{BaseUrl}/{identificationNumber}");
        }

        public async Task<int> AddUserAsync(User userEntity)
        {
            var response = await httpClient.PostAsJsonAsync(BaseUrl, userEntity);
            response.EnsureSuccessStatusCode();
            return userEntity.Id;
        }

        public async Task UpdateUserByIdAsync(int identificationNumber, User userEntity)
        {
            var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{identificationNumber}", userEntity);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteUserByIdAsync(int identificationNumber)
        {
            var response = await httpClient.DeleteAsync($"{BaseUrl}/{identificationNumber}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await httpClient.GetFromJsonAsync<List<User>>(BaseUrl);
        }

        public async Task CreateNewUserAsync(int identificationNumber, string fullName, string emailAddress)
        {
            // logica ramane locala, construim user-ul si il trimitem la API
            User user = new User(identificationNumber, fullName, emailAddress);
            await ValidateUserIntegrityAsync(user);
            await AddUserAsync(user);
        }

        public async Task ValidateUserIntegrityAsync(User userEntity)
        {
            // logica pura de validare, ramane locala
            ArgumentNullException.ThrowIfNull(userEntity);

            if ((await GetAllUsersAsync()).Contains(userEntity))
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