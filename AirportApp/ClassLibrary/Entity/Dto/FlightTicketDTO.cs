using System.Collections.Generic;

namespace AirportApp.ClassLibrary.Entity.Dto
{
    public record FlightTicketDTO(
        int Id, 
        int UserId, 
        int FlightId, 
        string Seat, 
        float Price, 
        string Status, 
        string PassengerFirstName, 
        string PassengerLastName, 
        string PassengerEmail, 
        string PassengerPhone,
        List<AddOnDTO> SelectedAddOns);
}
