using System.ComponentModel.DataAnnotations.Schema;

namespace AirportApp.ClassLibrary.Entity.Domain

{
    /// <summary>
    /// Result of parsing booking navigation parameters.
    /// Used by BookingService.ParseBookingParameters to return
    /// a clean, typed result instead of raw object arrays.
    /// </summary>
    /// 
    [NotMapped]
    public class BookingParametersResult
    {
        public Flight? Flight { get; set; }
        public Customer? User { get; set; }
        public int RequestedPassengers { get; set; }
    }
}