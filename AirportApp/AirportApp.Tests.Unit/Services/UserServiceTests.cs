using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirportApp.Src.Service;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.Entity.Domain;
using NSubstitute;

namespace AirportApp.Tests.Unit.Src.Service
{
    [TestClass]
    public class UserServiceTests
    {
        private const int UserId1 = 1;
        private const int UserId2 = 2;
        private const int NewUserId = 99;
        private const int CreateUserId = 10;
        private const string User1Name = "Ion Popescu";
        private const string User1Email = "ion@test.com";
        private const string User2Name = "Maria Ioana";
        private const string User2Email = "maria@test.com";
        private const string NewUserName = "Brand New";
        private const string NewUserEmail = "brandnew@test.com";
        private const string CreateUserName = "New User";
        private const string CreateUserEmail = "new@test.com";
        private const string EmptyNameUserEmail = "email@test.com";
        private const string ValidNameForEmptyEmailTest = "Nume Valid";

        private IUserRepository userRepository;
        private UserService userService;
        private User existingUser1;
        private User existingUser2;

        [TestInitialize]
        public void Setup()
        {
            userRepository = Substitute.For<IUserRepository>();
            userService = new UserService(userRepository);

            // Store references so tests can reuse the exact same objects
            existingUser1 = new User(UserId1, User1Name, User1Email);
            existingUser2 = new User(UserId2, User2Name, User2Email);

            var initialUsers = new List<User> { existingUser1, existingUser2 };
            userRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<User>)initialUsers));
        }

        [TestMethod]
        public async Task GetAllUsers_WhenCalled_ReturnsAllEntities()
        {
            var result = await userService.GetAllUsersAsync();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(User1Name, result[0].RetrieveConfiguredDisplayFullNameForBot());
            Assert.AreEqual(User2Name, result[1].RetrieveConfiguredDisplayFullNameForBot());
        }

        [TestMethod]
        public async Task CreateNewUser_WithValidData_CallsRepository()
        {
            await userService.CreateNewUserAsync(CreateUserId, CreateUserName, CreateUserEmail);

            await userRepository.Received(1).CreateNewEntityAsync(Arg.Is<User>(u => u.RetrieveUniqueDatabaseIdentifierForBot() == CreateUserId));
        }

        [TestMethod]
        public async Task CreateNewUser_WithInvalidData_DoesNotCallRepository()
        {
            // Use empty name to trigger validation failure reliably
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => userService.CreateNewUserAsync(NewUserId, string.Empty, NewUserEmail));

            await userRepository.DidNotReceive().CreateNewEntityAsync(Arg.Any<User>());
        }

        [TestMethod]
        public async Task ValidateUserIntegrity_WithNullUser_ThrowsArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => userService.ValidateUserIntegrityAsync(null!));
        }

        [TestMethod]
        public async Task ValidateUserIntegrity_ForDuplicateUser_ThrowsArgumentException()
        {
            var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => userService.ValidateUserIntegrityAsync(existingUser1));

            StringAssert.Contains("User already exists", ex.Message);
        }

        [TestMethod]
        public async Task ValidateUserIntegrity_WithEmptyName_ThrowsArgumentException()
        {
            var userWithEmptyName = new User(NewUserId, string.Empty, EmptyNameUserEmail);

            var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => userService.ValidateUserIntegrityAsync(userWithEmptyName));

            StringAssert.Contains("Name cannot be null or empty", ex.Message);
        }

        [TestMethod]
        public async Task ValidateUserIntegrity_WithEmptyEmail_ThrowsArgumentException()
        {
            var userWithEmptyEmail = new User(NewUserId, ValidNameForEmptyEmailTest, string.Empty);

            var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => userService.ValidateUserIntegrityAsync(userWithEmptyEmail));

            StringAssert.Contains("Email cannot be null or empty", ex.Message);
        }

        [TestMethod]
        public async Task ValidateUserIntegrity_WithValidUniqueUser_DoesNotThrow()
        {
            var newUser = new User(NewUserId, NewUserName, NewUserEmail);

            await userService.ValidateUserIntegrityAsync(newUser);
        }
    }
}