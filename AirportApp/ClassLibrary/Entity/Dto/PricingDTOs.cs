using System;
using System.Collections.Generic;
using System.Linq;

namespace AirportApp.ClassLibrary.Entity.Dto
{
    public class CalculateBasePriceRequestDTO
    {
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
    }

    public class CalculateTotalPriceRequestDTO
    {
        public float BasePrice { get; set; }
        public float FlightDiscountPercentage { get; set; }
        public List<PricingAddOnDTO> SelectedAddOns { get; set; } = new();
        public List<PricingAddOnDiscountDTO> AddonDiscounts { get; set; } = new();
    }

    public class PricingAddOnDTO
    {
        public int Id { get; set; }
        public float BasePrice { get; set; }
    }

    public class PricingAddOnDiscountDTO
    {
        public int AddOnId { get; set; }
        public float DiscountPercentage { get; set; }
    }

    public class CalculatePriceBreakdownRequestDTO
    {
        public CalculateBasePriceRequestDTO FlightData { get; set; } = new();
        public float FlightDiscountPercentage { get; set; }
        public List<PricingAddOnDiscountDTO> AddonDiscounts { get; set; } = new();
        public List<List<PricingAddOnDTO>> TicketsAddOns { get; set; } = new();
    }
}
