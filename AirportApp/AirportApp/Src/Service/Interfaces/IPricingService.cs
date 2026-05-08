using System.Collections.Generic;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Src.Service.Interfaces
{
    public interface IPricingService
    {
        float CalculateBasePrice(Flight flight);
        float CalculateTotalPrice(FlightTicket flightTicket);
        PriceBreakdown CalculatePriceBreakdown(Flight flight, Customer user, List<FlightTicket> tickets);
    }
}
