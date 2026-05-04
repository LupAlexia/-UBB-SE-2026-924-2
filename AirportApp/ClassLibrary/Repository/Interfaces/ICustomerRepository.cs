using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(int id);

        Task<Customer?> GetByEmailAsync(string email);

        Task AddUserAsync(Customer user);

        Task UpdateUserMembershipAsync(int userId, int newMembershipId);
    }
}
