using System.Collections.Generic;
using AirportApp.Src.Model;

namespace AirportApp.Src.Repository.Interfaces
{
    public interface IUserRepository
    {
        int CreateNewEntity(User userEntity);
        void DeleteById(int identificationNumber);
        IEnumerable<User> GetAll();
        User GetById(int identificationNumber);
        void UpdateById(int identificationNumber, User userEntity);
        void UpdateUserMembership(int userId, int newMembershipId);
        void AddUserWithMembership(User user);
        User? GetByIdWithMembership(int id);
        IEnumerable<User> GetAllMemberships();
    }
}