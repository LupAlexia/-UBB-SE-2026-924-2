using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interfaces
{
    public interface IAuthService
    {
        Task<Customer> LoginAsync(string email, string password, int? currentUserId = null);
        Task RegisterAsync(string email, string phone, string username, string password);
        void Logout();
        Task<Customer> GetByIdAsync(int id);
        Task<Customer?> GetByEmailAsync(string email);
        Task AddUserAsync(Customer customer);
    }
}