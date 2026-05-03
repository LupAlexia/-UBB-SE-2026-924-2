using System.Collections.Generic;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interfaces
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

