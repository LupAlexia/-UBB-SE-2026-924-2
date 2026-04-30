using System.Collections.Generic;
using AirportApp.Src.Model;

namespace AirportApp.Src.Service.Interfaces
{
    public interface IUserService
    {
        User GetById(int identificationNumber);
        int AddUser(User userEntity);
        void UpdateUserById(int identificationNumber, User userEntity);
        void DeleteUserById(int identificationNumber);
        List<User> GetAllUsers();
        void CreateNewUser(int identificationNumber, string fullName, string emailAddress);
        void ValidateUserIntegrity(User userEntity);
    }
}