using AirportApp.Src.Domain;

namespace AirportApp.Src.Repository
{
    public interface IUser2Repository
    {
        User2? GetById(int id);

        User2? GetByEmail(string email);

        void AddUser(User2 user);

        void UpdateUserMembership(int userId, int newMembershipId);
    }
}
