using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.DataAccess;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AirportDbContext databaseContext;
        private readonly IMembershipRepository membershipRepository;

        public CustomerRepository(AirportDbContext databaseContext, IMembershipRepository membershipRepository)
        {
            this.databaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
            this.membershipRepository = membershipRepository ?? throw new ArgumentNullException(nameof(membershipRepository));
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await this.databaseContext.Customers
                .Include(customer => customer.Membership)
                .FirstOrDefaultAsync(customer => customer.Id == id);
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            return await this.databaseContext.Customers
                .Include(customer => customer.Membership)
                .FirstOrDefaultAsync(customer => customer.Email == email);
        }

        public async Task AddUserAsync(Customer user)
        {
            this.databaseContext.Add(user);
            await this.databaseContext.SaveChangesAsync();
        }

        public async Task UpdateUserMembershipAsync(int userId, int newMembershipId)
        {
            var userToUpdate = await this.databaseContext.Customers
                .Include(customer => customer.Membership)
                .FirstOrDefaultAsync(customer => customer.Id == userId);

            if (userToUpdate == null)
            {
                throw new KeyNotFoundException("User with id " + userId + " not found.");
            }

            var membership = await this.membershipRepository.GetMembershipByIdAsync(newMembershipId);
            userToUpdate.Membership = membership;
            this.databaseContext.Entry(userToUpdate).State = EntityState.Modified;
            await this.databaseContext.SaveChangesAsync();
        }
    }
}
