using System.Collections.Generic;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interfaces;

namespace AirportApp.ClassLibrary.Proxy.ServiceProxies
{
    public class PricingServiceProxy : IPricingService
    {
        public float CalculateBasePrice(Flight flight)
        {
            throw new System.NotSupportedException("CalculateBasePrice is not available through the service proxy.");
        }

        public float CalculateTotalPrice(FlightTicket flightTicket)
        {
            throw new System.NotSupportedException("CalculateTotalPrice is not available through the service proxy.");
        }

        public PriceBreakdown CalculatePriceBreakdown(Flight flight, Customer user, List<FlightTicket> tickets)
        {
            throw new System.NotSupportedException("CalculatePriceBreakdown is not available through the service proxy.");
        }
    }
}
