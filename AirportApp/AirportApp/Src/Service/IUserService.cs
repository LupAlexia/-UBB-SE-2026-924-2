using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Src.Service.Interfaces
{
    public interface IUserService
    {
        Task<User> GetByIdAsync(int identificationNumber);
        Task<int> AddUserAsync(User userEntity);
        Task UpdateUserByIdAsync(int identificationNumber, User userEntity);
        Task DeleteUserByIdAsync(int identificationNumber);
        Task<List<User>> GetAllUsersAsync();
        Task CreateNewUserAsync(int identificationNumber, string fullName, string emailAddress);
        Task ValidateUserIntegrityAsync(User userEntity);
    }
}