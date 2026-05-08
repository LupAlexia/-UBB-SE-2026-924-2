using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository;
using AirportApp.Src.Service;

namespace AirportApp.Src.Proxy
{
    public class AuthServiceProxy : IAuthService
    {
        private const int MinimumUsernameLength = 3;
        private const int MinimumPasswordLength = 6;
        private const string BaseUrl = "api/customer";

        private readonly HttpClient httpClient;
        private readonly PasswordHasher<Customer> passwordHasher;

        public AuthServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            this.passwordHasher = new PasswordHasher<Customer>();
        }

        public async Task<Customer> LoginAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email is required.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password is required.");
            }

            Customer? existingUser = await this.httpClient
                .GetFromJsonAsync<Customer>(
                    $"{BaseUrl}/by-email?email={Uri.EscapeDataString(email.Trim())}");

            if (existingUser == null)
            {
                throw new InvalidOperationException("No account found with this email.");
            }

            PasswordVerificationResult passwordVerificationResult =
                this.passwordHasher.VerifyHashedPassword(
                    existingUser, existingUser.PasswordHash, password);

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                throw new InvalidOperationException("Invalid email or password.");
            }

            return existingUser;
        }

        public async Task RegisterAsync(
            string email, string phone, string username, string password)
        {
            string? normalizedEmail = email?.Trim();
            string? normalizedUsername = username?.Trim();
            string? normalizedPhone = phone?.Trim();

            ValidateRegistrationData(
                normalizedEmail, normalizedPhone, normalizedUsername, password);

            HttpResponseMessage checkResponse = await this.httpClient
                .GetAsync(
                    $"{BaseUrl}/by-email?email={Uri.EscapeDataString(normalizedEmail!)}");

            if (checkResponse.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    "An account with this email already exists.");
            }

            Customer newUser = new Customer
            {
                Email = normalizedEmail!,
                Phone = normalizedPhone,
                Username = normalizedUsername!,
                Membership = null
            };

            string hashedPassword = this.passwordHasher.HashPassword(newUser, password);
            newUser.PasswordHash = hashedPassword;

            HttpResponseMessage response = await this.httpClient
                .PostAsJsonAsync(BaseUrl, newUser);
            response.EnsureSuccessStatusCode();
        }

        public void Logout()
        {
            UserSession.CurrentUser = null;
            UserSession.PendingBookingParameters = null;
        }

        private static void ValidateRegistrationData(
            string? email, string? phone, string? username, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email is required.");
            }

            if (!ValidationHelper.IsValidEmail(email))
            {
                throw new ArgumentException("Email format is invalid.");
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username is required.");
            }

            if (username.Length < MinimumUsernameLength)
            {
                throw new ArgumentException(
                    "Username must have at least 3 characters.");
            }

            if (!username.All(character =>
                char.IsLetter(character) ||
                char.IsDigit(character) ||
                character == '_' ||
                character == ' '))
            {
                throw new ArgumentException(
                    "Username contains invalid characters.");
            }

            if (string.IsNullOrWhiteSpace(phone))
            {
                throw new ArgumentException("Phone is required.");
            }

            if (!ValidationHelper.IsValidPhone(phone))
            {
                throw new ArgumentException(
                    "Phone number must contain only digits "
                    + "and have 10 to 15 digits.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password is required.");
            }

            if (password.Length < MinimumPasswordLength)
            {
                throw new ArgumentException(
                    "Password must be at least 6 characters long.");
            }
        }
    }
}
