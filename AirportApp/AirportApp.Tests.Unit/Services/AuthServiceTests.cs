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

        private ICustomerRepository _mockRepository;
        private AuthService _authService;
        private PasswordHasher<Customer> _passwordHasher;

        [TestInitialize]
        public void Setup()
        {
            _mockRepository = Substitute.For<ICustomerRepository>();
            _authService = new AuthService(_mockRepository);
            _passwordHasher = new PasswordHasher<Customer>();
        }

        [TestMethod]
        public async Task Login_ValidUser_ReturnsCustomer()
        {
            var customer = new Customer { Email = "andrei.ionescu@gmail.com" };
            customer.PasswordHash = _passwordHasher.HashPassword(customer, ValidPassword);
            _mockRepository.GetByEmailAsync(customer.Email).Returns(customer);

            var result = await _authService.LoginAsync(customer.Email, ValidPassword);

            Assert.IsNotNull(result);
            Assert.AreEqual(customer.Email, result.Email);
        }

        [TestMethod]
        public async Task Login_InvalidPassword_ThrowsException()
        {
            var customer = new Customer { Email = "george.popa@yahoo.ro" };
            customer.PasswordHash = _passwordHasher.HashPassword(customer, ValidSecretPassword);
            _mockRepository.GetByEmailAsync(customer.Email).Returns(customer);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _authService.LoginAsync(customer.Email, InvalidPassword));
        }

        [TestMethod]
        public async Task Login_UserNotFound_ThrowsException()
        {
            _mockRepository.GetByEmailAsync(Arg.Any<string>()).Returns((Customer?)null);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _authService.LoginAsync("nonexistent@gmail.com", ValidPassword));
        }

        [TestMethod]
        public async Task Login_NullEmail_ThrowsException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => _authService.LoginAsync(null!, ValidPassword));
        }

        [TestMethod]
        public async Task Login_EmptyEmail_ThrowsException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => _authService.LoginAsync(string.Empty, ValidPassword));
        }

        [TestMethod]
        public async Task Login_NullPassword_ThrowsException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => _authService.LoginAsync(ValidEmail, null!));
        }

        [TestMethod]
        public async Task Login_EmptyPassword_ThrowsException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => _authService.LoginAsync(ValidEmail, string.Empty));
        }

        [TestMethod]
        public async Task Register_DuplicateEmail_ThrowsException()
        {
            string email = "bogdan.stefan@gmail.com";
            _mockRepository.GetByEmailAsync(email).Returns(new Customer { Email = email });

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _authService.RegisterAsync(email, ValidPhoneNumber, "bogdan_s", "ParolaBogdan!"));
        }

        [TestMethod]
        public async Task Register_InvalidEmailFormat_ThrowsException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => _authService.RegisterAsync("mariusPaguba", ValidPhoneNumber, "marius", "Parola1"));
        }

        [TestMethod]
        public async Task Register_ValidUser_CreatesNewUser()
        {
            string email = "gabriela.stan@yahoo.ro";
            _mockRepository.GetByEmailAsync(email).Returns((Customer?)null);

            await _authService.RegisterAsync(email, ValidAlternatePhone, "gabriela_s", "ParolaGabriela123!");

            await _mockRepository.Received(1).AddUserAsync(Arg.Is<Customer>(c => c.Email == email));
        }

        [TestMethod]
        public async Task Register_PasswordTooShort_ThrowsException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => _authService.RegisterAsync(ValidEmail, ValidPhoneNumber, ValidUsername, "12345"));
        }

        [TestMethod]
        public async Task Register_UsernameTooShort_ThrowsException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => _authService.RegisterAsync(ValidEmail, ValidPhoneNumber, "ab", "ValidPass1"));
        }

        [TestMethod]
        public async Task Register_InvalidUsername_ThrowsException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => _authService.RegisterAsync(ValidEmail, ValidPhoneNumber, "user@#$", "ValidPass1"));
        }

        [TestMethod]
        public async Task Register_NullPhone_ThrowsException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => _authService.RegisterAsync(ValidEmail, null!, ValidUsername, "ValidPass1"));
        }

        [TestMethod]
        public async Task Register_InvalidPhone_ThrowsException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => _authService.RegisterAsync(ValidEmail, InvalidPhoneFormat, ValidUsername, "ValidPass1"));
        }
    }
}



