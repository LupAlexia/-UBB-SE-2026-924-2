using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.Service.Interfaces;

namespace AirportApp.Src.Service
{
    public class MembershipService : IMembershipService
    {
        private readonly ICustomerRepository userRepository;
        private readonly IMembershipRepository membershipRepository;

        public MembershipService(ICustomerRepository userRepository, IMembershipRepository membershipRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.membershipRepository = membershipRepository ?? throw new ArgumentNullException(nameof(membershipRepository));
        }

        public async Task<IEnumerable<Membership>> GetAllMembershipsAsync()
        {
            var memberships = (await this.membershipRepository.GetAllMembershipsAsync()).ToList();
            foreach (var membership in memberships)
            {
                membership.AddonDiscounts = (await this.membershipRepository.GetAddonDiscountsAsync(membership.Id)).ToList();
            }

            return memberships;
        }

        public async Task<Membership?> GetMembershipByIdAsync(int id)
        {
            return await this.membershipRepository.GetMembershipByIdAsync(id);
        }

        public async Task<IEnumerable<MembershipAddonDiscount>> GetAddonDiscountsAsync(int membershipId)
        {
            return await this.membershipRepository.GetAddonDiscountsAsync(membershipId);
        }

        public async Task<Membership?> UpgradeUserMembershipAsync(int userId, int newMembershipId)
        {
            await this.userRepository.UpdateUserMembershipAsync(userId, newMembershipId);

            var membership = await this.membershipRepository.GetMembershipByIdAsync(newMembershipId);
            if (membership != null)
            {
                membership.AddonDiscounts = (await this.membershipRepository.GetAddonDiscountsAsync(newMembershipId)).ToList();
            }

            return membership;
        }

        public async Task<MembershipPurchaseResult> PurchaseMembershipAsync(int userId, int membershipId)
        {
            try
            {
                var updatedMembership = await UpgradeUserMembershipAsync(userId, membershipId);
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