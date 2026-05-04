using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.ClassLibrary.Repository
{
    public class UserRepository : IUserRepository, AirportApp.ClassLibrary.Repository.Interfaces.IRepository<int, User>
    {
        private readonly AirportDbContext dataBaseContext;

        public UserRepository(AirportDbContext context)
        {
            dataBaseContext = context ?? throw new ArgumentNullException(nameof(dataBaseContext));
        }

        public async Task<int> CreateNewEntityAsync(User userEntity)
        {
            dataBaseContext.users.Add(userEntity);
            await dataBaseContext.SaveChangesAsync();
            return userEntity.Id;
        }

        public async Task DeleteByIdAsync(int identificationNumber)
        {
            var user = await dataBaseContext.users.FindAsync(identificationNumber);
            if (user != null)
            {
                dataBaseContext.users.Remove(user);
                await dataBaseContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await dataBaseContext.users.ToListAsync();
        }

        public async Task<User> GetByIdAsync(int identificationNumber)
        {
            return await dataBaseContext.users.FindAsync(identificationNumber)
                   ?? throw new KeyNotFoundException($"User with id {identificationNumber} was not found.");
        }

        public async Task UpdateByIdAsync(int identificationNumber, User userEntity)
        {
            dataBaseContext.users.Update(userEntity);
            await dataBaseContext.SaveChangesAsync();
        }
    }
}