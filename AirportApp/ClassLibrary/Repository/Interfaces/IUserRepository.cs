using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<int> CreateNewEntityAsync(User userEntity);

        Task DeleteByIdAsync(int identificationNumber);

        Task<IEnumerable<User>> GetAllAsync();

        Task<User> GetByIdAsync(int identificationNumber);

        Task UpdateByIdAsync(int identificationNumber, User userEntity);
    }
}
