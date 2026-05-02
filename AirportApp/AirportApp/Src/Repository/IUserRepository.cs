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
    }
}