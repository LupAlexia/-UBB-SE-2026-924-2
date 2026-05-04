using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository
{
    public class UserRepository : IUserRepository, AirportApp.ClassLibrary.Repository.Interfaces.IRepository<int, User>
    {
        private readonly AirportDbContext dataBaseContext;

        public UserRepository(AirportDbContext context)
        {
            dataBaseContext = context ?? throw new ArgumentNullException(nameof(dataBaseContext));
        }

        public int CreateNewEntity(User userEntity)
        {
            dataBaseContext.users.Add(userEntity);
            dataBaseContext.SaveChanges();
            return userEntity.Id;   
        }

        public void DeleteById(int identificationNumber)
        {
            var user = dataBaseContext.users.Find(identificationNumber);
            if (user != null)
            {
                dataBaseContext.users.Remove(user);
                dataBaseContext.SaveChanges();
            }
        }

        public IEnumerable<User> GetAll()
        {
            
            return dataBaseContext.users.ToList();
        }

        public User GetById(int identificationNumber)
        {
            return dataBaseContext.users.Find(identificationNumber)
                   ?? throw new KeyNotFoundException($"User with id {identificationNumber} was not found.");
        }

        public void UpdateById(int identificationNumber, User userEntity)
        {
           
            dataBaseContext.users.Update(userEntity);
            dataBaseContext.SaveChanges();
        }
    }
}