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
            return await this.dataBaseContext.Memberships
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Membership>> GetAllMembershipsAsync()
        {
            return await this.dataBaseContext.Memberships.ToListAsync();
        }

        public async Task<IEnumerable<MembershipAddonDiscount>> GetAddonDiscountsAsync(int membershipId)
        {
            return await this.dataBaseContext.MembershipAddonDiscounts
                .Include(d => d.AddOn)
                .Where(d => d.MembershipId == membershipId)
                .ToListAsync();
        }
    }
}
