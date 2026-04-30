using System.Collections.Generic;
using AirportApp.Src.Domain;

namespace AirportApp.Src.Service
{
    public interface IPricingService
    {
        float CalculateBasePrice(Flight flight);
        float CalculateTotalPrice(FlightTicket FlightTicket);
        PriceBreakdown CalculatePriceBreakdown(Flight flight, User2 user, List<FlightTicket> tickets);
    }
}
