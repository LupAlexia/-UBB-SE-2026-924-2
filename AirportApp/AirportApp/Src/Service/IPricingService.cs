using System.Collections.Generic;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Src.Service
{
    public interface IPricingService
    {
        float CalculateBasePrice(Flight flight);
        float CalculateTotalPrice(FlightTicket FlightTicket);
        PriceBreakdown CalculatePriceBreakdown(Flight flight, Customer user, List<FlightTicket> tickets);
    }
}
