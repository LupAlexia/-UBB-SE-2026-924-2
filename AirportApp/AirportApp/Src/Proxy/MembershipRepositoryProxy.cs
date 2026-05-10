using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.Src.Proxy
{
    public class MembershipRepositoryProxy : IMembershipRepository
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/membership";

        public MembershipRepositoryProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<Membership?> GetMembershipByIdAsync(int id)
        {
            try
            {
                var membershipTransferObject = await httpClient.GetFromJsonAsync<AirportApp.ClassLibrary.Entity.Dto.MembershipDTO>($"{BaseUrl}/{id}");
                if (membershipTransferObject == null)
                {
                    return null;
                }

                return new Membership
                {
                    Id = membershipTransferObject.id,
                    Name = membershipTransferObject.name,
                    FlightDiscountPercentage = membershipTransferObject.flightDiscountPercentage
                };
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

        public async Task<IEnumerable<Membership>> GetAllMembershipsAsync()
        {
            var membershipTransferObjectList = await httpClient.GetFromJsonAsync<IEnumerable<AirportApp.ClassLibrary.Entity.Dto.MembershipDTO>>(BaseUrl);
            if (membershipTransferObjectList == null)
            {
                return new List<Membership>();
            }

            return membershipTransferObjectList.Select(membershipTransferObject => new Membership
            {
                Id = membershipTransferObject.id,
                Name = membershipTransferObject.name,
                FlightDiscountPercentage = membershipTransferObject.flightDiscountPercentage
            }).ToList();
        }

        public async Task<IEnumerable<MembershipAddonDiscount>> GetAddonDiscountsAsync(int membershipId)
        {
            try
            {
                var membershipAddonDiscountTransferObjectList = await httpClient.GetFromJsonAsync<IEnumerable<AirportApp.ClassLibrary.Entity.Dto.MembershipAddonDiscountDTO>>($"{BaseUrl}/{membershipId}/addon-discounts");
                if (membershipAddonDiscountTransferObjectList == null)
                {
                    return new List<MembershipAddonDiscount>();
                }

                var discounts = new List<MembershipAddonDiscount>();
                foreach (var membershipAddonDiscountTransferObject in membershipAddonDiscountTransferObjectList)
                {
                    discounts.Add(new MembershipAddonDiscount
                    {
                        Membership = new Membership
                        {
                            Id = membershipAddonDiscountTransferObject.membershipId
                        },
                        DiscountPercentage = membershipAddonDiscountTransferObject.discountPercentage,
                        AddOn = new AddOn { Id = membershipAddonDiscountTransferObject.addOnId, Name = membershipAddonDiscountTransferObject.addOnName }
                    });
                }
                return discounts;
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException($"Failed to retrieve addon discounts for membership {membershipId}.", httpRequestException);
            }
        }
    }
}