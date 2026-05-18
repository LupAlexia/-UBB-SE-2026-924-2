using System.Collections.Generic;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interfaces
{
    public interface IPricingService
    {
        Task<float> CalculateBasePriceAsync(Flight flight);
        Task<float> CalculateTotalPriceAsync(FlightTicket flightTicket);
        Task<PriceBreakdown> CalculatePriceBreakdownAsync(Flight flight, Customer user, List<FlightTicket> tickets);
    }
}
