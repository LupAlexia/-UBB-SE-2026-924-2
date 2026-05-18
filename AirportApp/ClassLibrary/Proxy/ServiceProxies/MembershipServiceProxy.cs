using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interfaces;

namespace AirportApp.ClassLibrary.Proxy.ServiceProxies
{
    public class MembershipServiceProxy : IMembershipService
    {
        private readonly HttpClient httpClient;
        private const string MembershipBaseUrl = "api/membership";
        private const string CustomerBaseUrl = "api/customer";

        public MembershipServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<IEnumerable<Membership>> GetAllMembershipsAsync()
        {
            try
            {
                IEnumerable<MembershipDTO> membershipTransferObjectList = await httpClient.GetFromJsonAsync<IEnumerable<MembershipDTO>>(MembershipBaseUrl);
                if (membershipTransferObjectList == null)
                {
                    return new List<Membership>();
                }

                var memberships = new List<Membership>();
                foreach (MembershipDTO membershipTransferObject in membershipTransferObjectList)
                {
                    memberships.Add(MapMembershipFromTransferObject(membershipTransferObject));
                }

                return memberships;
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException("Failed to retrieve all memberships.", httpRequestException);
            }
        }

        public async Task<Membership?> GetMembershipByIdAsync(int id)
        {
            try
            {
                MembershipDTO membershipTransferObject = await httpClient.GetFromJsonAsync<MembershipDTO>($"{MembershipBaseUrl}/{id}");
                if (membershipTransferObject == null)
                {
                    return null;
                }

                return MapMembershipFromTransferObject(membershipTransferObject);
            }
            catch (HttpRequestException httpRequestException) when (httpRequestException.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Server communication error while retrieving membership {id}.", httpRequestException);
            }
        }

        public async Task<IEnumerable<MembershipAddonDiscount>> GetAddonDiscountsAsync(int membershipId)
        {
            try
            {
                IEnumerable<MembershipAddonDiscountDTO> discountTransferObjectList = await httpClient.GetFromJsonAsync<IEnumerable<MembershipAddonDiscountDTO>>($"{MembershipBaseUrl}/{membershipId}/addon-discounts");
                if (discountTransferObjectList == null)
                {
                    return new List<MembershipAddonDiscount>();
                }

                var discounts = new List<MembershipAddonDiscount>();
                foreach (MembershipAddonDiscountDTO discountTransferObject in discountTransferObjectList)
                {
                    discounts.Add(new MembershipAddonDiscount
                    {
                        DiscountPercentage = discountTransferObject.discountPercentage,
                        AddOn = new AddOn
                        {
                            Id = discountTransferObject.addOnId,
                            Name = discountTransferObject.addOnName
                        }
                    });
                }

                return discounts;
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to retrieve addon discounts for membership {membershipId}.", httpRequestException);
            }
        }

        public async Task<Membership?> UpgradeUserMembershipAsync(int userId, int newMembershipId)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{CustomerBaseUrl}/{userId}/membership", newMembershipId);
                response.EnsureSuccessStatusCode();

                return await GetMembershipByIdAsync(newMembershipId);
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to upgrade membership for user {userId}.", httpRequestException);
            }
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

        private static Membership MapMembershipFromTransferObject(MembershipDTO membershipTransferObject)
        {
            return new Membership
            {
                Id = membershipTransferObject.id,
                Name = membershipTransferObject.name,
                FlightDiscountPercentage = membershipTransferObject.flightDiscountPercentage
            };
        }
    }
}
