using System.Collections.Generic;

namespace AirportApp.ClassLibrary.Entity.Dto
{
    public record FlightTicketDTO(
        int id, 
        int userId, 
        int flightId, 
        string seat, 
        float price, 
        string status, 
        string passengerFirstName, 
        string passengerLastName, 
        string passengerEmail, 
        string passengerPhone,
        List<AddOnDTO> selectedAddOns);
}
