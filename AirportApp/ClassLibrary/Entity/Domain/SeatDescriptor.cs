
namespace AirportApp.ClassLibrary.Entity.Domain
{
    /// <summary>
    /// Describes a single seat in the seat map.
    /// The ViewModel computes these, and the View just creates a Button for each one.
    /// </summary>
    public class SeatDescriptor
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public string Label { get; set; } = string.Empty;
    }
}