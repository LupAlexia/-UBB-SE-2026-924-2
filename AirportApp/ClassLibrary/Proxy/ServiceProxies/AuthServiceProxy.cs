using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AirportApp.ClassLibrary.Proxy.ServiceProxies
{
    public class AuthServiceProxy : IAuthService
    {
        private readonly HttpClient httpClient;
        private readonly PasswordHasher<Customer> passwordHasher;
        private const string BaseUrl = "api/customer";

        public AuthServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.passwordHasher = new PasswordHasher<Customer>();
        }

        public async Task<Customer> LoginAsync(string email, string password, int? currentUserId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email is required.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password is required.");
            }

            Customer existingUser = await GetByEmailAsync(email.Trim());

            if (existingUser == null)
            {
                throw new InvalidOperationException("No account found with this email.");
            }

            if (currentUserId.HasValue && existingUser.Id != currentUserId.Value)
            {
                throw new InvalidOperationException("This account does not belong to the current user.");
            }

            PasswordVerificationResult result =
                passwordHasher.VerifyHashedPassword(existingUser, existingUser.PasswordHash, password);

            if (result == PasswordVerificationResult.Failed)
            {
                throw new InvalidOperationException("Invalid email or password.");
            }

            return existingUser;
        }

        public async Task RegisterAsync(string email, string phone, string username, string password)
        {
            string normalizedEmail = email?.Trim();
            string normalizedUsername = username?.Trim();
            string normalizedPhone = phone?.Trim();

            if (string.IsNullOrWhiteSpace(normalizedEmail))
            {
                throw new ArgumentException("Email is required.");
            }

            if (string.IsNullOrWhiteSpace(normalizedUsername))
            {
                throw new ArgumentException("Username is required.");
            }

            if (string.IsNullOrWhiteSpace(normalizedPhone))
            {
                throw new ArgumentException("Phone is required.");
            }

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                throw new ArgumentException("Password must be at least 6 characters long.");
            }

            Customer existingUser = await GetByEmailAsync(normalizedEmail);
            if (existingUser != null)
            {
                throw new InvalidOperationException("An account with this email already exists.");
            }

            Customer newUser = new Customer
            {
                Email = normalizedEmail,
                Phone = normalizedPhone,
                Username = normalizedUsername,
                Membership = null
            };

            string hashedPassword = passwordHasher.HashPassword(newUser, password);
            newUser.PasswordHash = hashedPassword;

            await AddUserAsync(newUser);
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
