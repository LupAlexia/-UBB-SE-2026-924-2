using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interfaces
{
    public interface IMembershipService
    {
        Task<IEnumerable<Membership>> GetAllMembershipsAsync();
        Task<Membership?> GetMembershipByIdAsync(int id);
        Task<IEnumerable<MembershipAddonDiscount>> GetAddonDiscountsAsync(int membershipId);
        Task<Membership?> UpgradeUserMembershipAsync(int userId, int newMembershipId);
        Task<MembershipPurchaseResult> PurchaseMembershipAsync(int userId, int membershipId);
    }
}