using System;
using System.Collections.Generic;
using AirportApp.Src.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.Src.Service;

namespace AirportApp.Tests.Unit.Src.Service
{
    [TestClass]
    public class PricingServiceTests
    {
        private const float PricePerMinuteMultiplier = 1.25f;
        private const float ZeroPrice = 0f;
        private const float MinimumFlightPrice = 40.0f;
        private const float StandardFlightPrice = 100.0f;
        private const float StandardAddOnPrice1 = 50.0f;
        private const float StandardAddOnPrice2 = 25.0f;
        private const float StandardAddOnPrice3 = 30.0f;
        private const float StandardFlightDiscountPercentage = 10.0f;
        private const float StandardAddOnDiscountPercentage = 20.0f;
        private const int ShortFlightDurationMinutes = 10;
        private const int LongFlightDurationMinutes = 100;
        private const float PercentageDivisor = 100.0f;
        private const int BagageAddOnId = 1;
        private const int PriorityAddOnId = 2;
        private const int UnmatchedAddOnId = 99;
        private const float ExpectedDefaultBreakdownValue = 0f;

        private readonly PricingService pricingService = new PricingService();

        private static Route CreateRoute(int durationMinutes)
        {
            var now = DateTime.Now;
            return new Route
            {
                DepartureTime = now,
                ArrivalTime = now.AddMinutes(durationMinutes)
            };
        }

        private static Flight CreateFlightWithDuration(int durationMinutes)
        {
            return new Flight { Route = CreateRoute(durationMinutes) };
        }

        private static Flight CreateFlightWithBasePrice(float targetPrice)
        {
            int minutes = (int)(targetPrice / PricePerMinuteMultiplier);
            return CreateFlightWithDuration(minutes);
        }

        private static Customer CreateBasicCustomer()
        {
            return new Customer
            {
                Id = 1,
                Email = "test@test.com",
                Username = "testuser",
                PasswordHash = "hash",
                Membership = null
            };
        }

        private static Customer CreateCustomerWithMembership(Membership membership)
        {
            return new Customer
            {
                Id = 1,
                Email = "test@test.com",
                Username = "testuser",
                PasswordHash = "hash",
                Membership = membership
            };
        }

        [TestMethod]
        public void CalculateBasePrice_FlightOrRouteNull_ReturnsZero()
        {
            var flightWithNoRoute = new Flight { Route = null! };

            var resultForNullFlight = pricingService.CalculateBasePrice(null!);
            var resultForNullRoute = pricingService.CalculateBasePrice(flightWithNoRoute);

            Assert.AreEqual(ZeroPrice, resultForNullFlight);
            Assert.AreEqual(ZeroPrice, resultForNullRoute);
        }

        [TestMethod]
        public void CalculateBasePrice_ShortFlight_ReturnsMinimumPrice()
        {
            var flight = CreateFlightWithDuration(ShortFlightDurationMinutes);

            var price = pricingService.CalculateBasePrice(flight);

            Assert.AreEqual(MinimumFlightPrice, price);
        }

        [TestMethod]
        public void CalculateBasePrice_LongFlight_ReturnsCalculatedPrice()
        {
            var flight = CreateFlightWithDuration(LongFlightDurationMinutes);

            var price = pricingService.CalculateBasePrice(flight);

            Assert.AreEqual(LongFlightDurationMinutes * PricePerMinuteMultiplier, price);
        }

        [TestMethod]
        public void CalculateTotalPrice_NoMembership_ReturnsBasePricePlusAddOns()
        {
            var customer = CreateBasicCustomer();
            var ticket = new FlightTicket
            {
                Price = StandardFlightPrice,
                User = customer,
                SelectedAddOns = new List<AddOn>
                {
                    new AddOn { BasePrice = StandardAddOnPrice1 },
                    new AddOn { BasePrice = StandardAddOnPrice2 }
                }
            };

            var totalPrice = pricingService.CalculateTotalPrice(ticket);

            Assert.AreEqual(StandardFlightPrice + StandardAddOnPrice1 + StandardAddOnPrice2, totalPrice);
        }

        [TestMethod]
        public void CalculateTotalPrice_MembershipExists_AppliesFlightDiscount()
        {
            var membership = new Membership { Id = 1, Name = "Premium", FlightDiscountPercentage = StandardFlightDiscountPercentage };
            var customer = CreateCustomerWithMembership(membership);
            var ticket = new FlightTicket
            {
                Price = StandardFlightPrice,
                User = customer,
                SelectedAddOns = new List<AddOn>()
            };

            var totalPrice = pricingService.CalculateTotalPrice(ticket);

            float expectedPrice = StandardFlightPrice - (StandardFlightPrice * (StandardFlightDiscountPercentage / PercentageDivisor));
            Assert.AreEqual(expectedPrice, totalPrice);
        }

