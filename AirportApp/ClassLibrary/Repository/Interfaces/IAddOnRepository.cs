using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interfaces
{
    public interface IAddOnRepository
    {
        Task<IEnumerable<AddOn>> GetAllAddOnsAsync();

        Task<IEnumerable<AddOn>> GetAddOnsByIdsAsync(IEnumerable<int> ids);
    }
}
