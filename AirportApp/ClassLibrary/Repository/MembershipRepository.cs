using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<Membership?> GetMembershipByIdAsync(int id)
        {
            return await this.dataBaseContext.memberships
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Membership>> GetAllMembershipsAsync()
        {
            return await this.dataBaseContext.memberships.ToListAsync();
        }

        public async Task<IEnumerable<MembershipAddonDiscount>> GetAddonDiscountsAsync(int membershipId)
        {
            var membership = await this.dataBaseContext.memberships
                .FirstOrDefaultAsync(m => m.Id == membershipId);

            if (membership == null)
            {
                return new List<MembershipAddonDiscount>();
            }

            var addOns = await this.dataBaseContext.addOns.ToListAsync();

            var addOnDiscounts = addOns
                .Select(a => new MembershipAddonDiscount(membership, a, 10f))
                .ToList();

            return addOnDiscounts;
        }
    }
}