        [TestMethod]
        public void CalculateTotalPrice_MembershipExists_AppliesAddOnDiscounts()
        {
            var addon1 = new AddOn { Id = BagageAddOnId, Name = "Bagaj", BasePrice = StandardAddOnPrice1 };
            var addon2 = new AddOn { Id = PriorityAddOnId, Name = "Prioritate", BasePrice = StandardAddOnPrice3 };
            var membership = new Membership { Id = 1, Name = "Premium", FlightDiscountPercentage = StandardFlightDiscountPercentage };
            var addonDiscount = new MembershipAddonDiscount(membership, addon1, StandardAddOnDiscountPercentage);
            membership.AddonDiscounts = new List<MembershipAddonDiscount> { addonDiscount };
            var customer = CreateCustomerWithMembership(membership);
            var ticket = new FlightTicket
            {
                Price = StandardFlightPrice,
                User = customer,
                SelectedAddOns = new List<AddOn> { addon1, addon2 }
            };

            var totalPrice = pricingService.CalculateTotalPrice(ticket);

            float expectedFlightPrice = StandardFlightPrice - (StandardFlightPrice * (StandardFlightDiscountPercentage / PercentageDivisor));
            float expectedAddon1Price = StandardAddOnPrice1 - (StandardAddOnPrice1 * (StandardAddOnDiscountPercentage / PercentageDivisor));
            float expectedPrice = expectedFlightPrice + expectedAddon1Price + StandardAddOnPrice3;
            Assert.AreEqual(expectedPrice, totalPrice);
        }

        [TestMethod]
        public void CalculatePriceBreakdown_BasicUser_WorksCorrectly()
        {
            var flight = CreateFlightWithBasePrice(StandardFlightPrice);
            var customer = CreateBasicCustomer();
            var tickets = new List<FlightTicket> { new FlightTicket { Price = StandardFlightPrice } };

            var breakdown = pricingService.CalculatePriceBreakdown(flight, customer, tickets);

            Assert.AreEqual(StandardFlightPrice, breakdown.FinalTotal);
            Assert.AreEqual(ZeroPrice, breakdown.MembershipSavings);
        }

        [TestMethod]
        public void CalculatePriceBreakdown_MembershipUser_AppliesDiscount()
        {
            var flight = CreateFlightWithBasePrice(StandardFlightPrice);
            var membership = new Membership { Id = 1, Name = "Premium", FlightDiscountPercentage = StandardFlightDiscountPercentage };
            var customer = CreateCustomerWithMembership(membership);
            var tickets = new List<FlightTicket> { new FlightTicket { Price = StandardFlightPrice } };

            var breakdown = pricingService.CalculatePriceBreakdown(flight, customer, tickets);

            float expectedDiscount = StandardFlightPrice * (StandardFlightDiscountPercentage / PercentageDivisor);
            Assert.AreEqual(expectedDiscount, breakdown.MembershipSavings);
            Assert.AreEqual(StandardFlightPrice - expectedDiscount, breakdown.FinalTotal);
        }

        [TestMethod]
        public void CalculatePriceBreakdown_WithAddOns_IncludesAddOns()
        {
            var flight = CreateFlightWithBasePrice(StandardFlightPrice);
            var customer = CreateBasicCustomer();
            var ticket = new FlightTicket { Price = StandardFlightPrice };
            ticket.SelectedAddOns.Add(new AddOn { Name = "Bagaj", BasePrice = StandardAddOnPrice1 });
            var tickets = new List<FlightTicket> { ticket };

            var breakdown = pricingService.CalculatePriceBreakdown(flight, customer, tickets);

            Assert.AreEqual(StandardAddOnPrice1, breakdown.AddOnsTotal);
            Assert.AreEqual(StandardFlightPrice + StandardAddOnPrice1, breakdown.FinalTotal);
        }

        [TestMethod]
        public void CalculatePriceBreakdown_MultiplePassengers_HandlesCorrectly()
        {
            var flight = CreateFlightWithBasePrice(StandardFlightPrice);
            var customer = CreateBasicCustomer();
            var tickets = new List<FlightTicket>
            {
                new FlightTicket { Price = StandardFlightPrice },
                new FlightTicket { Price = StandardFlightPrice }
            };

            var breakdown = pricingService.CalculatePriceBreakdown(flight, customer, tickets);

            Assert.AreEqual(StandardFlightPrice * tickets.Count, breakdown.BasePriceTotal);
            Assert.AreEqual(StandardFlightPrice * tickets.Count, breakdown.FinalTotal);
        }

        [TestMethod]
        public void CalculatePriceBreakdown_MembershipUser_CalculatesSavingsCorrectly()
        {
            var flight = CreateFlightWithBasePrice(StandardFlightPrice);
            var membership = new Membership { Id = 1, Name = "Premium", FlightDiscountPercentage = StandardFlightDiscountPercentage };
            var customer = CreateCustomerWithMembership(membership);
            var tickets = new List<FlightTicket>
            {
                new FlightTicket { Price = StandardFlightPrice },
                new FlightTicket { Price = StandardFlightPrice }
            };

            var breakdown = pricingService.CalculatePriceBreakdown(flight, customer, tickets);

            float expectedSavings = (StandardFlightPrice * (StandardFlightDiscountPercentage / PercentageDivisor)) * tickets.Count;
            Assert.AreEqual(expectedSavings, breakdown.MembershipSavings);
            Assert.AreEqual((StandardFlightPrice * tickets.Count) - expectedSavings, breakdown.FinalTotal);
        }

