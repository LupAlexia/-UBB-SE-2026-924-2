using AirportApp.Src.Domain;

namespace AirportApp.Src.Service
{
    public interface IAuthService
    {
        Customer Login(string email, string password);
        void Register(string email, string phone, string username, string password);
        void Logout();
    }
}
