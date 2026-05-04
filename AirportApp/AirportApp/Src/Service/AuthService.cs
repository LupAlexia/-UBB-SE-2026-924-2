using System;
using System.Linq;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.Repository;

namespace AirportApp.Src.Service
{
    public class AuthService : IAuthService
    {
        private const int MinimumUsernameLength = 3;
        private const int MinimumPasswordLength = 6;

        private readonly ICustomerRepository userRepository;
        private readonly PasswordHasher<Customer> passwordHasher;

        public AuthService(ICustomerRepository userRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.passwordHasher = new PasswordHasher<Customer>();
        }

        public Customer Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email is required.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password is required.");
            }

            Customer? existingUser = this.userRepository.GetByEmail(email.Trim());

            if (existingUser == null)
            {
                throw new InvalidOperationException("No account found with this email.");
            }

            PasswordVerificationResult passwordVerificationResult =
                this.passwordHasher.VerifyHashedPassword(existingUser, existingUser.PasswordHash, password);

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                throw new InvalidOperationException("Invalid email or password.");
            }

            return existingUser;
        }

        public void Register(string email, string phone, string username, string password)
        {
            string? normalizedEmail = email?.Trim();
            string? normalizedUsername = username?.Trim();
            string? normalizedPhone = phone?.Trim();

            this.ValidateRegistrationData(normalizedEmail, normalizedPhone, normalizedUsername, password);

            Customer? existingUser = this.userRepository.GetByEmail(normalizedEmail!);
            if (existingUser != null)
            {
                throw new InvalidOperationException("An account with this email already exists.");
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

            this.userRepository.AddUser(newUser);
        }

        public void Logout()
        {
            UserSession.CurrentUser = null;
            UserSession.PendingBookingParameters = null;
        }

        private void ValidateRegistrationData(string? email, string? phone, string? username, string password)
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
                throw new ArgumentException("Username must have at least 3 characters.");
            }

            if (!username.All(character => char.IsLetter(character) || char.IsDigit(character) || character == '_' || character == ' '))
            {
                throw new ArgumentException("Username contains invalid characters.");
            }

            if (string.IsNullOrWhiteSpace(phone))
            {
                throw new ArgumentException("Phone is required.");
            }

            if (!ValidationHelper.IsValidPhone(phone))
            {
                throw new ArgumentException("Phone number must contain only digits and have 10 to 15 digits.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password is required.");
            }

            if (password.Length < MinimumPasswordLength)
            {
                throw new ArgumentException("Password must be at least 6 characters long.");
            }
        }
    }
}

