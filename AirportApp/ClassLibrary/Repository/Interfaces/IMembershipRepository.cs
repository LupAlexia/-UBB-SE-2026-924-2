using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interfaces
{
    public interface IMembershipRepository
    {
        Task<Membership?> GetMembershipByIdAsync(int id);

        Task<IEnumerable<MembershipAddonDiscount>> GetAddonDiscountsAsync(int membershipId);

        Task<IEnumerable<Membership>> GetAllMembershipsAsync();
    }
}
