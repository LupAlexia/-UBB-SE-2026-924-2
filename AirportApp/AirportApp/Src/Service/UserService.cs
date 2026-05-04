using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.Src.Service.Interfaces;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.Src.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;

        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<User> GetByIdAsync(int identificationNumber)
        {
            return await userRepository.GetByIdAsync(identificationNumber);
        }

        public async Task<int> AddUserAsync(User user)
        {
            return await userRepository.CreateNewEntityAsync(user);
        }

        public async Task UpdateUserByIdAsync(int identificationNumber, User userEntity)
        {
            await userRepository.UpdateByIdAsync(identificationNumber, userEntity);
        }

        public async Task DeleteUserByIdAsync(int identificationNumber)
        {
            await userRepository.DeleteByIdAsync(identificationNumber);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return (await userRepository.GetAllAsync()).ToList();
        }

        public async Task CreateNewUserAsync(int identificationNumber, string fullName, string emailAddress)
        {
            User user = new User(identificationNumber, fullName, emailAddress);
            await ValidateUserIntegrityAsync(user);
            await AddUserAsync(user);
        }

        public async Task ValidateUserIntegrityAsync(User userEntity)
        {
            ArgumentNullException.ThrowIfNull(userEntity);
            if ((await this.GetAllUsersAsync()).Contains(userEntity))
            {
                throw new ArgumentException("User already exists");
            }
            if (string.IsNullOrEmpty(userEntity.RetrieveConfiguredDisplayFullNameForBot()))
            {
                throw new ArgumentException("Name cannot be null or empty");
            }
            if (string.IsNullOrEmpty(userEntity.RetrieveConfiguredEmailAddressForBotContact()))
            {
                throw new ArgumentException("Email cannot be null or empty");
            }
        }
    }
}