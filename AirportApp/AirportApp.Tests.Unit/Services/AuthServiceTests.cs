using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using AirportApp.Src.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Microsoft.AspNetCore.Identity;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service;

namespace AirportApp.Tests.Unit
{
    [TestClass]
    public class AuthServiceTests
    {
        private const string ValidPassword = "ParolaAndrei2024!";
        private const string ValidSecretPassword = "parola_secreta_99";
        private const string InvalidPassword = "parola_incorecta_99";
        private const string ValidPhoneNumber = "0722334455";
        private const string ValidAlternatePhone = "0744112233";
        private const string InvalidPhoneFormat = "072211-2233";
        private const string ValidEmail = "test@gmail.com";
        private const string ValidUsername = "user123";

        private ICustomerRepository mockRepository;
        private AuthService authService;
        private PasswordHasher<Customer> passwordHasher;

        [TestInitialize]
        public void Setup()
        {
            mockRepository = Substitute.For<ICustomerRepository>();
            authService = new AuthService(mockRepository);
            passwordHasher = new PasswordHasher<Customer>();
        }

        [TestMethod]
        public async Task LoginAsync_ValidUser_ReturnsCustomer()
        {
            var customer = new Customer { Email = "andrei.ionescu@gmail.com" };
            customer.PasswordHash = passwordHasher.HashPassword(customer, ValidPassword);
            mockRepository.GetByEmailAsync(customer.Email).Returns(customer);

            var result = await authService.LoginAsync(customer.Email, ValidPassword);

            Assert.IsNotNull(result);
            Assert.AreEqual(customer.Email, result.Email);
        }

        [TestMethod]
        public async Task LoginAsync_InvalidPassword_ThrowsException()
        {
            var customer = new Customer { Email = "george.popa@yahoo.ro" };
            customer.PasswordHash = passwordHasher.HashPassword(customer, ValidSecretPassword);
            mockRepository.GetByEmailAsync(customer.Email).Returns(customer);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => authService.LoginAsync(customer.Email, InvalidPassword));
        }

        [TestMethod]
        public async Task LoginAsync_UserNotFound_ThrowsException()
        {
            mockRepository.GetByEmailAsync(Arg.Any<string>()).Returns((Customer?)null);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => authService.LoginAsync("nonexistent@gmail.com", ValidPassword));
        }

        [TestMethod]
        public async Task LoginAsync_NullEmail_ThrowsException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => authService.LoginAsync(null!, ValidPassword));
        }

        [TestMethod]
        public async Task LoginAsync_EmptyEmail_ThrowsException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => authService.LoginAsync(string.Empty, ValidPassword));
        }

        [TestMethod]
        public async Task LoginAsync_NullPassword_ThrowsException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => authService.LoginAsync(ValidEmail, null!));
        }

        [TestMethod]
        public async Task LoginAsync_EmptyPassword_ThrowsException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => authService.LoginAsync(ValidEmail, string.Empty));
        }

        [TestMethod]
        public async Task RegisterAsync_DuplicateEmail_ThrowsException()
        {
            string email = "bogdan.stefan@gmail.com";
            mockRepository.GetByEmailAsync(email).Returns(new Customer { Email = email });

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => authService.RegisterAsync(email, ValidPhoneNumber, "bogdan_s", "ParolaBogdan!"));
        }

        [TestMethod]
        public async Task RegisterAsync_InvalidEmailFormat_ThrowsException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => authService.RegisterAsync("mariusPaguba", ValidPhoneNumber, "marius", "Parola1"));
        }

        [TestMethod]
        public async Task RegisterAsync_ValidUser_CreatesNewUser()
        {
            string email = "gabriela.stan@yahoo.ro";
            mockRepository.GetByEmailAsync(email).Returns((Customer?)null);

            await authService.RegisterAsync(email, ValidAlternatePhone, "gabriela_s", "ParolaGabriela123!");

            await mockRepository.Received(1).AddUserAsync(Arg.Is<Customer>(customer => customer.Email == email));
        }

        [TestMethod]
        public async Task RegisterAsync_PasswordTooShort_ThrowsException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => authService.RegisterAsync(ValidEmail, ValidPhoneNumber, ValidUsername, "12345"));
        }

        [TestMethod]
        public async Task RegisterAsync_UsernameTooShort_ThrowsException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => authService.RegisterAsync(ValidEmail, ValidPhoneNumber, "ab", "ValidPass1"));
        }

        [TestMethod]
        public async Task RegisterAsync_InvalidUsername_ThrowsException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => authService.RegisterAsync(ValidEmail, ValidPhoneNumber, "user@#$", "ValidPass1"));
        }

        [TestMethod]
        public async Task RegisterAsync_NullPhone_ThrowsException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => authService.RegisterAsync(ValidEmail, null!, ValidUsername, "ValidPass1"));
        }

        [TestMethod]
        public async Task RegisterAsync_InvalidPhone_ThrowsException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => authService.RegisterAsync(ValidEmail, InvalidPhoneFormat, ValidUsername, "ValidPass1"));
        }
    }
}
