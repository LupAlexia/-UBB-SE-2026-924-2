using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interfaces;

namespace AirportApp.ClassLibrary.Proxy.ServiceProxies
{
    public class AuthServiceProxy : IAuthService
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/customer";

        public AuthServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public Task<Customer> LoginAsync(string email, string password, int? currentUserId = null)
        {
            throw new NotSupportedException("LoginAsync is handled locally and is not available through the service proxy.");
        }

        public Task RegisterAsync(string email, string phone, string username, string password)
        {
            throw new NotSupportedException("RegisterAsync is handled locally and is not available through the service proxy.");
        }

        public void Logout()
        {
            throw new NotSupportedException("Logout is handled locally and is not available through the service proxy.");
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            try
            {
                CustomerDTO customerTransferObject = await httpClient.GetFromJsonAsync<CustomerDTO>($"{BaseUrl}/{id}");
                if (customerTransferObject == null)
                {
                    return null;
                }

                return MapCustomerFromTransferObject(customerTransferObject);
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
                CustomerDTO customerTransferObject = await httpClient.GetFromJsonAsync<CustomerDTO>($"{BaseUrl}/by-email?email={Uri.EscapeDataString(email)}");
                if (customerTransferObject == null)
                {
                    return null;
                }

                return MapCustomerFromTransferObject(customerTransferObject);
            }
            catch (HttpRequestException httpRequestException) when (httpRequestException.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException("Server communication error while retrieving customer by email.", httpRequestException);
            }
        }

        public async Task AddUserAsync(Customer customer)
        {
            try
            {
                var customerTransferObject = new CustomerDTO(
                    customer.Id,
                    customer.Email,
                    customer.Phone,
                    customer.Username,
                    customer.PasswordHash,
                    customer.Membership?.Id,
                    null);

                HttpResponseMessage response = await httpClient.PostAsJsonAsync(BaseUrl, customerTransferObject);
                response.EnsureSuccessStatusCode();

                CustomerDTO createdCustomerTransferObject = await response.Content.ReadFromJsonAsync<CustomerDTO>();
                if (createdCustomerTransferObject != null)
                {
                    customer.Id = createdCustomerTransferObject.id;
                }
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException("Failed to add customer through the service proxy.", httpRequestException);
            }
        }

        private static Customer MapCustomerFromTransferObject(CustomerDTO customerTransferObject)
        {
            return new Customer
            {
                Id = customerTransferObject.id,
                Email = customerTransferObject.email,
                Phone = customerTransferObject.phone,
                Username = customerTransferObject.username,
                PasswordHash = customerTransferObject.passwordHash,
                Membership = customerTransferObject.membership != null
                    ? new Membership
                    {
                        Id = customerTransferObject.membership.id,
                        Name = customerTransferObject.membership.name,
                        FlightDiscountPercentage = customerTransferObject.membership.flightDiscountPercentage
                    }
                    : null
            };
        }
    }
}
