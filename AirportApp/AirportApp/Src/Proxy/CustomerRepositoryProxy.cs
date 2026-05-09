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
                var dto = await httpClient.GetFromJsonAsync<AirportApp.ClassLibrary.Entity.Dto.CustomerDTO>($"{BaseUrl}/{id}");
                if (dto == null) return null;

                return new Customer
                {
                    Id = dto.id,
                    Email = dto.email,
                    Phone = dto.phone,
                    Username = dto.username,
                    PasswordHash = dto.passwordHash,
                    MembershipId = dto.membershipId,
                    Membership = dto.membership != null ? new Membership
                    {
                        Id = dto.membership.id,
                        Name = dto.membership.name,
                        FlightDiscountPercentage = dto.membership.flightDiscountPercentage
                    } : null
                };
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Server communication error while retrieving customer {id}.", ex);
            }
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            try
            {
                var dto = await httpClient.GetFromJsonAsync<AirportApp.ClassLibrary.Entity.Dto.CustomerDTO>($"{BaseUrl}/by-email?email={Uri.EscapeDataString(email)}");
                if (dto == null) return null;

                return new Customer
                {
                    Id = dto.id,
                    Email = dto.email,
                    Phone = dto.phone,
                    Username = dto.username,
                    PasswordHash = dto.passwordHash,
                    MembershipId = dto.membershipId,
                    Membership = dto.membership != null ? new Membership
                    {
                        Id = dto.membership.id,
                        Name = dto.membership.name,
                        FlightDiscountPercentage = dto.membership.flightDiscountPercentage
                    } : null
                };
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // 404 means the user isn't there, which is a VALID result for registration!
                return null;
            }
            catch (HttpRequestException ex)
            {
                // Any other error (like the server being down) should still be thrown
                throw new InvalidOperationException("Server communication error.", ex);
            }
        }


        public async Task AddUserAsync(Customer user)
        {
            try
            {
                var dto = new AirportApp.ClassLibrary.Entity.Dto.CustomerDTO(
                    user.Id,
                    user.Email,
                    user.Phone,
                    user.Username,
                    user.PasswordHash,
                    user.MembershipId);

                var response = await httpClient.PostAsJsonAsync(BaseUrl, dto);
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