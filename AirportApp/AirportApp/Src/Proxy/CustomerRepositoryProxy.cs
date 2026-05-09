using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.Src.Proxy
{
    public class CustomerRepositoryProxy : ICustomerRepository
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/customer";

        public CustomerRepositoryProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            try
            {
                return await httpClient.GetFromJsonAsync<Customer>($"{BaseUrl}/{id}");
            }
            catch (HttpRequestException ex)
            {
                throw new KeyNotFoundException($"Customer with id {id} not found.", ex);
            }
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            try
            {
                return await httpClient.GetFromJsonAsync<Customer>($"{BaseUrl}/by-email?email={Uri.EscapeDataString(email)}");
            }
            catch (HttpRequestException ex)
            {
                throw new KeyNotFoundException($"Customer with email {email} not found.", ex);
            }
        }

        public async Task AddUserAsync(Customer user)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync(BaseUrl, user);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException("Failed to add customer to server.", ex);
            }
        }

        public async Task UpdateUserMembershipAsync(int userId, int newMembershipId)
        {
            try
            {
                var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{userId}/membership", newMembershipId);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to update membership for customer {userId}.", ex);
            }
        }
    }
}