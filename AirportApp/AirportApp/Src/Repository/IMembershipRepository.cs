using System.Collections.Generic;
using AirportApp.Src.Domain;

namespace AirportApp.Src.Repository
{
    public interface IMembershipRepository
    {
        Membership? GetMembershipById(int id);

        IEnumerable<MembershipAddonDiscount> GetAddonDiscounts(int membershipId);

        IEnumerable<Membership> GetAllMemberships();
    }
}
