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
            this.dataBaseContext = dataBaseContext;
        }
        public IEnumerable<AddOn> GetAllAddOns()
        {
            return this.dataBaseContext.addOns;
        }

        public IEnumerable<AddOn> GetAddOnsByIds(IEnumerable<int> ids)
        {
            return this.dataBaseContext.addOns.Where(addOnEntity => ids.Contains(addOnEntity.AddOnId));
        }
    }
}

