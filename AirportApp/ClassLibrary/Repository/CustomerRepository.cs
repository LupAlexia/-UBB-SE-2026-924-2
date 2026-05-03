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
            this.dataBaseContext = dataBaseContext;
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

            var membership = this.membershipRepository.GetMembershipById(newMembershipId);
            userToUpdate.Membership = membership;
            this.dataBaseContext.Entry(userToUpdate).State = EntityState.Modified;
            dataBaseContext.SaveChanges();
        }

        private Customer MapUser(SqlDataReader reader)
        {
            int membershipIdOrdinal = reader.GetOrdinal("membership_id");
            Membership? membership = null;

            if (!reader.IsDBNull(membershipIdOrdinal))
            {
                membership = new Membership
                {
                    MembershipId = reader.GetInt32(membershipIdOrdinal),
                    Name = reader.GetString(reader.GetOrdinal("membership_name")),
                    FlightDiscountPercentage = (float)reader.GetByte(reader.GetOrdinal("flight_discount_percentage"))
                };

                membership.AddonDiscounts = this.membershipRepository.GetAddonDiscounts(membership.MembershipId).ToList();
            }

            return new Customer(
                reader.GetInt32(reader.GetOrdinal("user_id")),
                reader.GetString(reader.GetOrdinal("email")),
                reader.IsDBNull(reader.GetOrdinal("phone")) ? null : reader.GetString(reader.GetOrdinal("phone")),
                reader.GetString(reader.GetOrdinal("username")),
                reader.GetString(reader.GetOrdinal("password_hash")),
                membership);
        }
    }
}
