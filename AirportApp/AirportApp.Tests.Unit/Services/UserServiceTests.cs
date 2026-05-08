using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.Src.ViewModel;
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
        private IUserRepository userRepository;
        private UserService userService;

        [TestInitialize]
        public void Setup()
        {
            userRepository = Substitute.For<IUserRepository>();
            userService = new UserService(userRepository);

            var initialUsers = new List<User>
            {
                new User(1, "Ion Popescu", "ion@test.com"),
                new User(2, "Maria Ioana", "maria@test.com")
            };
            userRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<User>)initialUsers));
        }

        [TestMethod]
        public async Task GetById_ExistingId_ReturnsUserFromRepository()
        {
            var expectedUser = new User(1, "Ion Popescu", "ion@test.com");
            userRepository.GetByIdAsync(1).Returns(Task.FromResult(expectedUser));

            var resultedUser = await userService.GetByIdAsync(1);

            Assert.AreEqual(expectedUser, resultedUser);
            await userRepository.Received(1).GetByIdAsync(1);
        }

        [TestMethod]
        public async Task GetAllUsers_WhenCalled_ReturnsAllEntities()
        {
            var resultedUser = await userService.GetAllUsersAsync();

            Assert.AreEqual(2, resultedUser.Count);
            Assert.AreEqual("Ion Popescu", resultedUser[0].RetrieveConfiguredDisplayFullNameForBot());
            Assert.AreEqual("Maria Ioana", resultedUser[1].RetrieveConfiguredDisplayFullNameForBot());
        }

        [TestMethod]
        public async Task CreateNewUser_WithValidData_CallsRepository()
        {
            int identificationNumber = 10;
            string fullName = "New User";
            string emailAddress = "new@test.com";

            await userService.CreateNewUserAsync(identificationNumber, fullName, emailAddress);

            await userRepository.Received(1).CreateNewEntityAsync(Arg.Is<User>(user => user.RetrieveUniqueDatabaseIdentifierForBot() == identificationNumber));
        }

        [TestMethod]
        public async Task ValidateUserIntegrity_WithNullUser_ThrowsArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
                await userService.ValidateUserIntegrityAsync(null!));
        }

        [TestMethod]
        public async Task ValidateUserIntegrity_ForDuplicateUser_ThrowsArgumentException()
        {
            var existingUser = (await userService.GetAllUsersAsync()).First();

            var exceptionThrown = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await userService.ValidateUserIntegrityAsync(existingUser));

            StringAssert.Contains("User already exists", exceptionThrown.Message);
        }

        [TestMethod]
        public async Task ValidateUserIntegrity_WithEmptyName_ThrowsArgumentException()
        {
            var userWithEmptyName = new User(1, string.Empty, "email@test.com");

            var exceptionThrown = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await userService.ValidateUserIntegrityAsync(userWithEmptyName));

            StringAssert.Contains("Name cannot be null or empty", exceptionThrown.Message);
        }

        [TestMethod]
        public async Task ValidateUserIntegrity_WithEmptyEmail_ThrowsArgumentException()
        {
            var userWithEmptyEmail = new User(1, "Nume Valid", string.Empty);

            var exceptionThrown = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await userService.ValidateUserIntegrityAsync(userWithEmptyEmail));

            StringAssert.Contains("Email cannot be null or empty", exceptionThrown.Message);
        }

        [TestMethod]
        public async Task DeleteUserById_WhenCalled_CallsRepository()
        {
            await userService.DeleteUserByIdAsync(100);

            await userRepository.Received(1).DeleteByIdAsync(100);
        }

        [TestMethod]
        public async Task UpdateUserById_WithCorrectData_CallsRepository()
        {
            int identificationNumber = 1;
            var updatedUser = new User(identificationNumber, "Nume Actualizat", "updated@test.com");

            await userService.UpdateUserByIdAsync(identificationNumber, updatedUser);

            await userRepository.Received(1).UpdateByIdAsync(identificationNumber, updatedUser);
        }
    }
}
