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
        private readonly AirportDbContext databaseContext;

        public UserRepository(AirportDbContext databaseContext)
        {
            this.databaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
        }

        public async Task<int> CreateNewEntityAsync(User userEntity)
        {
            databaseContext.Users.Add(userEntity);
            await databaseContext.SaveChangesAsync();
            return userEntity.Id;
        }

        public async Task DeleteByIdAsync(int identificationNumber)
        {
            var user = await databaseContext.Users.FindAsync(identificationNumber);
            if (user != null)
            {
                databaseContext.Users.Remove(user);
                await databaseContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await databaseContext.Users.ToListAsync();
        }

        public async Task<User> GetByIdAsync(int identificationNumber)
        {
            return await databaseContext.Users.FindAsync(identificationNumber)
                   ?? throw new KeyNotFoundException($"User with id {identificationNumber} was not found.");
        }

        public async Task UpdateByIdAsync(int identificationNumber, User userEntity)
        {
            databaseContext.Users.Update(userEntity);
            await databaseContext.SaveChangesAsync();
        }
    }
}