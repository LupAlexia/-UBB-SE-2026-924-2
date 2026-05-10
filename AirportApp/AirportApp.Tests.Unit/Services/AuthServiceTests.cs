using System;
using System.Threading.Tasks;
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
        private const string InvalidShortPhone = "123";
        private const string ValidEmail = "test@gmail.com";
        private const string ValidUsername = "user123";
        private const string TooShortUsername = "ab";
        private const string TooShortPassword = "12345";
        private const string ExactMinLengthPassword = "abc123";
        private const string ExactMinLengthUsername = "abc";
        private const string UsernameWithUnderscore = "user_name";
        private const string UsernameWithSpace = "user name";
        private const string InvalidUsernameChars = "user@#$";
        private const string InvalidEmailNoAt = "mariusPaguba";
        private const string DuplicateEmail = "bogdan.stefan@gmail.com";
        private const string DuplicateUsername = "bogdan_s";
        private const string DuplicatePassword = "ParolaBogdan!";
        private const string ValidNewEmail = "gabriela.stan@yahoo.ro";
        private const string ValidNewUsername = "gabriela_s";
        private const string ValidNewPassword = "ParolaGabriela123!";
        private const string HashTestEmail = "hash.test@yahoo.ro";
        private const string HashTestUsername = "hashtest";
        private const string ExactPassEmail = "exact.pass@test.com";
        private const string ExactUserEmail = "exact.user@test.com";
        private const string UnderscoreEmail = "underscore@test.com";
        private const string SpaceEmail = "space@test.com";
        private const string NonExistentEmail = "nonexistent@gmail.com";
        private const string LoginEmail1 = "andrei.ionescu@gmail.com";
        private const string LoginEmail2 = "george.popa@yahoo.ro";

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
            var customer = new Customer { Email = LoginEmail1 };
            customer.PasswordHash = passwordHasher.HashPassword(customer, ValidPassword);
            mockRepository.GetByEmailAsync(customer.Email).Returns(customer);

            var result = await authService.LoginAsync(customer.Email, ValidPassword);

            Assert.IsNotNull(result);
            Assert.AreEqual(customer.Email, result.Email);
        }

        [TestMethod]
        public async Task LoginAsync_InvalidPassword_ThrowsException()
        {
            var customer = new Customer { Email = LoginEmail2 };
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
                () => authService.LoginAsync(NonExistentEmail, ValidPassword));
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
        public async Task LoginAsync_WhitespaceEmail_ThrowsException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => authService.LoginAsync("   ", ValidPassword));
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
        public async Task LoginAsync_EmailWithWhitespace_TrimsAndFindsUser()
        {
            var customer = new Customer { Email = ValidEmail };
            customer.PasswordHash = passwordHasher.HashPassword(customer, ValidPassword);
            mockRepository.GetByEmailAsync(ValidEmail).Returns(customer);

            var result = await authService.LoginAsync("  " + ValidEmail + "  ", ValidPassword);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task RegisterAsync_DuplicateEmail_ThrowsException()
        {
            mockRepository.GetByEmailAsync(DuplicateEmail).Returns(new Customer { Email = DuplicateEmail });

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => authService.RegisterAsync(DuplicateEmail, ValidPhoneNumber, DuplicateUsername, DuplicatePassword));
        }

        [TestMethod]
        public async Task RegisterAsync_InvalidEmailFormat_ThrowsException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => authService.RegisterAsync(InvalidEmailNoAt, ValidPhoneNumber, ExactMinLengthUsername, ExactMinLengthPassword));
        }

        [TestMethod]
        public async Task RegisterAsync_NullEmail_ThrowsException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => authService.RegisterAsync(null!, ValidPhoneNumber, ValidUsername, ValidPassword));
        }

        [TestMethod]
        public async Task RegisterAsync_ValidUser_CreatesNewUser()
        {
            mockRepository.GetByEmailAsync(ValidNewEmail).Returns((Customer?)null);

            await authService.RegisterAsync(ValidNewEmail, ValidAlternatePhone, ValidNewUsername, ValidNewPassword);

            await mockRepository.Received(1).AddUserAsync(Arg.Is<Customer>(customer => customer.Email == ValidNewEmail));
        }

        [TestMethod]
        public async Task RegisterAsync_ValidUser_HashesPassword()
        {
            mockRepository.GetByEmailAsync(HashTestEmail).Returns((Customer?)null);

            await authService.RegisterAsync(HashTestEmail, ValidPhoneNumber, HashTestUsername, ValidPassword);

            await mockRepository.Received(1).AddUserAsync(Arg.Is<Customer>(c => c.PasswordHash != ValidPassword && !string.IsNullOrEmpty(c.PasswordHash)));
        }

        [TestMethod]
        public async Task RegisterAsync_PasswordTooShort_ThrowsException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => authService.RegisterAsync(ValidEmail, ValidPhoneNumber, ValidUsername, TooShortPassword));
        }

        [TestMethod]
        public async Task RegisterAsync_PasswordExactlyMinLength_Succeeds()
        {
            mockRepository.GetByEmailAsync(ExactPassEmail).Returns((Customer?)null);

            await authService.RegisterAsync(ExactPassEmail, ValidPhoneNumber, ValidUsername, ExactMinLengthPassword);

            await mockRepository.Received(1).AddUserAsync(Arg.Any<Customer>());
        }

        [TestMethod]
        public async Task RegisterAsync_UsernameTooShort_ThrowsException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => authService.RegisterAsync(ValidEmail, ValidPhoneNumber, TooShortUsername, ValidPassword));
        }

        [TestMethod]
        public async Task RegisterAsync_UsernameExactlyMinLength_Succeeds()
        {
            mockRepository.GetByEmailAsync(ExactUserEmail).Returns((Customer?)null);

            await authService.RegisterAsync(ExactUserEmail, ValidPhoneNumber, ExactMinLengthUsername, ValidPassword);

            await mockRepository.Received(1).AddUserAsync(Arg.Any<Customer>());
        }

        [TestMethod]
        public async Task RegisterAsync_InvalidUsername_ThrowsException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => authService.RegisterAsync(ValidEmail, ValidPhoneNumber, InvalidUsernameChars, ValidPassword));
        }

        [TestMethod]
        public async Task RegisterAsync_UsernameWithUnderscore_Succeeds()
        {
            mockRepository.GetByEmailAsync(UnderscoreEmail).Returns((Customer?)null);

            await authService.RegisterAsync(UnderscoreEmail, ValidPhoneNumber, UsernameWithUnderscore, ValidPassword);

            await mockRepository.Received(1).AddUserAsync(Arg.Any<Customer>());
        }

        [TestMethod]
        public async Task RegisterAsync_UsernameWithSpace_Succeeds()
        {
            mockRepository.GetByEmailAsync(SpaceEmail).Returns((Customer?)null);

            await authService.RegisterAsync(SpaceEmail, ValidPhoneNumber, UsernameWithSpace, ValidPassword);

            await mockRepository.Received(1).AddUserAsync(Arg.Any<Customer>());
        }

        [TestMethod]
        public async Task RegisterAsync_NullPhone_ThrowsException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => authService.RegisterAsync(ValidEmail, null!, ValidUsername, ValidPassword));
        }

        [TestMethod]
        public async Task RegisterAsync_InvalidPhone_ThrowsException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => authService.RegisterAsync(ValidEmail, InvalidPhoneFormat, ValidUsername, ValidPassword));
        }

        [TestMethod]
        public async Task RegisterAsync_PhoneTooShort_ThrowsException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => authService.RegisterAsync(ValidEmail, InvalidShortPhone, ValidUsername, ValidPassword));
        }

        [TestMethod]
        public async Task RegisterAsync_NullUsername_ThrowsException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => authService.RegisterAsync(ValidEmail, ValidPhoneNumber, null!, ValidPassword));
        }

        [TestMethod]
        public async Task RegisterAsync_NullPassword_ThrowsException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => authService.RegisterAsync(ValidEmail, ValidPhoneNumber, ValidUsername, null!));
        }
    }
}