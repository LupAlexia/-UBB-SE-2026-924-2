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
                var dto = await httpClient.GetFromJsonAsync<AirportApp.ClassLibrary.Entity.Dto.MembershipDTO>($"{BaseUrl}/{id}");
                if (dto == null)
                {
                    return null;
                }

                return new Membership
                {
                    Id = dto.id,
                    Name = dto.name,
                    FlightDiscountPercentage = dto.flightDiscountPercentage
                };
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Server communication error while retrieving membership {id}.", ex);
            }
        }

        public async Task<IEnumerable<Membership>> GetAllMembershipsAsync()
        {
            var dtos = await httpClient.GetFromJsonAsync<IEnumerable<AirportApp.ClassLibrary.Entity.Dto.MembershipDTO>>(BaseUrl);
            if (dtos == null)
            {
                return new List<Membership>();
            }

            return dtos.Select(dto => new Membership
            {
                Id = dto.id,
                Name = dto.name,
                FlightDiscountPercentage = dto.flightDiscountPercentage
            }).ToList();
        }

        public async Task<IEnumerable<MembershipAddonDiscount>> GetAddonDiscountsAsync(int membershipId)
        {
            try
            {
                var dtos = await httpClient.GetFromJsonAsync<IEnumerable<AirportApp.ClassLibrary.Entity.Dto.MembershipAddonDiscountDTO>>($"{BaseUrl}/{membershipId}/addon-discounts");
                if (dtos == null)
                {
                    return new List<MembershipAddonDiscount>();
                }

                var discounts = new List<MembershipAddonDiscount>();
                foreach (var dto in dtos)
                {
                    discounts.Add(new MembershipAddonDiscount
                    {
                        MembershipId = dto.membershipId,
                        AddOnId = dto.addOnId,
                        DiscountPercentage = dto.discountPercentage,
                        AddOn = new AddOn { Id = dto.addOnId, Name = dto.addOnName }
                    });
                }
                return discounts;
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to retrieve addon discounts for membership {membershipId}.", ex);
            }
        }
    }
}