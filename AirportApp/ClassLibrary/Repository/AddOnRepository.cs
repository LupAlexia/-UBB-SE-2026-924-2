using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.DataAccess;

namespace AirportApp.ClassLibrary.Repository
{
    public class AddOnRepository : IAddOnRepository
    {
        private readonly AirportDbContext dataBaseContext;
        public AddOnRepository(AirportDbContext databaseContext)
        {
            this.dataBaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
        }
        public IEnumerable<AddOn> GetAllAddOns()
        {
            return this.dataBaseContext.addOns;
        }

        public IEnumerable<AddOn> GetAddOnsByIds(IEnumerable<int> addOnIds)
        {
            return this.dataBaseContext.addOns.Where(addOnEntity => addOnIds.Contains(addOnEntity.Id));
        }
    }
}

