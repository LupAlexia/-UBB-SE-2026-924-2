using System.Collections.Generic;

namespace AirportApp.ClassLibrary.Entity.Dto
{
    public class SaveTicketsRequestDTO
    {
        public List<FlightTicketDTO> Tickets { get; set; } = new List<FlightTicketDTO>();
        public List<List<int>> AddOnIds { get; set; } = new List<List<int>>();
    }
}
