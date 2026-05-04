using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Src.Service
{
    public interface IMembershipService
    {
        IEnumerable<Membership> GetAllMemberships();

        Membership? UpgradeUserMembership(int userId, int newMembershipId);

        MembershipPurchaseResult PurchaseMembership(int userId, int membershipId);
    }
}
