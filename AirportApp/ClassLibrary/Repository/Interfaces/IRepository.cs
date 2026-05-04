using System.Collections.Generic;
using System.Threading.Tasks;

namespace AirportApp.ClassLibrary.Repository.Interfaces
{
    public interface IRepository<TKey, TValue>
        where TValue : class
    {
        Task<TValue> GetByIdAsync(TKey id);

        Task<TKey> CreateNewEntityAsync(TValue elem);

        Task DeleteByIdAsync(TKey id);

        Task UpdateByIdAsync(TKey id, TValue elem);

        Task<IEnumerable<TValue>> GetAllAsync();
    }
}
