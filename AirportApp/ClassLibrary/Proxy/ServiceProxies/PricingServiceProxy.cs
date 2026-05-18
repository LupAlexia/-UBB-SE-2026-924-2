using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interfaces;

namespace AirportApp.ClassLibrary.Proxy.ServiceProxies
{
    public class PricingServiceProxy : IPricingService
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "api/pricing";

        public PricingServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new System.ArgumentNullException(nameof(httpClient));
        }

        public async Task<float> CalculateBasePriceAsync(Flight flight)
        {
            try
            {
                var req = new CalculateBasePriceRequestDTO
                {
                    DepartureTime = flight.Route?.DepartureTime ?? DateTime.Now,
                    ArrivalTime = flight.Route?.ArrivalTime ?? DateTime.Now
                };
                var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/calculate-base-price", req);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<float>();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException("Failed to calculate base price.", httpRequestException);
            }
        }

        public async Task<float> CalculateTotalPriceAsync(FlightTicket ticket)
        {
            try
            {
                var req = new CalculateTotalPriceRequestDTO
                {
                    BasePrice = ticket.Price,
                    FlightDiscountPercentage = ticket.User?.Membership?.FlightDiscountPercentage ?? 0,
                    AddonDiscounts = ticket.User?.Membership?.AddonDiscounts?.Select(d => new PricingAddOnDiscountDTO
                    {
                        AddOnId = d.AddOn?.Id ?? 0,
                        DiscountPercentage = d.DiscountPercentage
                    }).ToList() ?? new List<PricingAddOnDiscountDTO>(),
                    SelectedAddOns = ticket.SelectedAddOns?.Select(a => new PricingAddOnDTO
                    {
                        Id = a.Id,
                        BasePrice = a.BasePrice
                    }).ToList() ?? new List<PricingAddOnDTO>()
                };
                var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/calculate-total-price", req);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<float>();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException("Failed to calculate total price.", httpRequestException);
            }
        }

        public async Task<PriceBreakdown> CalculatePriceBreakdownAsync(Flight flight, Customer user, List<FlightTicket> tickets)
        {
            try
            {
                var req = new CalculatePriceBreakdownRequestDTO
                {
                    FlightData = new CalculateBasePriceRequestDTO
                    {
                        DepartureTime = flight.Route?.DepartureTime ?? DateTime.Now,
                        ArrivalTime = flight.Route?.ArrivalTime ?? DateTime.Now
                    },
                    FlightDiscountPercentage = user?.Membership?.FlightDiscountPercentage ?? 0,
                    AddonDiscounts = user?.Membership?.AddonDiscounts?.Select(d => new PricingAddOnDiscountDTO
                    {
                        AddOnId = d.AddOn?.Id ?? 0,
                        DiscountPercentage = d.DiscountPercentage
                    }).ToList() ?? new List<PricingAddOnDiscountDTO>(),
                    TicketsAddOns = tickets?.Select(t => t.SelectedAddOns?.Select(a => new PricingAddOnDTO
                    {
                        Id = a.Id,
                        BasePrice = a.BasePrice
                    }).ToList() ?? new List<PricingAddOnDTO>()).ToList() ?? new List<List<PricingAddOnDTO>>()
                };

                var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/calculate-breakdown", req);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<PriceBreakdown>();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new InvalidOperationException("Failed to calculate price breakdown.", httpRequestException);
            }
        }
    }
}
