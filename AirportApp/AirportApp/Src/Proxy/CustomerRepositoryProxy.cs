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
                var customerTransferObject = await httpClient.GetFromJsonAsync<AirportApp.ClassLibrary.Entity.Dto.CustomerDTO>($"{BaseUrl}/{id}");
                if (customerTransferObject == null)
                {
                    return null;
                }

                return new Customer
                {
                    Id = customerTransferObject.id,
                    Email = customerTransferObject.email,
                    Phone = customerTransferObject.phone,
                    Username = customerTransferObject.username,
                    PasswordHash = customerTransferObject.passwordHash,
                    Membership = customerTransferObject.membership != null ? new Membership
                    {
                        Id = customerTransferObject.membership.id,
                        Name = customerTransferObject.membership.name,
                        FlightDiscountPercentage = customerTransferObject.membership.flightDiscountPercentage
                    }
                    : null
                };
            }
            catch (HttpRequestException httpRequestException) when (httpRequestException.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Server communication error while retrieving customer {id}.", httpRequestException);
            }
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            try
            {
                var customerTransferObject = await httpClient.GetFromJsonAsync<AirportApp.ClassLibrary.Entity.Dto.CustomerDTO>($"{BaseUrl}/by-email?email={Uri.EscapeDataString(email)}");
                if (customerTransferObject == null)
                {
                    return null;
                }

                return new Customer
                {
                    Id = customerTransferObject.id,
                    Email = customerTransferObject.email,
                    Phone = customerTransferObject.phone,
                    Username = customerTransferObject.username,
                    PasswordHash = customerTransferObject.passwordHash,
                    Membership = customerTransferObject.membership != null ? new Membership
                    {
                        Id = customerTransferObject.membership.id,
                        Name = customerTransferObject.membership.name,
                        FlightDiscountPercentage = customerTransferObject.membership.flightDiscountPercentage
                    }
                    : null
                };
            }
            catch (HttpRequestException httpRequestException) when (httpRequestException.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // 404 means the user isn't there, which is a VALID result for registration!
                return null;
            }
            catch (HttpRequestException httpRequestException)
            {
                // Any other error (like the server being down) should still be thrown
                throw new InvalidOperationException("Server communication error.", httpRequestException);
            }
        }

        public async Task AddUserAsync(Customer user)
        {
            try
            {
                var customerTransferObject = new AirportApp.ClassLibrary.Entity.Dto.CustomerDTO(
                    user.Id,
                    user.Email,
                    user.Phone,
                    user.Username,
                    user.PasswordHash,
                    user.Membership?.Id,
                    null);

                var response = await httpClient.PostAsJsonAsync(BaseUrl, customerTransferObject);
                response.EnsureSuccessStatusCode();

                var createdCustomerTransferObject = await response.Content.ReadFromJsonAsync<AirportApp.ClassLibrary.Entity.Dto.CustomerDTO>();
                if (createdCustomerTransferObject != null)
                {
                    user.Id = createdCustomerTransferObject.id;
                }
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException("Failed to add customer to server.", httpRequestException);
            }
        }

        public async Task UpdateUserMembershipAsync(int userId, int newMembershipId)
        {
            try
            {
                var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{userId}/membership", newMembershipId);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to update membership for customer {userId}.", httpRequestException);
            }
        }
    }
}