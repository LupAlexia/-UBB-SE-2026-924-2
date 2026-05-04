using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Src.Service
{
    public interface IMembershipService
    {
        Task<IEnumerable<Membership>> GetAllMembershipsAsync();

        Task<Membership?> UpgradeUserMembershipAsync(int userId, int newMembershipId);

        Task<MembershipPurchaseResult> PurchaseMembershipAsync(int userId, int membershipId);
    }
}
