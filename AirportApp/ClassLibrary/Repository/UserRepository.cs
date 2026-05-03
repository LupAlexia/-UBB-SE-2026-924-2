using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AirportDbContext _context;

        public UserRepository(AirportDbContext context)
        {
            _context = context;
        }

        public int CreateNewEntity(User userEntity)
        {
            _context.users.Add(userEntity);
            _context.SaveChanges();
            return userEntity.Id;   
        }

        public void DeleteById(int identificationNumber)
        {
            var user = _context.users.Find(identificationNumber);
            if (user != null)
            {
                _context.users.Remove(user);
                _context.SaveChanges();
            }
        }

        public IEnumerable<User> GetAll()
        {
            
            return _context.users.ToList();
        }

        public User GetById(int identificationNumber)
        {
            return _context.users.Find(identificationNumber)
                   ?? throw new KeyNotFoundException($"User with id {identificationNumber} was not found.");
        }

        public void UpdateById(int identificationNumber, User userEntity)
        {
           
            _context.users.Update(userEntity);
            _context.SaveChanges();
        }
    }
}