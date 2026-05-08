using System;
using System.Collections.Generic;
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
                return await httpClient.GetFromJsonAsync<Membership>($"{BaseUrl}/{id}");
            }
            catch (HttpRequestException ex)
            {
                throw new KeyNotFoundException($"Membership with id {id} not found.", ex);
            }
        }

        public async Task<IEnumerable<Membership>> GetAllMembershipsAsync()
        {
            return await httpClient.GetFromJsonAsync<IEnumerable<Membership>>(BaseUrl)
                   ?? new List<Membership>();
        }

        public async Task<IEnumerable<MembershipAddonDiscount>> GetAddonDiscountsAsync(int membershipId)
        {
            try
            {
                return await httpClient.GetFromJsonAsync<IEnumerable<MembershipAddonDiscount>>($"{BaseUrl}/{membershipId}/addon-discounts")
                       ?? new List<MembershipAddonDiscount>();
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to retrieve addon discounts for membership {membershipId}.", ex);
            }
        }
    }
}
