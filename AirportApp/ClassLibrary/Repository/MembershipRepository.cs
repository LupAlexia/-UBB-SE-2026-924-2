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
        private readonly AirportDbContext databaseContext;

        public MembershipRepository(AirportDbContext databaseContext)
        {
            this.databaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
        }

        public async Task<Membership?> GetMembershipByIdAsync(int id)
        {
            return await this.databaseContext.Memberships
                .FirstOrDefaultAsync(membershipEntity => membershipEntity.Id == id);
        }

        public async Task<IEnumerable<Membership>> GetAllMembershipsAsync()
        {
            return await this.databaseContext.Memberships.ToListAsync();
        }

        public async Task<IEnumerable<MembershipAddonDiscount>> GetAddonDiscountsAsync(int membershipId)
        {
            return await this.databaseContext.MembershipAddonDiscounts
                .Include(discount => discount.AddOn)
                .Where(discount => EF.Property<int>(discount, "MembershipId") == membershipId)
                .ToListAsync();
        }
    }
}
