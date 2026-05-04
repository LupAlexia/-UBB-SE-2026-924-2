using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interfaces
{
    public interface ICustomerRepository
    {
        Customer? GetById(int id);

        Customer? GetByEmail(string email);

        void AddUser(Customer user);

        void UpdateUserMembership(int userId, int newMembershipId);
    }
}
