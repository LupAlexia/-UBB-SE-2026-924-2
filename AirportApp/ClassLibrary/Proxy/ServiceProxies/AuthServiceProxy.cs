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

        public async Task<Customer> LoginAsync(string email, string password, int? currentUserId = null)
        {
            var request = new LoginRequestDTO
            {
                Email = email?.Trim() ?? string.Empty,
                Password = password ?? string.Empty,
                CurrentUserId = currentUserId
            };

            HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{BaseUrl}/login", request);

            if (!response.IsSuccessStatusCode)
            {
                string errorMessage = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException(errorMessage);
            }

            var customerDTO = await response.Content.ReadFromJsonAsync<CustomerDTO>();
            if (customerDTO == null)
            {
                throw new InvalidOperationException("Failed to read user data from server.");
            }

            return MapCustomerFromTransferObject(customerDTO);
        }

        public async Task RegisterAsync(string email, string phone, string username, string password)
        {
            var request = new RegisterRequestDTO
            {
                Email = email?.Trim() ?? string.Empty,
                Phone = phone?.Trim() ?? string.Empty,
                Username = username?.Trim() ?? string.Empty,
                Password = password ?? string.Empty
            };

            HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{BaseUrl}/register", request);

            if (!response.IsSuccessStatusCode)
            {
                string errorMessage = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException(errorMessage);
            }
        }

        public void Logout()
        {
            UserSession.CurrentUser = null;
            UserSession.PendingBookingParameters = null;
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
