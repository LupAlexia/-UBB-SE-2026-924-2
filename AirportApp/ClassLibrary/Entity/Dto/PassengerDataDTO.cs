using System.Collections.Generic;

namespace AirportApp.ClassLibrary.Entity.Dto
{
    public class PassengerDataDTO
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string SelectedSeat { get; set; } = string.Empty;
        public List<PricingAddOnDTO> SelectedAddOns { get; set; } = new ();
    }
}
