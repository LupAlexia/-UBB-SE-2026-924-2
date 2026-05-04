using System.Collections.Generic;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interfaces
{
    public interface IAddOnRepository
    {
        IEnumerable<AddOn> GetAllAddOns();
        IEnumerable<AddOn> GetAddOnsByIds(IEnumerable<int> ids);
    }
}
