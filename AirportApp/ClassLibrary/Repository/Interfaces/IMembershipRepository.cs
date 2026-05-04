using System.Collections.Generic;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interfaces
{
    public interface IMembershipRepository
    {
        Membership? GetMembershipById(int id);

        IEnumerable<MembershipAddonDiscount> GetAddonDiscounts(int membershipId);

        IEnumerable<Membership> GetAllMemberships();
    }
}
