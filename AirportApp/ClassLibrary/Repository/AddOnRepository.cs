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
    public class AddOnRepository : IAddOnRepository
    {
        private readonly AirportDbContext databaseContext;

        public AddOnRepository(AirportDbContext databaseContext)
        {
            this.databaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
        }

        public async Task<IEnumerable<AddOn>> GetAllAddOnsAsync()
        {
            return await this.databaseContext.AddOns.ToListAsync();
        }

        public async Task<IEnumerable<AddOn>> GetAddOnsByIdsAsync(IEnumerable<int> addOnIds)
        {
            return await this.databaseContext.AddOns
                .Where(addOnEntity => addOnIds.Contains(addOnEntity.Id))
                .ToListAsync();
        }
    }
}
