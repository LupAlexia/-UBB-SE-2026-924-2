using AirportApp.Src.Domain;

namespace AirportApp.Src.Repository
{
    public interface ICustomerRepository
    {
        Customer? GetById(int id);

        Customer? GetByEmail(string email);

        void AddUser(Customer user);

        void UpdateUserMembership(int userId, int newMembershipId);
    }
}
