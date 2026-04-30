using System.Collections.Generic;
using AirportApp.Src.Domain;

namespace AirportApp.Src.Repository
{
    public interface IAddOnRepository
    {
        IEnumerable<AddOn> GetAllAddOns();
        IEnumerable<AddOn> GetAddOnsByIds(IEnumerable<int> ids);
    }
}
