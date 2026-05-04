using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Src.Service
{
    public interface IAuthService
    {
        Task<Customer> LoginAsync(string email, string password);
        Task RegisterAsync(string email, string phone, string username, string password);
        void Logout();
    }
}
