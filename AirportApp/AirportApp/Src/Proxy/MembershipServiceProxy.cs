using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.Src.Service.Interfaces;

namespace AirportApp.Src.Proxy
{
    public class MembershipServiceProxy : IMembershipService
    {
        private const string MembershipBaseUrl = "api/membership";
        private const string CustomerBaseUrl = "api/customer";

        private readonly HttpClient httpClient;

        public MembershipServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        // HTTP calls replace: membershipRepository.GetAllMembershipsAsync + GetAddonDiscountsAsync
        public async Task<IEnumerable<Membership>> GetAllMembershipsAsync()
        {
            var membershipsList = await this.httpClient
                .GetFromJsonAsync<IEnumerable<Membership>>(MembershipBaseUrl);

            if (membershipsList == null)
            {
                return new List<Membership>();
            }

            var memberships = membershipsList.ToList();

            foreach (var membership in memberships)
            {
                var discounts = await this.httpClient
                    .GetFromJsonAsync<IEnumerable<MembershipAddonDiscount>>(
                        $"{MembershipBaseUrl}/{membership.Id}/addon-discounts");

                if (discounts != null)
                {
                    membership.AddonDiscounts = discounts.ToList();
                }
            }

            return memberships;
        }

        // HTTP calls replace: userRepository.UpdateUserMembershipAsync + membershipRepository.GetMembershipByIdAsync + GetAddonDiscountsAsync
        public async Task<Membership?> UpgradeUserMembershipAsync(int userId, int newMembershipId)
        {
            // HTTP PUT replaces userRepository.UpdateUserMembershipAsync
            HttpResponseMessage updateResponse = await this.httpClient
                .PutAsJsonAsync($"{CustomerBaseUrl}/{userId}/membership", newMembershipId);

            updateResponse.EnsureSuccessStatusCode();

            // HTTP GET replaces membershipRepository.GetMembershipByIdAsync
            var membership = await this.httpClient
                .GetFromJsonAsync<Membership>($"{MembershipBaseUrl}/{newMembershipId}");

            if (membership != null)
            {
                // HTTP GET replaces membershipRepository.GetAddonDiscountsAsync
                var discounts = await this.httpClient
                    .GetFromJsonAsync<IEnumerable<MembershipAddonDiscount>>(
                        $"{MembershipBaseUrl}/{newMembershipId}/addon-discounts");

                if (discounts != null)
                {
                    membership.AddonDiscounts = discounts.ToList();
                }
            }

            return membership;
        }

        // Logic combining other methods (UpgradeUserMembershipAsync)
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
