using System.Collections.Generic;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Entity.Dto
{
    public class SeatMapResponseDTO
    {
        public List<SeatDescriptor> Layout { get; set; } = new List<SeatDescriptor>();
        public int RowCount { get; set; }
    }
}
