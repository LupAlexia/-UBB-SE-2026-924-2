using System.Collections.Generic;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    public class PassengerData
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string SelectedSeat { get; set; } = string.Empty;

        public List<AddOn> SelectedAddOns { get; set; } = new List<AddOn>();
    }
}
