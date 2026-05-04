using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.DataAccess;

namespace AirportApp.ClassLibrary.Repository
{
    public class MembershipRepository : IMembershipRepository
    {
        private readonly AirportDbContext dataBaseContext;

        public MembershipRepository(AirportDbContext dataBaseContext)
        {
            this.dataBaseContext = dataBaseContext ?? throw new ArgumentNullException(nameof(dataBaseContext));
        }

        public Membership? GetMembershipById(int id)
        {
            return this.dataBaseContext.memberships
                .FirstOrDefault(m => m.Id == id);
        }

        public IEnumerable<Membership> GetAllMemberships()
        {
            return this.dataBaseContext.memberships.ToList();
        }

        public IEnumerable<MembershipAddonDiscount> GetAddonDiscounts(int membershipId)
        {
            var discounts = new List<MembershipAddonDiscount>();
            
            var membership = this.dataBaseContext.memberships
                .FirstOrDefault(m => m.Id == membershipId);
            
            if (membership == null)
            {
                return discounts;
            }

            var addOnDiscounts = this.dataBaseContext.addOns
                .ToList() 
                .Select(a => new MembershipAddonDiscount(membership, a, 10f))
                .ToList();

            return addOnDiscounts;
        }
    }
}
