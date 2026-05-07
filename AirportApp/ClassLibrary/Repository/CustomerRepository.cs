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
        private readonly AirportDbContext dataBaseContext;
        private readonly IMembershipRepository membershipRepository;

        public CustomerRepository(AirportDbContext dataBaseContext, IMembershipRepository membershipRepository)
        {
            this.dataBaseContext = dataBaseContext ?? throw new ArgumentNullException(nameof(dataBaseContext));
            this.membershipRepository = membershipRepository ?? throw new ArgumentNullException(nameof(membershipRepository));
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await this.dataBaseContext.Customers
                .Include(customer => customer.Membership)
                .FirstOrDefaultAsync(customer => customer.Id == id);
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            return await this.dataBaseContext.Customers
                .Include(customer => customer.Membership)
                .FirstOrDefaultAsync(customer => customer.Email == email);
        }

        public async Task AddUserAsync(Customer user)
        {
            this.dataBaseContext.Add(user);
            await this.dataBaseContext.SaveChangesAsync();
        }

        public async Task UpdateUserMembershipAsync(int userId, int newMembershipId)
        {
            var userToUpdate = await this.dataBaseContext.Customers
                .Include(customer => customer.Membership)
                .FirstOrDefaultAsync(customer => customer.Id == userId);

            if (userToUpdate == null)
            {
                throw new KeyNotFoundException("User with id " + userId + " not found.");
            }

            var membership = await this.membershipRepository.GetMembershipByIdAsync(newMembershipId);
            userToUpdate.Membership = membership;
            this.dataBaseContext.Entry(userToUpdate).State = EntityState.Modified;
            await this.dataBaseContext.SaveChangesAsync();
        }
    }
}
