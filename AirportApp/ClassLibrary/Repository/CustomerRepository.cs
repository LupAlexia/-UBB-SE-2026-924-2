using System;
using System.Linq;
using Microsoft.Data.SqlClient;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AirportDbContext dataBaseContext;
        private readonly IMembershipRepository membershipRepository;

        public CustomerRepository(AirportDbContext dataBaseContext, IMembershipRepository membershipRepository)
        {
            this.dataBaseContext = dataBaseContext ?? throw new ArgumentNullException(nameof(dataBaseContext)); ;
            this.membershipRepository = membershipRepository ?? throw new ArgumentNullException(nameof(membershipRepository));
        }

        public Customer? GetById(int id)
        {
            return this.dataBaseContext.customers
                .Include(customer => customer.Membership)
                .FirstOrDefault(customer => customer.UserId==id);
        }

        public Customer? GetByEmail(string email)
        {
            return this.dataBaseContext.customers
                .Include(customer => customer.Membership)
                .FirstOrDefault(customer => customer.Email == email);
        }

        public void AddUser(Customer user)
        {
            this.dataBaseContext.Add(user);
            this.dataBaseContext.SaveChanges();
        }

        public void UpdateUserMembership(int userId, int newMembershipId)
        {
            var userToUpdate = this.dataBaseContext.customers
                .Include(customer => customer.Membership)
                .FirstOrDefault(customer => customer.UserId == userId);

            if( userToUpdate == null)
            {
                throw new KeyNotFoundException("User with id " + userId + " not found.");
            }

            var membership = this.membershipRepository.GetMembershipById(newMembershipId);
            userToUpdate.Membership = membership;
            this.dataBaseContext.Entry(userToUpdate).State = EntityState.Modified;
            dataBaseContext.SaveChanges();
        }
    }
}
