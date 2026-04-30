using System;
using System.Collections.Generic;
using System.Linq;
using AirportApp.Src.Domain;
using AirportApp.Src.Repository;

namespace AirportApp.Src.Service
{
    public class MembershipService : IMembershipService
    {
        private readonly IUser2Repository userRepository;
        private readonly IMembershipRepository membershipRepository;

        public MembershipService(IUser2Repository userRepository, IMembershipRepository membershipRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.membershipRepository = membershipRepository ?? throw new ArgumentNullException(nameof(membershipRepository));
        }

        public IEnumerable<Membership> GetAllMemberships()
        {
            var memberships = this.membershipRepository.GetAllMemberships().ToList();

            foreach (var membership in memberships)
            {
                membership.AddonDiscounts = this.membershipRepository.GetAddonDiscounts(membership.MembershipId).ToList();
            }

            return memberships;
        }

        public Membership? UpgradeUserMembership(int userId, int newMembershipId)
        {
            this.userRepository.UpdateUserMembership(userId, newMembershipId);

            var membership = this.membershipRepository.GetMembershipById(newMembershipId);
            if (membership != null)
            {
                membership.AddonDiscounts = this.membershipRepository.GetAddonDiscounts(newMembershipId).ToList();
            }

            return membership;
        }
        public MembershipPurchaseResult PurchaseMembership(int userId, int membershipId)
        {
            try
            {
                var updatedMembership = UpgradeUserMembership(userId, membershipId);
                if (updatedMembership != null && UserSession.CurrentUser != null)
                {
                    UserSession.CurrentUser.Membership = updatedMembership;
                }

                return new MembershipPurchaseResult
                {
                    Succeeded = true,
                    Message = "Your membership purchase was completed successfully."
                };
            }
            catch
            {
                return new MembershipPurchaseResult
                {
                    Succeeded = false,
                    Message = "Membership purchase could not be completed. Please try again."
                };
            }
        }
    }
}