        [TestMethod]
        public void CalculatePriceBreakdown_NullOrEmptyArguments_ReturnsDefault()
        {
            var flight = CreateFlightWithBasePrice(StandardFlightPrice);
            var customer = CreateBasicCustomer();
            var emptyTickets = new List<FlightTicket>();

            var breakdownNullFlight = pricingService.CalculatePriceBreakdown(null!, customer, emptyTickets);
            var breakdownNullTickets = pricingService.CalculatePriceBreakdown(flight, customer, null!);
            var breakdownEmptyTickets = pricingService.CalculatePriceBreakdown(flight, customer, emptyTickets);

            Assert.AreEqual(ExpectedDefaultBreakdownValue, breakdownNullFlight.FinalTotal);
            Assert.AreEqual(ExpectedDefaultBreakdownValue, breakdownNullTickets.FinalTotal);
            Assert.AreEqual(ExpectedDefaultBreakdownValue, breakdownEmptyTickets.FinalTotal);
        }

        [TestMethod]
        public void CalculateTotalPrice_DiscountsListNull_AppliesNoAddonDiscount()
        {
            var membership = new Membership { Id = 1, Name = "Premium", FlightDiscountPercentage = StandardFlightDiscountPercentage };
            membership.AddonDiscounts = null!;
            var customer = CreateCustomerWithMembership(membership);
            var addon = new AddOn { Id = BagageAddOnId, Name = "Bagaj", BasePrice = StandardAddOnPrice1 };
            var ticket = new FlightTicket
            {
                Price = StandardFlightPrice,
                User = customer,
                SelectedAddOns = new List<AddOn> { addon }
            };

            var totalPrice = pricingService.CalculateTotalPrice(ticket);

            float expectedFlightPrice = StandardFlightPrice - (StandardFlightPrice * (StandardFlightDiscountPercentage / PercentageDivisor));
            Assert.AreEqual(expectedFlightPrice + StandardAddOnPrice1, totalPrice);
        }

        [TestMethod]
        public void CalculateTotalPrice_AddonIdMismatch_AppliesNoAddonDiscount()
        {
            var selectedAddon = new AddOn { Id = BagageAddOnId, Name = "Bagaj", BasePrice = StandardAddOnPrice1 };
            var discountedAddon = new AddOn { Id = UnmatchedAddOnId, Name = "Unrelated", BasePrice = StandardAddOnPrice2 };
            var membership = new Membership { Id = 1, Name = "Premium", FlightDiscountPercentage = StandardFlightDiscountPercentage };
            var addonDiscount = new MembershipAddonDiscount(membership, discountedAddon, StandardAddOnDiscountPercentage);
            membership.AddonDiscounts = new List<MembershipAddonDiscount> { addonDiscount };
            var customer = CreateCustomerWithMembership(membership);
            var ticket = new FlightTicket
            {
                Price = StandardFlightPrice,
                User = customer,
                SelectedAddOns = new List<AddOn> { selectedAddon }
            };

            var totalPrice = pricingService.CalculateTotalPrice(ticket);

            float expectedFlightPrice = StandardFlightPrice - (StandardFlightPrice * (StandardFlightDiscountPercentage / PercentageDivisor));
            Assert.AreEqual(expectedFlightPrice + StandardAddOnPrice1, totalPrice);
        }

        [TestMethod]
        public void CalculatePriceBreakdown_ComplexDiscountScenario_CalculatesCorrectly()
        {
            var flight = CreateFlightWithBasePrice(StandardFlightPrice);
            var addon = new AddOn { Id = BagageAddOnId, Name = "Bagaj", BasePrice = StandardAddOnPrice1 };
            var membership = new Membership { Id = 1, Name = "Premium", FlightDiscountPercentage = StandardFlightDiscountPercentage };
            var addonDiscount = new MembershipAddonDiscount(membership, addon, StandardAddOnDiscountPercentage);
            membership.AddonDiscounts = new List<MembershipAddonDiscount> { addonDiscount };
            var customer = CreateCustomerWithMembership(membership);
            var ticket1 = new FlightTicket { Price = StandardFlightPrice, SelectedAddOns = new List<AddOn> { addon } };
            var ticket2 = new FlightTicket { Price = StandardFlightPrice, SelectedAddOns = new List<AddOn> { addon } };
            var tickets = new List<FlightTicket> { ticket1, ticket2 };

            var breakdown = pricingService.CalculatePriceBreakdown(flight, customer, tickets);

            Assert.AreEqual(StandardFlightPrice * tickets.Count, breakdown.BasePriceTotal);
            Assert.AreEqual(StandardAddOnPrice1 * tickets.Count, breakdown.AddOnsTotal);
            Assert.IsTrue(breakdown.MembershipSavings > ZeroPrice);
        }
    }
}



