using System.Collections.Generic;
using AirportApp.Src.Domain;
using AirportApp.Src.Model;

namespace AirportApp.Src.Service
{
    public interface IPricingService
    {
        float CalculateBasePrice(Flight flight);
        float CalculateTotalPrice(FlightTicket FlightTicket);
        PriceBreakdown CalculatePriceBreakdown(Flight flight, User user, List<FlightTicket> tickets);
    }
}
