using System;
using System.Collections.Generic;
using System.Linq;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interfaces;

namespace AirportApp.Src.Service
{
    public class PricingService : IPricingService
    {
        private const float PricePerMinuteMultiplier = 1.25f;
        private const float MinimumFlightPrice = 40f;
        private const float PercentageDivisor = 100.0f;
        private const float ZeroPrice = 0f;

        public Task<float> CalculateBasePriceAsync(Flight flight)
        {
            if (flight?.Route == null)
            {
                return Task.FromResult(ZeroPrice);
            }

            TimeSpan duration = flight.Route.ArrivalTime - flight.Route.DepartureTime;
            float calculatedPrice = (float)duration.TotalMinutes * PricePerMinuteMultiplier;
            return Task.FromResult(Math.Max(calculatedPrice, MinimumFlightPrice));
        }

        public Task<float> CalculateTotalPriceAsync(FlightTicket ticket)
        {
            float finalTotal = ticket.Price;

            if (ticket.User != null && ticket.User.Membership != null)
            {
                float flightDiscount = ticket.User.Membership.FlightDiscountPercentage;
                finalTotal -= finalTotal * (flightDiscount / PercentageDivisor);

                foreach (var addon in ticket.SelectedAddOns)
                {
                    float addonPrice = addon.BasePrice;
                    float specificAddonDiscount = ZeroPrice;

                    if (ticket.User.Membership.AddonDiscounts != null)
                    {
                        foreach (var discount in ticket.User.Membership.AddonDiscounts)
                        {
                            if (discount.AddOn != null && discount.AddOn.Id == addon.Id)
                            {
                                specificAddonDiscount = discount.DiscountPercentage;
                                break;
                            }
                        }
                    }

                    finalTotal += addonPrice - (addonPrice * (specificAddonDiscount / PercentageDivisor));
                }
            }
            else
            {
                foreach (var addon in ticket.SelectedAddOns)
                {
                    finalTotal += addon.BasePrice;
                }
            }

            return Task.FromResult(finalTotal);
        }

        public async Task<PriceBreakdown> CalculatePriceBreakdownAsync(Flight flight, Customer user, List<FlightTicket> tickets)
        {
            if (flight == null || tickets == null || tickets.Count == 0)
            {
                return new PriceBreakdown();
            }

            float basePrice = await CalculateBasePriceAsync(flight);
            float basePriceTotal = basePrice * tickets.Count;

            float addOnsWithoutMembership = tickets.Sum(ticket => ticket.SelectedAddOns.Sum(addOn => addOn.BasePrice));
            float totalWithoutMembership = basePriceTotal + addOnsWithoutMembership;

            float finalTotal = ZeroPrice;
            foreach (var ticket in tickets)
            {
                ticket.User = user;
                ticket.Price = basePrice;
                finalTotal += await CalculateTotalPriceAsync(ticket);
            }

            float membershipSavings = Math.Max(0, totalWithoutMembership - finalTotal);

            return new PriceBreakdown
            {
                BasePricePerPerson = basePrice,
                BasePriceTotal = basePriceTotal,
                AddOnsTotal = addOnsWithoutMembership,
                MembershipSavings = membershipSavings,
                FinalTotal = finalTotal
            };
        }
    }
}
